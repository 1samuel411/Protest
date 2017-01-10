using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System.Linq;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LoadingController : Controller
{

    public new static LoadingController instance;

    public LoadingView _View;

    void Awake()
    {
        Screen.fullScreen = false;
#if UNITY_EDITOR
        PlayerSettings.statusBarHidden = false;
#endif
        instance = this;
        _View = view.GetComponent<LoadingView>();

        OneSignal.StartInit("a7227cd5-532a-4b87-9dd0-080dfda19ab2")
        .HandleNotificationOpened(HandleNotificationOpened)
        .EndInit();

        OneSignal.SetSubscription(true);
    }

    // Gets called when the player opens the notification.
    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        if (Authentication.authenticated == false)
            return;

        ListController.instance.Show(ListController.ShowType.news, Authentication.userModel, ProtestListController.instance);
    }

    public void LoginFacebook()
    {
        Authentication.Login_Facebook(CallbackFacebook);
    }

    public void LoginGoogle()
    {
        //Authentication.Login_Google(LoginCallback);
        Popup.Create("Not supported", "Google login is still being developed and will be released soon!", null, "Popup", "Okay");
    }

    private void CallbackFacebook(ILoginResult result)
    {
        if (result.Cancelled || result.Error != null)
        {
            Debug.Log("Canceled or Error: " + result.Error);
        }
        else
        {
            if(FB.IsLoggedIn)
            {
                Debug.Log("Getting Data from FB");
                FB.API("/me?fields=name,email", HttpMethod.GET, CallbackFacebookInfo);
            }
        }
    }

    private void CallbackFacebookInfo(IGraphResult result)
    {
        if(!string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("Failed: " + result.Error);
            return;
        }

        string email = "";
        string nameUser = result.ResultDictionary["name"].ToString();
        if(result.ResultDictionary.Keys.Contains("email"))
            email = result.ResultDictionary["email"].ToString();
        string bio = ""; 
        Authentication.Authenticate(AccessToken.CurrentAccessToken.UserId, AccessToken.CurrentAccessToken.TokenString, Authentication.LoginType.Facebook, "https://graph.facebook.com/" + AccessToken.CurrentAccessToken.UserId + "/picture?width=128&height=128", nameUser, email, bio, AccessToken.CurrentAccessToken.UserId, "", "");
    }

    public void Load()
    {
        Authentication.RefreshUserModel(null);
        _View.loading = true;
        ProtestListController.instance.Load(LoadCallback);
        Hide();
    }

    public void LoadCallback(int response)
    {
        if(response == 0)
        {
            //Hide();
        }
    }
}
