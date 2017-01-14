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
    public Text distanceText;
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

        if (locationTextNew.Length >= 40)
            locationTextNew = locationTextNew.Substring(0, 37) + "...";

        locationText.text = locationTextNew;
        _time = DataParser.ParseDate(newModel.date);
        _time = _time.ToLocalTime();
        dateText.text = _time.DayOfWeek + ", " + _time.ToShortDateString() + "\n" + _time.ToShortTimeString();

        goingText.text = DataParser.GetCount(protestToDisplay.going.Length);



        likesText.text = DataParser.GetCount(newModel.likes.Length);


        iconImage.sprite = sprite;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => { callback(protestToDisplay.index); });

        distanceText.text = Math.Round(DataParser.CalculateDistance(Input.location.lastData.latitude, Input.location.lastData.longitude, newModel.x, newModel.y), 1).ToString() + " miles";
    }
}