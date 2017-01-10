using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileView : View
{

    public Image profileImage;

    public Text nameText;
    public Text bioText;

    public Button editButton;
    public Button flagButton;

    public Button followButton;
    public Text followButtonText;

    public Button snapchatButton;
    public Button facebookButton;
    public Button instagramButton;
    public Button twitterButton;

    public Text followersText;
    public Text followingText;
    public Text attendedText;
    public Text createdText;

    public RectTransform protestsHolder;

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
            RefreshData();
        }
    }

    public void RefreshData()
    {
        bool ours = userModel.index == Authentication.userIndex;

        nameText.text = userModel.name;
        bioText.text = userModel.bio;

        followersText.text = DataParser.GetCount(userModel.followers.Length);
        followingText.text = DataParser.GetCount(userModel.following.Length);
        attendedText.text = DataParser.GetCount(userModel.protestsAttended.Length);
        createdText.text = DataParser.GetCount(userModel.protestsCreated.Length);

        DataParser.SetSprite(profileImage, userModel.profilePicture);
        
        editButton.gameObject.SetActive((ours));
        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(() => { ProfileViewController.instance.EditProfile(userModel); });

        flagButton.gameObject.SetActive(!(ours));
        flagButton.onClick.RemoveAllListeners();
        flagButton.onClick.AddListener(() => { ProfileViewController.instance.ReportProfile(userModel); });

        if(Authentication.userModel != null)
            if (Authentication.userModel.following.Contains(userModel.index))
                followButtonText.text = "Following";
            else
                followButtonText.text = "Follow";

        followButton.gameObject.SetActive(!(ours));
        followButton.onClick.RemoveAllListeners();
        followButton.onClick.AddListener(() => { ProfileViewController.instance.Follow(userModel.index); });

        snapchatButton.gameObject.SetActive(!(userModel.snapchatUser.Length <= 0));
        facebookButton.gameObject.SetActive(!(userModel.facebookUser.Length <= 0));
        instagramButton.gameObject.SetActive(!(userModel.instagramUser.Length <= 0));
        twitterButton.gameObject.SetActive(!(userModel.twitterUser.Length <= 0));

        snapchatButton.onClick.RemoveAllListeners();
        snapchatButton.onClick.AddListener(() => { ProfileViewController.instance.OpenSnapchat(userModel.snapchatUser); });

        facebookButton.onClick.RemoveAllListeners();
        facebookButton.onClick.AddListener(() => { ProfileViewController.instance.OpenFacebook(userModel.facebookUser); });

        instagramButton.onClick.RemoveAllListeners();
        instagramButton.onClick.AddListener(() => { ProfileViewController.instance.OpenInstagram(userModel.instagramUser); });

        twitterButton.onClick.RemoveAllListeners();
        twitterButton.onClick.AddListener(() => { ProfileViewController.instance.OpenTwitter(userModel.twitterUser); });

        ProfileViewController.instance.PopulateProtests(userModel);
    }
    
    public void Return()
    {
        ProfileViewController.instance.Return();
    }

    public void OpenFollowers()
    {
        ProfileViewController.instance.OpenFollowers();
    }

    public void OpenFollowing()
    {
        ProfileViewController.instance.OpenFollowing();
    }

    public void OpenProtestsCreated()
    {
        ProfileViewController.instance.OpenProtestsCreated();
    }

    public void OpenAttended()
    {
        ProfileViewController.instance.OpenAttended();
    }
}
