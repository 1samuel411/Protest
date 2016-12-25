using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtestListObjectView : View
{

    public ProtestModel protestToDisplay;

    public Text titleText;
    public Text locationText;
    public Text dateText;
    public Text goingText;
    public Text likesText;

    public Image iconImage;

    public Button selectButton;

    private DateTime _time;
    public void ChangeInfo(ProtestModel newModel, Sprite sprite, Action<int> callback)
    {
        protestToDisplay = newModel;

        titleText.text = protestToDisplay.name;
        locationText.text = protestToDisplay.location;
        _time = DataParser.ParseDate(newModel.date);
        dateText.text = _time.ToString("MM/dd/yy  H:mm");
        goingText.text = protestToDisplay.going.Length.ToString();


        string likesCountString = "";
        if (newModel.likes.Length >= 1000)
        {
            likesCountString = (newModel.likes.Length / 1000.0f).ToString() + "k";
        }
        else
            likesCountString = newModel.likes.Length.ToString();

        if (newModel.likes.Length >= 1000000)
        {
            likesCountString = (newModel.likes.Length / 1000000.0f).ToString() + "m";
        }
        likesText.text = likesCountString;


        iconImage.sprite = sprite;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => { callback(protestToDisplay.index); });
    }
}