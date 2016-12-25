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
     

    public void ChangeInfoProtest(ProtestModel protestModel, Sprite sprite, Action<int> protestCallback)
    {
        iconImage.sprite = sprite;
        nameText.text = protestModel.name;
        dateText.text = protestModel.date;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { protestCallback(protestModel.index); });
    }

    public void ChangeInfoUser(UserModel userMode, Sprite sprite, Action<int> userCallback)
    {
        iconImage.sprite = sprite;
        nameText.text = userMode.name;
        dateText.text = "";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { userCallback(userMode.index); });
    }
}
