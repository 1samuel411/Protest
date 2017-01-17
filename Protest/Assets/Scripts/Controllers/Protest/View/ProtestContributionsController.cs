using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestContributionsController : Controller
{

    public new static ProtestContributionsController instance;

    private ProtestContributionsView _view;

    public ContributionsModel[] contributionsData;

    void Awake()
    {
        _view = view.GetComponent<ProtestContributionsView>();
        instance = this;
    }

    public void PopulateFromServer()
    {
        _view.donationAmount = "0.0";
        _view.ChangeUI();
        if (ProtestController.instance.ourProtest || ProtestController.instance.GetModel().donationsEmail == "")
            _view.moneyView.SetActive(false);

        DataParser.GetContributions(ProtestController.instance.GetModel().contributions, PopulateList);
    }

    private PoolObject _obj;
    public void PopulateList(ContributionsModel[] contributionsData)
    {
        this.contributionsData = contributionsData;
        if (contributionsData.Length <= 0)
        {
            return;
        }

        Log.Create(1, "Populating List", "ProtestContributionsController");

        // Clear
        PoolManager.instance.SetPath(3);
        PoolManager.instance.Clear();

        // Populate List
        for (int i = 0; i < contributionsData.Length; i++)
        {
            if (contributionsData[i] == null)
                return;

            _obj = PoolManager.instance.Create(_view.listHolder);
            _obj.GetComponent<ContributionsListsObjectView>().ChangeInfo(contributionsData[i], Interact, false);
        }
    }

    public void Interact(ContributionsModel model)
    {
        SpinnerController.instance.Show();
        Log.Create(1, "Adding Contribution", "ProtestContributionsController");
        DataParser.AddContribution(model.index, AddCallback);
    }

    void AddCallback()
    {
        SpinnerController.instance.Hide();
    }

    public void Donate(float amount)
    {
        if (amount <= 0)
            return;

        Log.Create(2, "Opening Paypal checkout", "ProtestContributionsController");

        Popup.Create("In Development", "Currently Paypal checkout has yet to be implemented, please give us some time. Thank you.", null, "Popup", "Okay");
    }
}
