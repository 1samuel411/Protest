﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProtestContributionsView : View
{
    public RectTransform listHolder;

    public GameObject moneyView;

    public InputField donationField;

    public Text moneyRaisedText;

    private string _donationAmount;
    public string donationAmount
    {
        get
        {
            return _donationAmount;
        }
        set
        {
            _donationAmount = value;
            if (_donationAmount == "")
                _donationAmount = "0.0";

            donationField.text = donationAmount;
        }
    }

    public void ChangeUI()
    {
        moneyRaisedText.text = "$" + ProtestController.instance.GetModel().donationCurrent.ToString() + "/" + ProtestController.instance.GetModel().donationTarget.ToString();
    }

    public void ChangeDonationAmount(string input)
    {
        donationAmount = input;
    }

    public void AddDonation(int amount)
    {
        donationAmount = (float.Parse(donationAmount) + amount).ToString();
    }

    public void DonationSubmit()
    {
        ProtestContributionsController.instance.Donate(float.Parse(donationAmount));
    }
}