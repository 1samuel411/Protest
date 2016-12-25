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
        contributionsData = DataParser.GetContributions(ProtestController.instance.GetModel().contributions);

        Log.Create(1, "Populating from server", "ProtestContributionsController");

        PopulateList();
    }

    private PoolObject _obj;
    public void PopulateList()
    {
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
            _obj.GetComponent<ContributionsListsObjectView>().ChangeInfo(contributionsData[i].index, AddContribution, RemoveContribution);
        }
    }

    public void AddContribution(int id)
    {
        Log.Create(1, "Adding Contribution", "ProtestContributionsController");
        DataParser.AddContribution(id);
    }

    public void RemoveContribution(int id)
    {
        Log.Create(1, "Removing Contribution", "ProtestContributionsController");
        DataParser.RemoveContribution(id);
    }

    public void Donate(float amount)
    {
        if (amount <= 0)
            return;

        Log.Create(2, "Opening Paypal checkout", "ProtestContributionsController");
    }
}
