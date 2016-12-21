using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Purpose: Resize the chat to fit with the text
**/

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class ChatResize : Base
{

    public float modifier;

    public float newPosModifier = 0;

    private float _childrenHeightSum;
    public float childrenHeightSum
    {
        get
        {
            _childrenHeightSum = rectTransform.GetComponentsInChildren<RectTransform>().Sum(t => t.rect.height) - rectTransform.rect.height;
            return _childrenHeightSum;
        }
        set
        {
            _childrenHeightSum = value;
        }
    }

	void Start ()
    {
        UpdateVariables();

    }

#if UNITY_EDITOR
    void Update ()
    {
        UpdateVariables();

    }
#endif

    void UpdateVariables()
    {
        if(rectTransform.childCount <= 0)
        {
            return;
        }
        Vector2 newPos = rectTransform.localPosition;
        newPos.y = newPosModifier - ((childrenHeightSum/rectTransform.childCount) * rectTransform.childCount) / 2 + modifier - (rectTransform.childCount * 2.5f);
        rectTransform.localPosition = newPos;
    }
}
