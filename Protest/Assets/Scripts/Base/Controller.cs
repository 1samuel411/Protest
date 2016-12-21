using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : Base
{

    public static Controller instance;

    public View view;

    void Awake()
    {
        instance = this;
    }

    public void Show()
    {
        view.gameObject.SetActive(true);
    }

    public void Hide()
    {
        view.gameObject.SetActive(false);
    }
}
