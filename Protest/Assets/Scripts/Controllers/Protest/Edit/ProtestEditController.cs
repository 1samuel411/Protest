using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeadMosquito.AndroidGoodies;
using DeadMosquito.IosGoodies;

using ImageAndVideoPicker;
using System.Linq;

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
        imageChanged = false;
        model = new ProtestModel(0, "", "", "", "", "", "", "", 00f, 0.0f, null, null, null, null, Authentication.userIndex, 0.0f, 0.0f, true);
        model.contributionModels = new ContributionsModel[0];
        creating = true;
        returnController.Hide();
        _returnController = returnController;
        Show();
        _view.Reset();
    }

    public void Show(int index, Controller returnController)
    {
        imageChanged = false;
        SpinnerController.instance.Show();
        _returnController = returnController;
        DataParser.GetProtest(index, GetProtestCallback);
        PopulateList();
        _view.Reset();
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
        _returnController.Hide();
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

    private bool imageChanged = false;
    public void Complete()
    {
        SpinnerController.instance.Show();
        if (imageChanged)
        {
            Texture2D texture = new Texture2D(128, 128);
            texture.SetPixels(_view.iconImage.sprite.texture.GetPixels((int)_view.iconImage.sprite.textureRect.x, (int)_view.iconImage.sprite.textureRect.y, (int)_view.iconImage.sprite.textureRect.width, (int)_view.iconImage.sprite.textureRect.height));
            texture.Apply();
            DataParser.UploadImage(texture, UploadImageCallback);
        }
        else
        {
            CompleteFinal();
        }
    }

    public void UploadImageCallback(string url)
    {
        model.protestPicture = url;
        CompleteFinal();
    }

    void AddContributionCreationCallback(ContributionsModel newContribution)
    {
        if (i >= model.contributionModels.Length - 1)
        {
            CompleteCreateFinal();
            return;
        }
        i++;
        model.contributionModels[i].protest = model.index;
        DataParser.CreateContribution(model.contributionModels[i], AddContributionCreationCallback);
    }

    int i = 0;
    void CompleteCreate()
    {
        if (model.contributionModels.Length > 0)
        {
            i = 0;
            model.contributionModels[i].protest = model.index;
            DataParser.CreateContribution(model.contributionModels[i], AddContributionCreationCallback);
        }
        else
            CompleteCreateFinal();
    }

    void CompleteCreateFinal()
    {
        Debug.Log("Completed: " + model.index);
        SpinnerController.instance.Hide();
        Log.Create(2, "Complete Protest", "ProtestEditController");
        ProtestController.instance.Show(model.index, ProtestListController.instance);
        ProtestListController.instance.Load(Input.location.lastData.latitude, Input.location.lastData.longitude, null);
        Hide();
        return;
    }

    public void CompleteFinal()
    {
        if (!creating)
            DataParser.EditProtest(model, Authentication.auth_token, CompleteCallback);
        else
        {
            DataParser.CreateProtest(model, Authentication.auth_token, CompleteCallback);
        }
    }

    public void CompleteCallback(int protestIndex)
    {
        model.index = protestIndex;
        CompleteCreate();
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

            ProtestController.instance.Hide();
            ProtestListController.instance.Load(Input.location.lastData.latitude, Input.location.lastData.longitude, null);
        }
    }

    public void RemoveContribution(ContributionsModel model)
    {
        _model = model;
        Popup.Create("Are you sure?", "Deleting this contribution will remove it forever, are you sure you want to delete it?", PopupCallback, "Popup", "Yes", "No");
    }
    private ContributionsModel _model;

    void PopupCallback(int response)
    {
        if(response == 1)
        {
            PoolManager.instance.SetPath(3);

            if (creating)
            {
                this.model.contributionModels = this.model.contributionModels.Except(new ContributionsModel[] { _model }).ToArray();
                DeleteContribution(_model, PoolManager.instance.currentSystem);
            }
            else
            {
                SpinnerController.instance.Show();
                DataParser.DeleteContribution(_model.index, RemoveContributionCallback);
            }
        }
    }

    public void RemoveContributionCallback(int model)
    {
        SpinnerController.instance.Hide();
        PoolManager.instance.SetPath(3);
        DeleteContribution(model, PoolManager.instance.currentSystem);
    }

    // -------------------------------------------------------------------------------------
    public void DeleteContribution(ContributionsModel model, PoolSystem system)
    {
        for (int i = 0; i < system.poolObjects.Count; i++)
        {
            ContributionsListsObjectView x = system.poolObjects[i].GetComponent<ContributionsListsObjectView>();
            if (x.model.name == model.name && x.model.currentAmount == model.currentAmount && x.model.amountNeeded == model.amountNeeded)
            {
                x.GetComponent<PoolObject>().Hide();
                return;
            }
        }
    }

    public void DeleteContribution(int model, PoolSystem system)
    {
        for (int i = 0; i < system.poolObjects.Count; i++)
        {
            ContributionsListsObjectView x = system.poolObjects[i].GetComponent<ContributionsListsObjectView>();
            if (x.model.index == model)
            {
                x.GetComponent<PoolObject>().Hide();
                return;
            }
        }
    }
    // --------------------------------------------------------------------------------------
    public void CreateContribution(ContributionsModel model)
    {
        model.protest = this.model.index;
        PoolManager.instance.SetPath(3);

        if (this.model.contributionModels.Any(x => x.name == model.name))
        {
            Popup.Create("Duplicate", "There is already a contribution with the provided name", null, "Popup", "Okay");
            return;
        }

        if (String.IsNullOrEmpty(model.name.Trim()))
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be a name filled out!", null, "Popup", "Okay");
            return;
        }
        if (model.amountNeeded <= 0)
        {
            SpinnerController.instance.Hide();
            Popup.Create("Invalid", "There must be an amount filled out!", null, "Popup", "Okay");
            return;
        }

        if (creating)
        {
            this.model.contributionModels = this.model.contributionModels.Union(new ContributionsModel[] { model }).ToArray();
            _obj = PoolManager.instance.Create(_view.listHolder);
            _obj.GetComponent<ContributionsListsObjectView>().ChangeInfo(model, RemoveContribution, true);
        }
        else
        {
            SpinnerController.instance.Show();
            DataParser.CreateContribution(model, CreateContributionCallback);
        } 
    }

    public void CreateContributionCallback(ContributionsModel newModel)
    {
        SpinnerController.instance.Hide();
        PoolManager.instance.SetPath(3);
        _obj = PoolManager.instance.Create(_view.listHolder);
        _obj.GetComponent<ContributionsListsObjectView>().ChangeInfo(newModel, RemoveContribution, true);
    }

    private PoolObject _obj;
    public void PopulateList()
    {
        if (creating)
            return;
        if (model.contributions.Length <= 0)
            return;

        Log.Create(1, "Populating List", "ProtestContributionsController");

        DataParser.GetContributions(model.contributions, GetContributionsCallback);
    }

    void GetContributionsCallback(ContributionsModel[] models)
    {
        this.model.contributionModels = models;
        // Clear
        PoolManager.instance.SetPath(3);
        PoolManager.instance.Clear();

        // Populate List
        for (int i = 0; i < models.Length; i++)
        {
            _obj = PoolManager.instance.Create(_view.listHolder);
            _obj.GetComponent<ContributionsListsObjectView>().ChangeInfo(models[i], RemoveContribution, true);
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
        imageChanged = true;
        Debug.Log("Got Texture");
        TextureScale.Bilinear(texture, 128, 128);
        _view.iconImage.sprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0, 0));
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
