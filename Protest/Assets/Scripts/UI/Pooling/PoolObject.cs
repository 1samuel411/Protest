using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : Base
{

    public bool inUse;

    void Awake()
    {
        Hide();
    }
    
    public void Create(Transform parent)
    {
        inUse = true;
        gameObject.SetActive(true);
        transform.SetParent(parent, false);
        transform.localPosition = Vector2.zero;
    }

    public void Hide()
    {
        inUse = false;
        gameObject.SetActive(false);
    }

}
