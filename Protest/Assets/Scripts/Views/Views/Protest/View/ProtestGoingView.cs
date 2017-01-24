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

    public string searchInput;

    public InputField searchInputField;

    public void PageBack()
    {
        ProtestGoingController.instance.PageBack();
    }

    public void PageForward()
    {
        ProtestGoingController.instance.PageForward();
    }

    public void SearchInput(string newInput)
    {
        if(newInput != searchInput)
        {
            searchInput = newInput;
            ProtestGoingController.instance.listIndex = 1;
        }
    }
}
