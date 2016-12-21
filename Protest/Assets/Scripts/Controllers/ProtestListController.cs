using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestListController : Controller
{

    public new static ProtestListController instance;
    private ProtestListView _view;

    void Awake()
    {
        instance = this;
        _view = view.GetComponent<ProtestListView>();

        PopulateList();
    }

    public void Load(Action<int> callback)
    {
        int response = 0;
        Log.Create(1, "Loading Info", "ProtestController");

        PopulateList();

        callback.Invoke(response);
        Show();
    }

    public void PopulateList()
    {
        Log.Create(1, "Populating List", "ProtestController");
        // Get info

        ProtestModel[] data = new ProtestModel[2];
        data[0] = new ProtestModel(0, "https://blog.eu.playstation.com/files/avatars/avatar_4502445.jpg", "Our Portest Name", null, "426 NW 1st Ave Cape Coral", "16.11.12.1.30.0", null, null, null, 0, new UserModel[2], new UserModel[1], null, null);
        data[1] = new ProtestModel(1, "https://blog.eu.playstation.com/files/avatars/avatar_4502445.jpg", "Our Portest Namesss", null, "426 NW 1st Ave Cape Coral", "16.11.12.1.30.0", null, null, null, 0, new UserModel[2], new UserModel[1], null, null);

        // Clear
        PoolManager.instance.SetPath(0);
        PoolManager.instance.Clear();

        // Populate List
        for (int i = 0; i < data.Length; i++)
        {
            PoolManager.instance.Create(_view.listHolder);
        }
    }
}
