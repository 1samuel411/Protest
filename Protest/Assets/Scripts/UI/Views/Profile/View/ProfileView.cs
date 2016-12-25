﻿using System.Collections;
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
        bool ours = userModel.index == Authentication.user.index;

        nameText.text = userModel.name;
        bioText.text = userModel.bio;

        followersText.text = userModel.followers.Length.ToString();
        followingText.text = userModel.following.Length.ToString();
        attendedText.text = userModel.protestsAttended.Length.ToString();
        createdText.text = userModel.protestsCreated.Length.ToString();

        DataParser.SetSprite(profileImage, userModel.profilePicture);
        
        editButton.gameObject.SetActive((ours));
        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(() => { ProfileViewController.instance.EditProfile(userModel); });

        flagButton.gameObject.SetActive(!(ours));
        flagButton.onClick.RemoveAllListeners();
        flagButton.onClick.AddListener(() => { ProfileViewController.instance.ReportProfile(userModel); });

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