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
        string locationTextNew = protestToDisplay.location;
        string[] splitLocation = locationTextNew.Split(',');
        if(splitLocation.Length >= 2)
        {
            locationTextNew = splitLocation[1].Trim() + ", " + splitLocation[2];
        }

        if (locationTextNew.Length >= 22)
            locationTextNew = locationTextNew.Substring(0, 22) + "...";

        locationText.text = locationTextNew;
        _time = DataParser.ParseDate(newModel.date);
        _time = _time.ToLocalTime();
        dateText.text = _time.ToString("MM/dd/yy  H:mm");

        goingText.text = DataParser.GetCount(protestToDisplay.going.Length);



        likesText.text = DataParser.GetCount(newModel.likes.Length);


        iconImage.sprite = sprite;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => { callback(protestToDisplay.index); });
    }
}