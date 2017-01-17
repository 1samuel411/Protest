using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageView : View
{
    public Image icon;

    public void SetImage(Sprite newImage)
    {
        icon.sprite = newImage;
    }

    public void Hide()
    {
        ImageViewController.instance.Hide();
    }
}
