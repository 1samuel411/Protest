﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using DeadMosquito.AndroidGoodies;
#endif
#if UNITY_IOS
using DeadMosquito.IosGoodies;
#endif
using System.Linq;

public class ProtestController : Controller
{

    private ProtestView _view;
    public static new ProtestController instance;
    public bool ourProtest;

    void Awake()
    {
        _view = view.GetComponent<ProtestView>();
        instance = this;
    }

    void Update()
    {
        if (!view.gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }

    bool IsOurs()
    {
        if (_view.protestModel == null)
            return false;
        bool belongs = (_view.protestModel.userCreated == Authentication.userIndex);
        return belongs;
    }

    public ProtestModel GetModel()
    {
        return _view.protestModel;
    }

    public void Show(int protestModel, Controller previousController)
    {
        _view.likesCountInt = 0;
        _previousController = previousController;
        previousController.Hide();

        Show();

        SpinnerController.instance.Show();
        Log.Create(1, "Opening Protest View", "ProtestController");
        DataParser.GetProtest(protestModel, GetProtestCallback);
    }

    void GetProtestCallback(ProtestModel model)
    {
        SpinnerController.instance.Hide();

        _view.protestModel = model;

        ourProtest = IsOurs();
        _view.selection = ProtestView.SelectionOptions.Info;
        ProtestInfoController.instance.Show();
        ProtestGoingController.instance.PopulateFromServer();
        ProtestContributionsController.instance.PopulateFromServer();
        ProtestChatController.instance.PopulateFromServer();
        ProtestChatController.instance.ready = true;
    }

    private Controller _previousController;
    public void Return()
    {
        Hide();
        ProtestChatController.instance.ready = false;
        _previousController.Show();
    }

    public void ViewLocation(string location)
    {
        Log.Create(0, "Opening Location View", "ProtestController");
#if UNITY_IOS
        string address = _view.protestModel.location;
            IGMaps.OpenMapAddress(address, "_view.protestModel.name", IGMaps.MapViewType.Hybrid);
#endif
#if UNITY_ANDROID
        AGMaps.OpenMapLocation(_view.protestModel.location);
#endif
    }

    public void Share()
    {
        Texture2D texture = new Texture2D(128, 128);
        var pixels = ProtestInfoController.instance.GetViewIcon().sprite.texture.GetPixels((int)ProtestInfoController.instance.GetViewIcon().sprite.textureRect.x,
                                         (int)ProtestInfoController.instance.GetViewIcon().sprite.textureRect.y,
                                         (int)ProtestInfoController.instance.GetViewIcon().sprite.textureRect.width,
                                         (int)ProtestInfoController.instance.GetViewIcon().sprite.textureRect.height);
        texture.SetPixels(pixels);
        texture.Apply();
#if UNITY_IOS
        IGShare.Share(() =>
            Debug.Log("Share completed"), "Check out this Protest!\n" + _view.protestModel.name +"\n" + DataParser.ParseDate(_view.protestModel.date).ToString() + "\n" + _view.protestModel.location + "\n" + _view.protestModel.description, texture);
#endif
#if UNITY_ANDROID
        AGShare.ShareTextWithImage("Protest", "Check out this Protest!\n" + _view.protestModel.name +"\n" + DataParser.ParseDate(_view.protestModel.date).ToString() + "\n" + _view.protestModel.location + "\n" + _view.protestModel.description, texture);
#endif
    }

    public void Going()
    {
        SpinnerController.instance.Show();
        DataParser.GoingProtest(_view.protestModel.index, GoingCallback);
    }

    public void GoingCallback(bool going)
    {
        SpinnerController.instance.Hide();
        _view.going = going;

        if(going)
        {
            ProtestGoingController.instance.AddUser(Authentication.userModel);
        }
        else
            ProtestGoingController.instance.RemoveUser(Authentication.userIndex);
    }

    public void Like()
    {
        SpinnerController.instance.Show();
        DataParser.LikeProtest(_view.protestModel.index, LikeCallback);
    }

    void LikeCallback(bool liked)
    {
        _view.likesCountInt += (liked) ? 1 : -1;
        SpinnerController.instance.Hide();
        _view.liked = liked;
    }

    public bool Contains(int a, int[] list)
    {
        for(int i = 0; i < list.Length; i++)
        {
            if (list[i] == a)
                return true;
        }
        return false;
    }

    public void AddToCalender()
    {
#if UNITY_ANDROID
        var beginTime = DataParser.ParseDate(_view.protestModel.date);
        var endTime = beginTime.AddHours(2);
        var eventBuilder = new AGCalendar.EventBuilder(_view.protestModel.name, beginTime);
        eventBuilder.SetEndTime(endTime);
        eventBuilder.SetIsAllDay(false);
        eventBuilder.SetLocation(_view.protestModel.location);
        eventBuilder.SetAccessLevel(AGCalendar.EventAccessLevel.Public);
        eventBuilder.SetAvailability(AGCalendar.EventAvailability.Free);
        eventBuilder.BuildAndShow();
#endif
#if UNITY_IOS

#endif
    }
}
