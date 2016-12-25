using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContributionsListsObjectView : View
{

    public Text nameText;

    public Text progress;

    public Button AddButton;
    public Button RemoveButton;

    public void ChangeInfo(int id, Action<int> addContribution, Action<int> removeContribution)
    {
        bool ours = ProtestController.instance.ourProtest;
        ContributionsModel model = DataParser.FindContribution(id);

        nameText.text = model.name;

        AddButton.gameObject.SetActive(!ours);
        RemoveButton.gameObject.SetActive(!ours);

        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(() => { addContribution(id); });

        RemoveButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.AddListener(() => { removeContribution(id); });

        progress.text = model.currentAmount + "/" + model.amountNeeded;
    }

    public void ChangeInfo(int id, Action<int> deleteContribution)
    {
        ContributionsModel model = DataParser.FindContribution(id);

        nameText.text = model.name;

        AddButton.gameObject.SetActive(false);

        RemoveButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.AddListener(() => { deleteContribution(id); });

        progress.text = model.currentAmount + "/" + model.amountNeeded;
    }
}
