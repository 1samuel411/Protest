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

    public void ChangeInfo(int id, Action<int> Interact, bool delete)
    {
        ContributionsModel model = DataParser.FindContribution(id);

        nameText.text = model.name;

        AddButton.gameObject.SetActive(!delete);
        RemoveButton.gameObject.SetActive(delete);

        AddButton.onClick.RemoveAllListeners();
        AddButton.onClick.AddListener(() => { Interact(id); });

        RemoveButton.onClick.RemoveAllListeners();
        RemoveButton.onClick.AddListener(() => { Interact(id); });

        progress.text = model.currentAmount + "/" + model.amountNeeded;
    }
}
