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
        imageChanged = false;

        _view.userModel = modelToEdit;
        this.previousController = previousController;
        previousController.Hide();
        Show();
    }

    public void Complete()
    {
        SpinnerController.instance.Show();
        Log.Create(2, "Editing profile", "ProfileEditController");
        if (imageChanged)
        {
            Texture2D texture = new Texture2D(128, 128);
            texture.SetPixels(_view.image.texture.GetPixels((int)_view.image.textureRect.x, (int)_view.image.textureRect.y, (int)_view.image.textureRect.width, (int)_view.image.textureRect.height));
            texture.Apply();
            DataParser.UploadImage(texture, UploadImageCallback);
        }
        else
        {
            DataParser.EditUser(_view.userModel, Authentication.auth_token, EditUserCallback);
        }
    }

    void UploadImageCallback(string url)
    {
        _view.userModel.profilePicture = url;
        DataParser.EditUser(_view.userModel, Authentication.auth_token, EditUserCallback);
    }

    void EditUserCallback()
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

    public bool imageChanged = false;
    void GetIconCallback(Texture2D texture)
    {
        imageChanged = true;
        Debug.Log("Got Texture");
        TextureScale.Bilinear(texture, 128, 128);
        _view.image = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0, 0));
    }
}
