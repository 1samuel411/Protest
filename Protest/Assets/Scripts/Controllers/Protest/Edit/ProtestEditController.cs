﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeadMosquito.AndroidGoodies;
using DeadMosquito.IosGoodies;

using ImageAndVideoPicker;

public class ProtestEditController : Controller
{

    public new static ProtestEditController instance;
    private ProtestEditView _view;

    public ProtestModel model;

    [HideInInspector]
    public bool creating;

    void Awake()
    {
        _view = view.GetComponent<ProtestEditView>();
        instance = this;
    }

    void Update()
    {
        if (!view.gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ProtestListController.instance.Show();
            Hide();
        }
    }

    private Controller _returnController;
    public void Show(Controller returnController)
    {
        model = new ProtestModel(0, "", "", "", "", "", "", "", 00f, 0.0f, null, null, null, null, Authentication.userIndex, 0.0f, 0.0f, true);
        creating = true;
        _returnController = returnController;
        Show();
    }

    public void Show(int index, Controller returnController)
    {
        SpinnerController.instance.Show();
        _returnController = returnController;
        DataParser.GetProtest(index, GetProtestCallback);
    }

    void GetProtestCallback(ProtestModel model)
    {
        SpinnerController.instance.Hide();
        this.model = model;
        if (!String.IsNullOrEmpty(model.protestPicture))
        {
            DataParser.SetSprite(_view.iconImage, model.protestPicture);
        }
        _view.ChangeUI();
        creating = false;
        Show();
    }

    public void Return()
    {
        Hide();
        if (_returnController == null)
            ProtestListController.instance.Show();
        else
            _returnController.Show();
    }

    public void Complete()
    {
        SpinnerController.instance.Show();

        if (!creating)
            DataParser.EditProtest(model, Authentication.auth_token, CompleteCallback);
        else
            DataParser.CreateProtest(model, Authentication.auth_token, CompleteCallback);
    }

    public void CompleteCallback(int protestIndex)
    {
        Debug.Log("Completed: " + protestIndex);
        SpinnerController.instance.Hide();
        Log.Create(2, "Complete Protest", "ProtestEditController");
        ProtestController.instance.Show(protestIndex, ProtestListController.instance);
        Hide();
    }

    public void Delete()
    {
        if(creating)
        {
            Return();
            return;
        }
        Popup.Create("Delete", "Are you sure you want to delete this protest? To undo this action, contact us.", DeleteCallback, "Popup", "Yes", "No");
    }

    void DeleteCallback(int response)
    {
        if (response == 1)
        {
            Log.Create(2, "Deleting Protest", "ProtestEditController");
            if (creating == false)
                DataParser.DeleteProtest(model.index);

            Return();
        }
    }

    public void RemoveContribution(int index)
    {
        DataParser.DeleteContribution(index);
        int[] contributers = new int[this.model.contributions.Length - 1];
        for (int i = 0; i < contributers.Length; i++)
        {
            if (contributers[i] != index)
                contributers[i] = this.model.contributions[i];
        }
        this.model.contributions = contributers;

        PopulateList();
    }

    public void CreateContribution(ContributionsModel model)
    {
        model.protest = this.model.index;

        model = DataParser.CreateContribution(model);
        int[] contributers = new int[this.model.contributions.Length + 1];
        for (int i = 0; i < contributers.Length - 1; i++)
        {
            contributers[i] = this.model.contributions[i];
        }
        contributers[contributers.Length - 1] = model.index;
        this.model.contributions = contributers;

        PopulateList();
    }

    private PoolObject _obj;
    public void PopulateList()
    {
        if (model.contributions.Length <= 0)
        {
            return;
        }

        Log.Create(1, "Populating List", "ProtestContributionsController");

        // Clear
        PoolManager.instance.SetPath(3);
        PoolManager.instance.Clear();

        // Populate List
        for (int i = 0; i < model.contributions.Length; i++)
        {
            _obj = PoolManager.instance.Create(_view.listHolder);
            _obj.GetComponent<ContributionsListsObjectView>().ChangeInfo(model.contributions[i], RemoveContribution, true);
        }
    }

    public void UpdateIcon()
    {
        DataParser.ChangeIcon(GetIconCallback);
#if UNITY_EDITOR
        model.protestPicture = "http://gallery.raccoonfink.com/d/7003-2/alien-head-128x128.png";
#endif
    }

    void GetIconCallback(Texture2D texture)
    {
        Debug.Log("Got Texture");
        texture.Resize(128, 128);
        _view.iconImage.sprite = Sprite.Create(texture, new Rect(new Vector2(0, 0), new Vector2(128, 128)), new Vector2(0, 0));
        DataParser.UploadImage(texture, UploadImageCallback);
    }

    public void UploadImageCallback(string url)
    {
        model.protestPicture = url;
    }

    string _date;
    public void SetDate()
    {
#if UNITY_ANDROID
        OnPickDateClick();
#endif
#if UNITY_IOS
        OnShowDateAndTimePickerIos();
#endif
#if UNITY_EDITOR
        model.date = DataParser.UnparseDate(DateTime.UtcNow);
#endif
    }

#if UNITY_IOS

    public void OnShowDateAndTimePickerIos()
    {
        IGDateTimePicker.ShowDateAndTimePicker(OnDateAndTimeTimeSelectedIos,
            () => Debug.Log("Picking date and time was cancelled"));
    }

    void OnDateAndTimeTimeSelectedIos(DateTime dateTime)
    {
        Debug.Log(string.Format("Date & Time selected: year: {0}, month: {1}, day {2}, hour: {3}, minute: {4}",
                dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute));
        if (dateTime < DateTime.Today)
        {
            IGDialogs.ShowOneBtnDialog("Not valid", "Enter a valid date", "Okay", () => Debug.Log("Button clicked!"));

            return;
        }
        model.date = DataParser.UnparseDate(dateTime);
    }
#endif

#if UNITY_ANDROID
    public void OnPickDateClick()
    {
        var now = DateTime.Now;
        AGDateTimePicker.ShowDatePicker(now.Year, now.Month, now.Day, OnDatePicked, OnDatePickCancel);
    }

    private void OnDatePicked(int year, int month, int day)
    {
        var picked = new DateTime(year, month, day);
        if (picked < DateTime.Today)
        {
            AGUIMisc.ShowToast("Enter a valid date");
            return;
        }
        _date = DataParser.UnparseDate(picked);
        OnTimePickClick();
    }

    private void OnDatePickCancel()
    {
        Debug.Log("Canceled");
    }

    public void OnTimePickClick()
    {
        var now = DateTime.Now;
        AGDateTimePicker.ShowTimePicker(now.Hour, now.Minute, OnTimePicked, OnTimePickCancel);
    }

    private void OnTimePicked(int hourOfDay, int minute)
    {
        var date = DataParser.ParseDate(_date);
        var picked = new DateTime(date.Year, date.Month, date.Day, hourOfDay, minute, 00);
        model.date = DataParser.UnparseDate(picked);

    }

    private void OnTimePickCancel()
    {
        Debug.Log("Canceled");
    }
#endif

    public void ChangeLocation(string newLocation)
    {
        SpinnerController.instance.Show();
        DataParser.GetLocation(newLocation, ChangeLocationCallback);
    }

    void ChangeLocationCallback(string address, float lat, float lng)
    {
        SpinnerController.instance.Hide();
        model.x = lat;
        model.y = lng;
        model.location = address;
        _view.locationInput.text = address;
    }
}
