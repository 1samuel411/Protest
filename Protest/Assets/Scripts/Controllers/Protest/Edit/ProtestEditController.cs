using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeadMosquito.AndroidGoodies;
using DeadMosquito.IosGoodies;

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

    private Controller _returnController;
    public void Show(Controller returnController)
    {
        model = new ProtestModel(0, "", "", "", "", "", "", "", "", 00f, 0.0f, null, null, new int[0], null, Authentication.user.index);
        model = DataParser.CreateProtest(model);
        creating = true;
        _returnController = returnController;
        Show();
    }

    public void Show(int index, Controller returnController)
    {
        model = DataParser.GetProtest(index);
        _view.ChangeUI();
        creating = false;
        _returnController = returnController;
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
        if (!creating)
            DataParser.EditProtest(model, Authentication.auth_token);
        else
            DataParser.CreateProtest(model);
        Log.Create(2, "Complete Protest", "ProtestEditController");
    }

    public void Delete()
    {
        Popup.Create("Delete", "Are you sure you want to delete this protest? It cannot be retrieved upon deletion.", DeleteCallback, "Popup", "Yes", "No");
    }

    void DeleteCallback(int response)
    {
        if(response == 1)
        {
            Log.Create(2, "Deleting Protest", "ProtestEditController");
            if(creating == false)
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
            if(contributers[i] != index)
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
        for(int i = 0; i < contributers.Length - 1; i++)
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
            _obj.GetComponent<ContributionsListsObjectView>().ChangeInfo(model.contributions[i], RemoveContribution);
        }
    }

    public void UpdateIcon()
    {

    }

    public void SetLocation()
    {

    }

    string _date;
    public void SetDate()
    {
#if UNITY_ANDROID
        OnPickDateClick();
#endif
    }

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
}
