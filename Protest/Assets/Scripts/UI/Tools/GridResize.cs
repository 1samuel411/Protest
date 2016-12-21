using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Purpose: Re-size the grid layout group for multiple screen widths.
**/

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class GridResize : Base
{

    public enum Axis { xy, x };
    public Axis axis;
    public float size;
    private GridLayoutGroup _layoutGroup;
    private GridLayoutGroup layoutGroup
    {
        get
        {
            if(!_layoutGroup)
            {
                _layoutGroup = GetComponent<GridLayoutGroup>();
            }
            return _layoutGroup;
        }
        set
        {
            _layoutGroup = value;
        }
    }

    void Start()
    {
        UpdateVariables();
    }

#if UNITY_EDITOR
    void Update()
    {
        UpdateVariables();
    }
#endif

    void UpdateVariables()
    {
        float sizeVector = size * ((float)Screen.width/Screen.height);
        layoutGroup.cellSize = new Vector2(sizeVector, (axis == Axis.xy) ? sizeVector : layoutGroup.cellSize.y);
    }
}
