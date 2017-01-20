using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/**
 * Purpose: Resize the entire chat box based on the ChatResizes.
**/

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class ChatBoxGrid : Base
{

    public RectTransform scrollView;

    public float modifier;
    public float modifierObj;
    public float scrollModifier;
    public float spacingX;

    public float[] _childrenSize;
    public float[] childrenSize
    {
        get
        {
            ChatResize[] x = GetComponentsInChildren<ChatResize>();
            _childrenSize = new float[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                _childrenSize[i] = x[i].GetComponent<RectTransform>().rect.height;
            }

            return _childrenSize;
        }
        set
        {
            _childrenSize = value;
        }
    }

    void Start()
    {
        UpdateVariables();
    }

    void Update()
    {
        UpdateVariables();
    }

    float previousSize = 0;
    float newSize = 0;
    void UpdateVariables()
    {
        previousSize = 0;
        for (int i = 0; i < childrenSize.Length; i++)
        {
            previousSize += childrenSize[i];
            newSize = (i * spacingX) - previousSize;

            transform.GetChild(i).transform.localPosition = new Vector2(0, newSize);
        }

        float height = childrenSize.Sum() - scrollModifier - (spacingX * childrenSize.Length);
        scrollView.sizeDelta = new Vector2(scrollView.sizeDelta.x, height);
    }
}
