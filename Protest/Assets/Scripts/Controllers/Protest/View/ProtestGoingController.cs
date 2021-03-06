﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestGoingController : Controller
{

    public new static ProtestGoingController instance;

    private ProtestGoingView _view;

    private UserModel[] usersData;

    private int _listIndex;
    public int listIndex
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
        SpinnerController.instance.Show();
        listIndex = 1;

        Log.Create(1, "Populating from server", "ProtestGoingController");

        _view.searchInputField.text = "";
        _view.searchInput = "";

        PopulateList();
    }

    public void GetUsersCallback(UserModel[] userModels)
    {
        _beginIndex = 0;
        _endIndex = 0;

        _pageLength = (userModels.Length / _pageSize) + 1;

        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = userModels.Length;
        else
            _endIndex = _pageSize * listIndex;

        _beginIndex = listIndex * _pageSize;
        _beginIndex -= _pageSize;

        // Update Button
        _view.pageForwardButton.interactable = (listIndex <= _pageLength - 1);
        _view.pageBackButton.interactable = (listIndex > 1);

        if (userModels.Length <= 0)
        {
            PoolManager.instance.SetPath(1);
            PoolManager.instance.Clear();

            SpinnerController.instance.Hide();
            return;
        }
        usersData = userModels;
        
        DataParser.GetAtlas(usersData.Skip(_beginIndex).Take(_endIndex).Select(x => x.profilePicture).ToArray(), PopulateWithAtlas);
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
        _beginIndex = 0;
        _endIndex = 0;

        _pageLength = (ProtestController.instance.GetModel().going.Length / _pageSize) + 1;

        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = ProtestController.instance.GetModel().going.Length;
        else
            _endIndex = _pageSize * listIndex;

        _beginIndex = listIndex * _pageSize;
        _beginIndex -= _pageSize;

        Log.Create(1, "Populating List", "ProtestGoingController");
        
        // Get Atlas for protests: _beginIndex to _endIndex
        SpinnerController.instance.Show();
        DataParser.GetUsers(ProtestController.instance.GetModel().going.Skip(_beginIndex).Take(_endIndex).ToArray(), _view.searchInput, GetUsersCallback);
    }

    private void PopulateWithAtlas(Texture2D _atlas)
    {
        PoolManager.instance.SetPath(1);
        PoolManager.instance.Clear();

        SpinnerController.instance.Hide();

        _rect.position = new Vector2(0, _atlas.height - _rect.height);

        // Populate List
        for (int i = _beginIndex; i < _endIndex; i++)
        {
            if(usersData[i] == null)
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

    private UserModel _user;
    public void AddUser(UserModel user)
    {
        _user = user;
        PoolManager.instance.SetPath(1);
        DataParser.SetSprite(user.profilePicture, CallbackGetUserSprite, 128);
    }

    void CallbackGetUserSprite(Sprite sprite)
    {
        _obj = PoolManager.instance.Create(_view.listHolder);
        _obj.transform.SetSiblingIndex(0);
        _obj.GetComponent<SearchListObjectView>().ChangeInfoUser(_user, sprite, SelectUser);
    }

    public void RemoveUser(int user)
    {
        for (int i = 0; i < _view.listHolder.childCount; i++)
        {
            if (_view.listHolder.GetChild(i).GetComponent<SearchListObjectView>().user.index == user)
            {
                _view.listHolder.GetChild(i).GetComponent<PoolObject>().Hide();
                return;
            }
        }
    }

    public void SelectUser(int user)
    {
        Log.Create(1, "Pressed User", "ProtestGoingController");
        ProfileViewController.instance.Show(user, ProtestController.instance);
    }
}
