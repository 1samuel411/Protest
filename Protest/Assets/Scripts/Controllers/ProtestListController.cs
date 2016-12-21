using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestListController : Controller
{

    public new static ProtestListController instance;

    void Awake()
    {
        instance = this;
    }

    public void Load(Action<int> callback)
    {
        int response = 0;
        Log.Create(1, "Loading Info", "ProtestController");

        // Load from database

        callback.Invoke(response);
        Show();
    }
}
