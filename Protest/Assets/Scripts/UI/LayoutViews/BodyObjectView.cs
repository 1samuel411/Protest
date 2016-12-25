using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyObjectView : View
{

    public Text bodyText;

    public void ChangeInfo(string text)
    {
        bodyText.text = text;
    }
}
