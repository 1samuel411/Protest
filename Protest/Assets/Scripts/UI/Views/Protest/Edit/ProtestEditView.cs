using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProtestEditView : View
{

    public RectTransform identifier;

    public RectTransform infoView;
    public RectTransform contributionsView;

    public Button infoButton;
    public Button contributionButton;

    public float identifierInfo;
    public float contributeView;

    public float leftPosition;
    public float centerPosition;
    public float rightPosition;

    public float speed;

    public enum Selection { Contributions, Info }
    private Selection selection;

    private int curIndex = 1;

    public Color colorSelected;

    public InputField nameInput;
    public InputField bodyInput;
    public InputField locationInput;
    public Text locationText;
    public Button dateButton;
    public Text dateText;

    public Button completeButton;

    public Image iconImage;

    public InputField donationsGoalInput;
    public InputField paypalEmailInput;

    public InputField contributionNameInput;
    public InputField contributionAmountInput;

    public RectTransform listHolder;

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        infoButton.image.color = Color.white;
        contributionButton.image.color = Color.white;

        if (selection == Selection.Info)
        {
            infoButton.image.color = colorSelected;
            infoView.DOAnchorPosX(centerPosition, speed, true);
            contributionsView.DOAnchorPosX(leftPosition, speed, true);
            identifier.DOAnchorPosX(identifierInfo, speed, true);
        }
        else if (selection == Selection.Contributions)
        {
            contributionButton.image.color = colorSelected;
            infoView.DOAnchorPosX(rightPosition, speed, true);
            contributionsView.DOAnchorPosX(centerPosition, speed, true);
            identifier.DOAnchorPosX(contributeView, speed, true);
        }

        if (SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.right)
        {
            if (curIndex > 0)
                curIndex--;
        }

        if (SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.left)
        {
            if (curIndex < 3)
                curIndex++;
        }
        selection = (Selection)curIndex;

        ProtestEditController.instance.model.name = nameInput.text;
        ProtestEditController.instance.model.description = bodyInput.text;
        ProtestEditController.instance.model.donationsEmail = paypalEmailInput.text;
        ProtestEditController.instance.model.location = locationInput.text;

        if (donationsGoalInput.text != "")
            ProtestEditController.instance.model.donationTarget = float.Parse(donationsGoalInput.text);

        if (nameInput.text == "" || bodyInput.text == "" || ProtestEditController.instance.model.protestPicture == "" || ProtestEditController.instance.model.date == "" || ProtestEditController.instance.model.location == "")
        {
            completeButton.interactable = false;
        }
        else
            completeButton.interactable = true;

        if(ProtestEditController.instance.model.date == "")
        {
            dateText.text = "Set Date";
        }
        else
        {
            System.DateTime newTime = DataParser.ParseDate(ProtestEditController.instance.model.date);
            dateText.text = newTime.ToShortDateString() + "\n" + newTime.ToShortTimeString();
        }

        if (ProtestEditController.instance.model.location == "")
        {
            locationText.text = "Set Location";
        }
        else
        {
            locationText.text = ProtestEditController.instance.model.location;
        }
    }

    public void ChangeUI()
    {
        nameInput.text = ProtestEditController.instance.model.name;
        bodyInput.text = ProtestEditController.instance.model.description;
        paypalEmailInput.text = ProtestEditController.instance.model.donationsEmail;
        donationsGoalInput.text = ProtestEditController.instance.model.donationTarget.ToString();
        locationInput.text = ProtestEditController.instance.model.location;
    }

    public void SetSelection(int index)
    {
        curIndex = index;
    }

    public void Delete()
    {
        ProtestEditController.instance.Delete();
    }

    public void Confirm()
    {
        ProtestEditController.instance.Complete();
    }

    public void Return()
    {
        ProtestEditController.instance.Return();
    }


    public void UpdateIcon()
    {
        ProtestEditController.instance.UpdateIcon();
    }

    public void SetDate()
    {
        ProtestEditController.instance.SetDate();
    }

    public void PaypalInfo()
    {
        Popup.Create("Why?", "Your Paypal email is securely stored so we can send funds donated to your account.\nCurrently Protest takes 0% of donations.", null);
    }

    public void CreateContribution()
    {
        if (contributionNameInput.text == "" || contributionAmountInput.text == "")
            return; 
        ContributionsModel model = new ContributionsModel(-1, contributionNameInput.text, int.Parse(contributionAmountInput.text), 0, ProtestEditController.instance.model.index);
        ProtestEditController.instance.CreateContribution(model);
    }

}