using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtestIconObjectView : View
{

    public Image iconImage;
    public Button inputButton;

    public void ChangeInfo(int model, Action<int> callback)
    {
        DataParser.SetSprite(iconImage, DataParser.GetProtest(model).protestPicture);
        inputButton.onClick.RemoveAllListeners();
        inputButton.onClick.AddListener(() => { callback(model); });
    }
}
