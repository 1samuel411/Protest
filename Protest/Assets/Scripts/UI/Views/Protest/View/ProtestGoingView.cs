using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtestGoingView : View
{
    public Text pageText;

    public Button pageBackButton;
    public Button pageForwardButton;

    public RectTransform listHolder;

    public void PageBack()
    {
        ProtestGoingController.instance.PageBack();
    }

    public void PageForward()
    {
        ProtestGoingController.instance.PageForward();
    }
}
