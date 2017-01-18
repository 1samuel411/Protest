using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtestInfoView : View
{

    public Button reportButton;

    public Button userButton;
    public Text userText;
    public Button editButton;

    public Image protestImage;

    public Text nameText;
    public Button locationButton;
    public Text locationText;
    public Text dateText;

    public Text bodyText;

    public void ChangeUI(ProtestModel model)
    {
        reportButton.gameObject.SetActive(!ProtestController.instance.ourProtest);
        reportButton.onClick.RemoveAllListeners();
        reportButton.onClick.AddListener(() => { ProtestInfoController.instance.ReportProtest(model); });

        userButton.gameObject.SetActive(!ProtestController.instance.ourProtest);
        userButton.onClick.RemoveAllListeners();
        userButton.onClick.AddListener(() => { ProfileViewController.instance.Show(model.userCreated, ProtestController.instance); });

        editButton.gameObject.SetActive(ProtestController.instance.ourProtest);
        editButton.onClick.RemoveAllListeners();
        editButton.onClick.AddListener(() => { ProtestEditController.instance.Show(model.index, ProtestController.instance); });

        DataParser.SetSprite(protestImage, model.protestPicture);

        nameText.text = model.name;

        locationButton.onClick.RemoveAllListeners();
        locationButton.onClick.AddListener(() => { ProtestController.instance.ViewLocation(model.location); });

        locationText.text = model.location;
        if (model.date != "")
        {
            System.DateTime newTime = DataParser.ParseDate(model.date).ToLocalTime();
            dateText.text = newTime.DayOfWeek + ", " + newTime.ToShortDateString() + "\n" + newTime.ToString("hh:mm tt");
        }
        else
            dateText.text = "Set Date";

        bodyText.text = model.description.Replace("\\n", "\n");

        if (!model.active)
        {
            editButton.gameObject.SetActive(false);
        }

        int[] me = new int[1];
        me[0] = model.userCreated;
        DataParser.GetUsers(me, "", Callback);
    }

    void Callback(UserModel[] users)
    {
        userText.text = users[0].name;
    }

    public void ViewProtestImage()
    {
        ProtestInfoController.instance.ViewProtestImage();
    }

    public void AddToCalender()
    {
        ProtestInfoController.instance.AddToCalender();
    }
}
