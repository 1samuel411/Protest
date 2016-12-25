﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListView : View
{

    public RectTransform listHolder;

    public Text pageLabel;

    public Text pageText;

    public Button pageForwardButton;
    public Button pageBackButton;

    public void UpdateUI(ListController.ShowType showType)
    {
        if (showType == ListController.ShowType.attended)
            pageLabel.text = "Attended";
        if (showType == ListController.ShowType.created)
            pageLabel.text = "Created";
        if (showType == ListController.ShowType.followers)
            pageLabel.text = "Followers";
        if (showType == ListController.ShowType.following)
            pageLabel.text = "Following";
    }


    public void ForwardButton()
    {
        ListController.instance.PageForward();
    }

    public void BackwardButton()
    {
        ListController.instance.PageBack();
    }

    public void Return()
    {
        ListController.instance.Hide();
    }
}