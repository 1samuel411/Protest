using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtestInfoController : Controller
{

    public new static ProtestInfoController instance;

    private ProtestInfoView _view;

    void Awake()
    {
        _view = view.GetComponent<ProtestInfoView>();
        instance = this;
    }

    public Image GetViewIcon()
    {
        return _view.protestImage;
    }

    public new void Show()
    {
        _view.ChangeUI(ProtestController.instance.GetModel());
    }

    private ProtestModel _reportProtest;
    public void ReportProtest(ProtestModel protest)
    {
        Log.Create(1, "Opening Report Profile", "ProtestViewController");
        _reportProtest = protest;
        Popup.Create("Report Protest", "", CallbackReport, "Popup", "Abusive Language", "Inappropriate Content", "Spamming", "Other");
    }

    void CallbackReport(int response)
    {
        if (response == 1)
        {
            Log.Create(1, "Abusive Language Report Sent", "ProtestViewController");
            DataParser.SendReportProtest(_reportProtest.index, "Language", CallbackReportProtest);
        }
        else if (response == 2)
        {
            Log.Create(1, "Content Report Sent", "ProtestViewController");
            DataParser.SendReportProtest(_reportProtest.index, "Content", CallbackReportProtest);
        }
        else if (response == 3)
        {
            Log.Create(1, "Spamming Report Sent", "ProtestViewController");
            DataParser.SendReportProtest(_reportProtest.index, "Spamming", CallbackReportProtest);
        }
        else if (response == 4)
        {
            Log.Create(1, "Other Report Sent", "ProtestViewController");
            DataParser.SendReportProtest(_reportProtest.index, "Other", CallbackReportProtest);
        }
        if (response != 0)
        {
            SpinnerController.instance.Show();
        }
    }

    void CallbackReportProtest()
    {
        SpinnerController.instance.Hide();
        Popup.Create("Report Sent", "Your report will be reviewed, Thank you for your submission", null, "Popup", "Okay");
    }

    public void ViewProtestImage()
    {
        ImageViewController.instance.Show(_view.protestImage.sprite, this);
    }

    public void AddToCalender()
    {
        ProtestController.instance.AddToCalender();
    }
}
