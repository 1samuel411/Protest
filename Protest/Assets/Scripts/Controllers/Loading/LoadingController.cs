using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Authentication.Login_Facebook(LoginCallback);
    }

    public void LoginGoogle()
    {
        Authentication.Login_Google(LoginCallback);
    }

    public void LoginCallback(int response)
    {
        Log.Create(2, "Login response: " + response, "LoadingController");
        if (response == 0)
        {
            _View.loading = true;
            ProtestListController.instance.Load(LoadCallback);
        }
    }

    public void LoadCallback(int response)
    {
        if(response == 0)
        {
            Hide();
        }
    }
}
