using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileEditView : View
{

    private UserModel _userModel;
    public UserModel userModel
    {
        get
        {
            return _userModel;
        }
        set
        {
            _userModel = value;
            UpdateLocal();
        }
    }

    public string nameString;
    public string bioString;

    public string snapchatUserString;
    public string facebookUserString;
    public string instagramUserString;
    public string twitterUserString;

    public bool notificationsLikesComments;
    public bool notificationsFollowing;
    public bool notificationsFollowers;

    private Sprite _image;
    public Sprite image
    {
        get
        {
            return _image;
        }
        set
        {
            _image = value;
            iconImage.sprite = value;
        }
    }


    public InputField nameField;
    public InputField bioField;
    public InputField snapchatUserField;
    public InputField facebookUserField;
    public InputField instagramUserField;
    public InputField twitterUserField;

    public Toggle notificationsLikesCommentsToggle;
    public Toggle notificationsFollowingToggle;
    public Toggle notificationsFollowersToggle;

    public Image iconImage;

    public void CallbackImage(Sprite sprite)
    {
        image = sprite;
    }

    void UpdateLocal()
    {
        nameString = userModel.name;
        bioString = userModel.bio;

        DataParser.SetSprite(userModel.profilePicture, CallbackImage);

        snapchatUserString = userModel.snapchatUser;
        facebookUserString = userModel.facebookUser;
        instagramUserString = userModel.instagramUser;
        twitterUserString = userModel.twitterUser;

        notificationsLikesComments = userModel.notifyLikesComments;
        notificationsFollowing = userModel.notifyFollowing;
        notificationsFollowers = userModel.notifyFollowers;

        UpdateFields();
    }

    void UpdateFields()
    {
        nameField.text = nameString;
        bioField.text = bioString;

        snapchatUserField.text = snapchatUserString;
        facebookUserField.text = facebookUserString;
        instagramUserField.text = instagramUserString;
        twitterUserField.text = twitterUserString;

        notificationsLikesCommentsToggle.isOn = notificationsLikesComments;
        notificationsFollowingToggle.isOn = notificationsFollowing;
        notificationsFollowersToggle.isOn = notificationsFollowers;
    }

    public void UpdateName(string input)
    {
        nameString = input;
    }

    public void UpdateBio(string input)
    {
        bioString = input;
    }

    public void UpdateSnapchat(string input)
    {
        snapchatUserString = input;
    }

    public void UpdateFacebook(string input)
    {
        facebookUserString = input;
    }

    public void UpdateInstagram(string input)
    {
        instagramUserString = input;
    }

    public void UpdateTwitter(string input)
    {
        twitterUserString = input;
    }

    public void UpdateNotificationsLikesComments(bool input)
    {
        notificationsLikesComments = input;
    }

    public void UpdateNotificationsFollowing(bool input)
    {
        notificationsFollowing = input;
    }

    public void UpdateNotificationsFollowers(bool input)
    {
        notificationsFollowers = input;
    }

    void UpdateModel()
    {
        userModel.name = nameString;
        userModel.bio = bioString;
        userModel.profilePicture = "";

        userModel.snapchatUser = snapchatUserString;
        userModel.facebookUser = facebookUserString;
        userModel.instagramUser = instagramUserString;
        userModel.twitterUser = twitterUserString;

        userModel.notifyLikesComments = notificationsLikesComments;
        userModel.notifyFollowers = notificationsFollowers;
        userModel.notifyFollowing = notificationsFollowing;
    }

    public void Confirm()
    {
        UpdateModel();
        ProfileEditController.instance.Complete(userModel);
    }

    public void Return()
    {
        ProfileEditController.instance.Return();
    }

    public void UpdateIcon()
    {
        ProfileEditController.instance.UpdateIcon();
    }
}
