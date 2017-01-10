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


        likesText.text = DataParser.GetCount(newModel.likes.Length);


        iconImage.sprite = sprite;

        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => { callback(protestToDisplay.index); });
    }
}