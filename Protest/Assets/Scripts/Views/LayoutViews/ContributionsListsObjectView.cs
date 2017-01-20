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

    public ContributionsModel model;

    public void ChangeInfo(ContributionsModel model, Action<ContributionsModel> Interact, bool delete)
    {
        this.model = model;

        nameText.text = model.name;

        AddButton.gameObject.SetActive(!delete);
        RemoveButton.gameObject.SetActive(delete);

        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(() => { Interact(model); Add(); });

        RemoveButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.AddListener(() => { Interact(model); });

        progress.text = model.currentAmount + "/" + model.amountNeeded;

        if(ProtestController.instance.GetModel().active == false)
        {
            AddButton.gameObject.SetActive(true);
            RemoveButton.gameObject.SetActive(false);
            AddButton.interactable = false;
        }
        else
            AddButton.interactable = false;
    }

    public void Add()
    {
        model.currentAmount++;

        progress.text = model.currentAmount + "/" + model.amountNeeded;
    }
}
