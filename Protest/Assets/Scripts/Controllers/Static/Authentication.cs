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

    private static string localKey = "SessionKeyV1.0";
    private static string loginTypeKey = "LoginTypeV1.0";

    public static UserModel user
    {
        get
        {
            if (authenticated)
                return DataParser.GetUser(auth_token);
            else
                return null;
        }
    }

    void Awake()
    {
        FB.Init();
        if(PlayerPrefs.HasKey(localKey))
        {
            Debug.Log(PlayerPrefs.GetString(localKey));
            Authenticate(PlayerPrefs.GetString(localKey), (LoginType)PlayerPrefs.GetInt(loginTypeKey));
        }
    }

    public void Update()
    {
        if(!authenticated)
        {
            LoadingController.instance.Show();
        }
    }

    public static void Login_Facebook(FacebookDelegate<ILoginResult> CallbackFacebook)
    {
        Log.Create(0, "Login to facebook event", "Authentication");

        // Login to facebook and return the key to call the authenticate method
        string[] permissions = new string[] { "email", "public_profile" };
        FB.LogInWithReadPermissions(permissions, CallbackFacebook);
    }
    
    public static void Login_Google(Action<int> callback)
    {
        int response = 0;
        // Login to google and return the key to call the authenticate method


        callback.Invoke(response);
    }

    public enum LoginType {  Google, Facebook  };

    public static void Authenticate(string userId, string sessionKey, LoginType loginType)
    {
        Log.Create(2, "Authentication Checking Session: " + sessionKey + " | UserId = " + userId, "Authentication");

        DataParser.AuthenticateUser(ResponseAuthenticate, loginType, userId, sessionKey);
    }

    private static void ResponseAuthenticate(string sessionToken, string userToken, LoginType loginType)
    {
        authenticated = true;
        auth_token = sessionToken;
        PlayerPrefs.SetInt(loginTypeKey, (int)loginType);
        PlayerPrefs.SetString(localKey, auth_token);
        Log.Create(2, "Authentication Successful", "Authentication", auth_token);

        LoadingController.instance.Load();
    }

    public static void Authenticate(string sessionKey, LoginType loginType)
    {
        Log.Create(2, "Authentication Checking Session: " + sessionKey, "Authentication");

        DataParser.AuthenticateUser(ResponseAuthenticate, loginType, sessionKey);
    }

    public static void UnAuthenticate()
    {
        Log.Create(2, "Unauthentication Successful", "Authentication", auth_token);
        authenticated = false;
        auth_token = "";
        PlayerPrefs.DeleteKey(localKey);
    }


    // Move this to server
    // *******************************************************************
    private static string character = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789!@#$%^&*()-=[]/?<>,.";
    private static string GenerateSessionKey()
    {
        DateTime now = DateTime.UtcNow;
        string randomString = "";
        for(int i = 0; i < 20; i++)
        {
            randomString += character[UnityEngine.Random.Range(0, character.Length - 1)];
        }
        return (randomString + "|" + now.ToShortDateString() + "|" + now.ToShortTimeString() + "|" + now.Millisecond).Replace(' ', '_');
    }
}
