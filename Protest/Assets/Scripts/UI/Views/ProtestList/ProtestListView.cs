using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProtestListView : View
{

    public RectTransform menuBar, list;

    public Text pageText;

    public Button pageBackButton;
    public Button pageForwardButton;

    public float positionViewList;
    public float positionHideList;
    public float positionViewMenubar;
    public float positionHideMenubar;

    public float moveSpeed;

    public bool viewMenuBar;

    public RectTransform listHolder;

    public GameObject notificationIcon;
    public Text notificationText;

    public GameObject notificationMenubarIcon;
    public Text notificationMenubarText;

    void Start()
    {
        MenuBarController.instance.UpdateProfile();
        ProtestListController.instance.PopulateFromServer();
        InvokeRepeating("LoadNotifications", 0, 10);
    }

    void LoadNotifications()
    {
        Debug.Log("Updating notifications!");
        ProtestListController.instance.LoadNotifications();
    }

    void Update()
    {
        UpdateUI();

        if (SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.right)
            ShowMenu();

        if (viewMenuBar && SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.left)
            HideMenu();


        if (listHolder.anchoredPosition.y <= 0 && SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.down)
            ProtestListController.instance.PopulateList();
    }

    private void UpdateUI()
    {
        menuBar.DOAnchorPosX((viewMenuBar ? positionViewMenubar : positionHideMenubar), moveSpeed, true);
        list.DOAnchorPosX((viewMenuBar ? positionHideList : positionViewList), moveSpeed, true);
    }

    public void ToggleMenu()
    {
        viewMenuBar = !viewMenuBar;

        if (viewMenuBar)
            MenuBarController.instance.UpdateProfile();
    }

    public void PageBack()
    {
        ProtestListController.instance.PageBack();
    }

    public void PageForward()
    {
        ProtestListController.instance.PageForward();
    }

    public void ShowMenu()
    {
        MenuBarController.instance.UpdateProfile();
        viewMenuBar = true;
    }

    public void HideMenu()
    {
        viewMenuBar = false;
    }

    public void OpenSearch()
    {
        ProtestListController.instance.OpenSearch();
    }

    public void CreateProtest()
    {
        ProtestListController.instance.CreateProtest();
    }

    public void Contact()
    {
        ProtestListController.instance.Contact();
    }

    public void ShareApp()
    {
        ProtestListController.instance.Share();
    }
}
