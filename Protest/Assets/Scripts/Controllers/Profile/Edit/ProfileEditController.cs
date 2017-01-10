using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ImageAndVideoPicker;

public class ProfileEditController : Controller
{

    public new static ProfileEditController instance;
    public static ProfileEditView _view;

    void Awake ()
    {
        instance = this;
        _view = view.GetComponent<ProfileEditView>();
    }

    void Update()
    {
        if (!view.gameObject.activeInHierarchy)
            return;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }

    Controller previousController;
    public void Show(UserModel modelToEdit, Controller previousController = null)
    {
        _view.userModel = modelToEdit;
        this.previousController = previousController;
        previousController.Hide();
        Show();
    }

    public void Complete()
    {
        SpinnerController.instance.Show();
        Log.Create(2, "Editing profile", "ProfileEditController");
        DataParser.EditUser(_view.userModel, Authentication.auth_token, EditUserCallback);
    }

    public void EditUserCallback()
    {
        SpinnerController.instance.Hide();
        Hide();
        ProfileViewController.instance.Show(_view.userModel.index, ProtestListController.instance);
    }

    public void Return()
    {
        previousController.Show();
        Hide();
    }

    public void UpdateIcon()
    {
        DataParser.ChangeIcon(GetIconCallback);
    }

    void GetIconCallback(Texture2D texture)
    {
        Debug.Log("Got Texture");
        texture.Resize(128, 128);
        _view.image = Sprite.Create(texture, new Rect(new Vector2(0, 0), new Vector2(128, 128)), new Vector2(0, 0));
        DataParser.UploadImage(texture, UploadImageCallback);
    }

    public void UploadImageCallback(string url)
    {
        _view.userModel.profilePicture = url;
    }
}
