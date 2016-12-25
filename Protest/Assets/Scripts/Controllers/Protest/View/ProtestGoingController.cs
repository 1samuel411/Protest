﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestGoingController : Controller
{

    public new static ProtestGoingController instance;

    private ProtestGoingView _view;

    private UserModel[] usersData;

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
        _view = view.GetComponent<ProtestGoingView>();
        instance = this;
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
        usersData = DataParser.GetUsers(ProtestController.instance.GetModel().going);
        listIndex = 1;

        Log.Create(1, "Populating from server", "ProtestGoingController");

        _pageLength = (usersData.Length / _pageSize) + 1;
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
        if (usersData.Length <= 0)
        {
            return;
        }

        Log.Create(1, "Populating List", "ProtestGoingController");

        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = usersData.Length;
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
        PoolManager.instance.SetPath(1);
        PoolManager.instance.Clear();

        _rect.position = new Vector2(0, _atlas.height - _rect.height);

        // Populate List
        for (int i = _beginIndex; i < _endIndex; i++)
        {
            if(usersData[i] == null || usersData[i].profilePicture == "")
                return;

            _obj = PoolManager.instance.Create(_view.listHolder);
            _sprite = Sprite.Create(_atlas, _rect, Vector2.zero);

            _obj.GetComponent<SearchListObjectView>().ChangeInfoUser(usersData[i], _sprite, SelectUser);

            _rect.x += _rect.width;
            if (_rect.x >= (_atlas.width))
            {
                _rect.y -= _rect.height;
                _rect.x = 0;
            }
        }
    }

    public void SelectUser(int user)
    {
        Log.Create(1, "Pressed User", "ProtestGoingController");
        ProfileViewController.instance.Show(user, this);
    }
}