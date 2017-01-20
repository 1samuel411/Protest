using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : Base
{

    public bool inUse;

    public Vector2 size;
    public Vector2 sizeRect;
    public Vector3 rotation;

    public bool hasChildren;
    public Transform childrenHolder;

    void Awake()
    {
        this.size = transform.localScale;
        this.sizeRect = rectTransform.sizeDelta;
        //this.rotation = transform.localEulerAngles;
        Hide();
    }
    
    public void Create(Transform parent)
    {
        inUse = true;
        gameObject.SetActive(true);
        transform.SetParent(parent, false);

        transform.localPosition = Vector2.zero;
        transform.localScale = size;
        rectTransform.sizeDelta = sizeRect;
        transform.localEulerAngles = rotation;
    }

    public void Hide()
    {
        if(hasChildren)
        {
            for(int i = 0; i < childrenHolder.childCount; i++)
            {
                PoolObject poolObject = childrenHolder.GetChild(i).GetComponent<PoolObject>();
                if(poolObject)
                {
                    poolObject.Hide();
                }
            }
        }
        inUse = false;
        gameObject.SetActive(false);
        transform.SetParent(null);
    }

}
