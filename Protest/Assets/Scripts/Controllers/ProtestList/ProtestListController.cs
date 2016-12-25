﻿using System;
using System.Collections;
using System.Collections.Generic;
using DeadMosquito.AndroidGoodies;
using DeadMosquito.IosGoodies;
using UnityEngine;

public class ProtestListController : Controller
{

    public new static ProtestListController instance;
    private ProtestListView _view;

    private ProtestModel[] protestsData;

    private int _listIndex;
    private int listIndex
    {
        get
        {
            return _listIndex;
        }
        set
        {
            _listIndex = value;
            PopulateList();
            _view.listHolder.anchoredPosition = new Vector2(_view.listHolder.anchoredPosition.x, 0);
            _view.pageText.text = _listIndex.ToString();
        }
    }

    void Awake()
    {
        instance = this;
        _view = view.GetComponent<ProtestListView>();
    }

    void Start()
    {
        
    }

    public void Load(Action<int> callback)
    {
        int response = 0;
        Log.Create(1, "Loading Info", "ProtestController");

        MenuBarController.instance.UpdateProfile();
        PopulateFromServer();

        callback.Invoke(response);
        Show();
    }

    public void PageBack()
    {
        if (listIndex > 1)
            listIndex--;
    }

    public void PageForward()
    {
        if (listIndex <= _pageLength - 1)
            listIndex++;
    }

    public int GetPage()
    {
        return listIndex;
    }

    public void PopulateFromServer()
    {
        protestsData = new ProtestModel[0];
        listIndex = 1;

        Log.Create(1, "Populating from server", "ProtestController");

        protestsData = DataParser.GetProtestList();
        _pageLength = (protestsData.Length / _pageSize) + 1;
        _beginIndex = 0;
        _endIndex = 0;
        PopulateList();
    }

    public Texture2D _atlas;
    private Sprite _sprite;
    private PoolObject _obj;
    private Rect _rect = new Rect(0, 0, 128, 128);
    private int _beginIndex;
    private int _endIndex;
    private int _pageLength;
    private int _pageSize = 32;
    public void PopulateList()
    {
        if (protestsData.Length <= 0)
        {
            return;
        }

        Log.Create(1, "Populating List", "ProtestController");

        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = protestsData.Length;
        else
            _endIndex = _pageSize * listIndex;

        _beginIndex = listIndex * _pageSize;
        _beginIndex -= _pageSize;

        // Update Button
        _view.pageForwardButton.interactable = (listIndex <= _pageLength - 1);
        _view.pageBackButton.interactable = (listIndex > 1);

        // Get Atlas for protests: _beginIndex to _endIndex

        PopulateWithAtlas();
    }

    private void PopulateWithAtlas()
    {
        // Clear
        PoolManager.instance.SetPath(0);
        PoolManager.instance.Clear();

        _rect.position = new Vector2(0, _atlas.height - _rect.height);

        // Populate List
        for (int i = _beginIndex; i < _endIndex; i++)
        {
            _obj = PoolManager.instance.Create(_view.listHolder);
            _sprite = Sprite.Create(_atlas, _rect, Vector2.zero);

            _obj.GetComponent<ProtestListObjectView>().ChangeInfo(protestsData[i], _sprite, SelectProtest);

            _rect.x += _rect.width;
            if (_rect.x >= (_atlas.width))
            {
                _rect.y -= _rect.height;
                _rect.x = 0;
            }
        }
    }

    public void OpenSearch()
    {
        Log.Create(0, "Opening the search view", "ProtestController");
        Hide();
        SearchController.instance.Load();
    }

    public void SelectProtest(int model)
    {
        Log.Create(1, "Clicked on Protest: " + model, "ProtestListController");
        ProtestController.instance.Show(model, this);
    }

    public void CreateProtest()
    {
        Log.Create(2, "Creating Protest", "ProtestListController");
        ProtestEditController.instance.Show(this);
    }

    public void Contact()
    {
#if UNITY_IOS
        var recipients = new[] { "armi.sam99@gmail.com" };
        IGShare.SendEmail(recipients, "Protest Contact - IOS", "Message");
#endif
#if UNITY_ANDROID
        var recipients = new[] { "armi.sam99@gmaik.com" };
        AGShare.SendEmail(recipients, "Protest Contact - Android", "Message");
#endif
    }

    public void Share()
    {
#if UNITY_IOS
        IGShare.Share(() =>
            Debug.Log("Share completed"), "Come join Protest and help change the world! http://protestchange.com/");
#endif
#if UNITY_ANDROID
        AGShare.ShareText("Protest", "Come join Protest and help change the world! http://protestchange.com/");
#endif
    }
}