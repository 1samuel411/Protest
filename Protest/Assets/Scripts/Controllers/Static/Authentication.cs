using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Facebook.Unity;

public class Authentication : Base
{

    public static string auth_token;

    public static bool authenticated;

    private bool completeFB;
    private bool completeGoogle = false;

    public static int userIndex;
    public static UserModel userModel;

    void Awake()
    {
        LoadingController.instance._View.loading = true;
        FB.Init(OnInitComplete);

        InvokeRepeating("CheckInternet", 1, 15);
    }

    public static void RefreshUserModel(Action Callback)
    {
        CallbackRefresh = Callback;
        DataParser.GetUser(userIndex, GetUserCallback);
    }
    private static Action CallbackRefresh;

    private static void GetUserCallback(UserModel userModel)
    {
        OneSignal.SendTag("identification", userIndex.ToString());
        Authentication.userModel = userModel;

        ProtestListController.instance.LoadNotifications();

        if (CallbackRefresh == null)
            return;

        CallbackRefresh();
    }

    void OnInitComplete()
    {
        if (completeGoogle)
            return;

        completeFB = true;
        
        if(FB.IsLoggedIn)
        {
            Debug.Log("Login Success!");
            Authentication.Authenticate(AccessToken.CurrentAccessToken.UserId, AccessToken.CurrentAccessToken.TokenString, Authentication.LoginType.Facebook, "", "", "", "", AccessToken.CurrentAccessToken.UserId, "", "");
        }
        else
        {
            Debug.Log("Login failed!");
            LoadingController.instance._View.loading = false;
        }
    }

    void CheckInternet()
    {
        StartCoroutine(CheckInternetCoroutine());
    }

    IEnumerator CheckInternetCoroutine()
    {
        if (!responded)
            yield break;
        WWW www = new WWW(DataParser.URL);
        yield return www;

        if(!String.IsNullOrEmpty(www.error))
        {
            responded = false;
            UnAuthenticate();
            Popup.Create("Connection lost", "Could not connect to the Protest servers. Check your connection or contact us", ResponsePopup, "Popup", "Reconnect", "Contact");
        }
    }

    private bool responded = true;

    void ResponsePopup(int response)
    {
        responded = true;
        switch(response)
        {
            case 1:
                Debug.Log("Reconnecting");
                CheckInternet();
                break;
            case 2:
                Debug.Log("Contacting");
                ProtestListController.instance.Contact();
                break;
        }
    }

    public void Update()
    {
        if (!authenticated)
        {
            LoadingController.instance.Show();
        }
    }

    public static void Login_Facebook(FacebookDelegate<ILoginResult> CallbackFacebook)
    {
        Log.Create(0, "Login to facebook event", "Authentication");

        // Login to facebook and return the key to call the authenticate method
        string[] permissions = new string[] { "public_profile", "email" };
        FB.LogInWithReadPermissions(permissions, CallbackFacebook);
    }
    
    public static void Login_Google(Action<int> callback)
    {
        int response = 0;
        // Login to google and return the key to call the authenticate method


        callback.Invoke(response);
    }

    public enum LoginType {  Google, Facebook  };

    public static void Authenticate(string userId, string sessionKey, LoginType loginType, string profilePicture = "", string name = "", string email = "", string bio = "", string facebookUserToken = "", string googleUserToken = "", string facebookUser = "")
    {
        Log.Create(2, "Authentication Checking Session: " + sessionKey + " | UserId = " + userId, "Authentication");

        DataParser.AuthenticateUser(ResponseAuthenticate, loginType, sessionKey.Trim(), profilePicture, name, email, bio, facebookUserToken, googleUserToken, facebookUser);
    }

    private static void ResponseAuthenticate(string sessionToken, int newId, LoginType loginType)
    {
        authenticated = true;
        auth_token = sessionToken;
        userIndex = newId;
        Log.Create(2, "Authentication Successful", "Authentication", auth_token);

        LoadingController.instance.Load();
    }

    public static void Logout()
    {
        if(FB.IsLoggedIn)
        {
            FB.LogOut();
        }
        authenticated = false;
        LoadingController.instance._View.loading = false;
        Log.Create(2, "Logging out", "Authentication");
    }

    public static void UnAuthenticate()
    {
        Log.Create(2, "Unauthentication Successful", "Authentication");
        authenticated = false;
        auth_token = "";

        if (FB.IsLoggedIn)
            FB.LogOut();
    }
}
