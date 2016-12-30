using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingView : View
{

    public CanvasGroup loginGroup;

    public Text quoteText;

    public float fadeSpeed;

    public bool loading;

    void Start()
    {
        QuoteManager.Reset();
        quoteText.text = QuoteManager.GetQuote();
    }

    void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        loginGroup.DOFade((loading ? 0 : 1), fadeSpeed / 2);
        quoteText.DOFade((loading ? 1 : 0), fadeSpeed);
    }

    public void LoginFacebook()
    {
        Log.Create(0, "Logging into facebook...", "LoadingView");
        LoadingController.instance.LoginFacebook();
    }

    public void LoginGoogle()
    {
        Log.Create(0, "Logging into google...", "LoadingView");
        LoadingController.instance.LoginGoogle();
    }
}
