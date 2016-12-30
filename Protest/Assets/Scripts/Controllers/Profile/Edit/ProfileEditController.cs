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
        PickerEventListener.onImageLoad += OnImageLoad;
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
        DataParser.ChangeIcon();
    }

    void OnImageLoad(string imgPath, Texture2D tex, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        _view.iconImage.sprite = Sprite.Create(tex, new Rect(0, 0, 128, 128), Vector2.zero);
        _view.userModel.profilePicture = DataParser.UploadImage(tex);
    }
}
