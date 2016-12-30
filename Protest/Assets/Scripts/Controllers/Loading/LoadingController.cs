using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class LoadingController : Controller
{

    public new static LoadingController instance;

    private LoadingView _View;

    void Awake()
    {
        instance = this;
        _View = view.GetComponent<LoadingView>();
    }

    public void LoginFacebook()
    {
        Authentication.Login_Facebook(CallbackFacebook);
    }

    public void LoginGoogle()
    {
        //Authentication.Login_Google(LoginCallback);
    }

    private void CallbackFacebook(ILoginResult result)
    {
        if (result.Cancelled || result.Error != null)
        {
            Debug.Log("Canceled or Error: " + result.Error);
        }
        else
        {
            Authentication.Authenticate(result.AccessToken.UserId, result.AccessToken.TokenString, Authentication.LoginType.Facebook);
            Debug.Log(result.AccessToken.UserId);
        }
    }

    public void Load()
    {
        _View.loading = true;
        ProtestListController.instance.Load(LoadCallback);
    }

    public void LoadCallback(int response)
    {
        if(response == 0)
        {
            Hide();
        }
    }
}
