using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtestIconObjectView : View
{

    public Image iconImage;
    public Button inputButton;

    public void ChangeInfo(Sprite sprite, int model, Action<int> callback)
    {
        iconImage.sprite = sprite;
        inputButton.onClick.RemoveAllListeners();
        inputButton.onClick.AddListener(() => { callback(model); });
    }
}
