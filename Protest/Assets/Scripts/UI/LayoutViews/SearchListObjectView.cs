using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchListObjectView : View
{

    public Image iconImage;
    public Text nameText;
    public Text dateText;

    public Button button;

    public Button iconButton;

    public UserModel user;

    public void ChangeInfoProtest(ProtestModel protestModel, Sprite sprite, Action<int> protestCallback)
    {
        iconImage.sprite = sprite;
        nameText.text = protestModel.name;
        dateText.text = protestModel.date;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { protestCallback(protestModel.index); });
    }

    public void ChangeInfoUser(UserModel userModel, Sprite sprite, Action<int> userCallback)
    {
        user = userModel;
        iconImage.sprite = sprite;
        nameText.text = userModel.name;
        dateText.text = "";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { userCallback(userModel.index); });
    }

    public void ChangeInfoNews(NewsModel newsModel, Sprite sprite, Action<NewsModel> notificationCallback)
    {
        iconImage.sprite = sprite;
        nameText.text = newsModel.text;
        string text = "";
        TimeSpan span = DateTime.UtcNow - (DataParser.ParseDate(newsModel.notificationTime));
        if (span.Days > 0)
            text = span.Days.ToString() + " days";
        else if (span.Hours > 0)
            text = span.Hours.ToString() + " hours";
        else if (span.Minutes > 0)
            text = span.Minutes.ToString() + " minutes";
        else if (span.Seconds >= 0)
            text = span.Seconds.ToString() + " seconds";
        else
            text = span.ToString();
        dateText.text = text + " ago";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { notificationCallback(newsModel); });

        iconButton.onClick.RemoveAllListeners();
        iconButton.onClick.AddListener(() => { ProfileViewController.instance.Show(newsModel.userIndex, ListController.instance); });
    }
}
