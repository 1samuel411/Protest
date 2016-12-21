using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ProtestListView : View
{

    public RectTransform menuBar, list;

    public float positionViewList;
    public float positionHideList;
    public float positionViewMenubar;
    public float positionHideMenubar;

    public float moveSpeed;

    public bool viewMenuBar;

    void Update()
    {
        UpdateUI();

        if(SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.right)
            ShowMenu();

        if (viewMenuBar && SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.left)
            HideMenu();
    }

    private void UpdateUI()
    {
        menuBar.DOAnchorPosX((viewMenuBar ? positionViewMenubar : positionHideMenubar), moveSpeed, true);
        list.DOAnchorPosX((viewMenuBar ? positionHideList : positionViewList), moveSpeed, true);
    }

    public void ToggleMenu()
    {
        viewMenuBar = !viewMenuBar;
    }

    public void ShowMenu()
    {
        viewMenuBar = true;
    }

    public void HideMenu()
    {
        viewMenuBar = false;
    }
}
