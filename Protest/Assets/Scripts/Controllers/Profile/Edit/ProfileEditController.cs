using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileEditController : Controller
{

    public new static ProfileEditController instance;
    public static ProfileEditView _view;

    void Awake ()
    {
        instance = this;
        _view = view.GetComponent<ProfileEditView>();
    }

    public void Show(UserModel modelToEdit)
    {
        _view.userModel = modelToEdit;
        Show();
    }

    public void Complete(UserModel userModel)
    {
        Log.Create(2, "Editing profile", "ProfileEditController");
        DataParser.EditUser(userModel, Authentication.user.index, Authentication.auth_token);
        Hide();
        ProfileViewController.instance.Show(userModel.index, ProtestListController.instance);
    }

    public void Return()
    {
        Hide();
    }

    public void UpdateIcon()
    {

    }
}
