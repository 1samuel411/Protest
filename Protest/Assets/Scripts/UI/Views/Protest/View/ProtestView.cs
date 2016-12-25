using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ProtestView : View
{

    public enum SelectionOptions { Contributions, Going, Info, Chat };
    private SelectionOptions _selection;
    public SelectionOptions selection
    {
        get
        {
            return _selection;
        }
        set
        {
            _selection = value;
            ChangeUI();
        }
    }

    public Button contributionsButton;
    public Button goingButton;
    public Button infoButton;
    public Button chatButton;

    public RectTransform selectionTransform;

    public float positionLeft;
    public float positionRight;
    public float positionCenter;

    public float selectionPositionContributions;
    public float selectionPositionGoing;
    public float selectionPositionInfo;
    public float selectionPositionChat;

    public float speed;

    public Color selectedColor;
    public Color defaultColor;

    private ProtestModel _protestModel;
    public ProtestModel protestModel
    {
        get
        {
            return _protestModel;
        }
        set
        {
            _protestModel = value;
            going = ProtestController.instance.Contains(Authentication.user.index, protestModel.going);
            liked = ProtestController.instance.Contains(Authentication.user.index, protestModel.likes);
        }
    }

    public Text title;
    public Text likesCount;

    public Button likeButton;
    public Button shareButton;
    public Button setGoingButton;

    public Color selectedTopBarColor;

    private bool going;
    private bool liked;

    void Awake()
    {
        selection = SelectionOptions.Info;
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        contributionsButton.image.color = defaultColor;
        goingButton.image.color = defaultColor;
        infoButton.image.color = defaultColor;
        chatButton.image.color = defaultColor;
        switch (selection)
        {
            case SelectionOptions.Contributions:
                contributionsButton.image.color = selectedColor;
                selectionTransform.DOAnchorPosX(selectionPositionContributions, speed, true);
                ProtestContributionsController.instance.view.rectTransform.DOAnchorPosX(positionCenter, speed, true);
                ProtestGoingController.instance.view.rectTransform.DOAnchorPosX(positionRight, speed, true);
                ProtestInfoController.instance.view.rectTransform.DOAnchorPosX(positionRight, speed, true);
                ProtestChatController.instance.view.rectTransform.DOAnchorPosX(positionRight, speed, true);
                break;
            case SelectionOptions.Going:
                goingButton.image.color = selectedColor;
                selectionTransform.DOAnchorPosX(selectionPositionGoing, speed, true);
                ProtestContributionsController.instance.view.rectTransform.DOAnchorPosX(positionLeft, speed, true);
                ProtestGoingController.instance.view.rectTransform.DOAnchorPosX(positionCenter, speed, true);
                ProtestInfoController.instance.view.rectTransform.DOAnchorPosX(positionRight, speed, true);
                ProtestChatController.instance.view.rectTransform.DOAnchorPosX(positionRight, speed, true);
                break;
            case SelectionOptions.Info:
                infoButton.image.color = selectedColor;
                selectionTransform.DOAnchorPosX(selectionPositionInfo, speed, true);
                ProtestContributionsController.instance.view.rectTransform.DOAnchorPosX(positionLeft, speed, true);
                ProtestGoingController.instance.view.rectTransform.DOAnchorPosX(positionLeft, speed, true);
                ProtestInfoController.instance.view.rectTransform.DOAnchorPosX(positionCenter, speed, true);
                ProtestChatController.instance.view.rectTransform.DOAnchorPosX(positionRight, speed, true);
                break;
            case SelectionOptions.Chat:
                chatButton.image.color = selectedColor;
                selectionTransform.DOAnchorPosX(selectionPositionChat, speed, true);
                ProtestContributionsController.instance.view.rectTransform.DOAnchorPosX(positionLeft, speed, true);
                ProtestGoingController.instance.view.rectTransform.DOAnchorPosX(positionLeft, speed, true);
                ProtestInfoController.instance.view.rectTransform.DOAnchorPosX(positionLeft, speed, true);
                ProtestChatController.instance.view.rectTransform.DOAnchorPosX(positionCenter, speed, true);
                break;
        }

        int curIndex = (int)selection;

        if (SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.right)
        {
            if (curIndex > 0)
                curIndex--;
        }

        if (SwipeDetection.instance.swipeDirection == SwipeDetection.SwipeDirections.left)
        {
            if (curIndex < 3)
                curIndex ++;
        }

        if (liked)
            likeButton.image.color = selectedTopBarColor;
        else
            likeButton.image.color = Color.white;

        if (going)
            setGoingButton.image.color = selectedTopBarColor;
        else
            setGoingButton.image.color = Color.white;

        selection = (SelectionOptions)curIndex;

    }

    public void SetSelection(int index)
    {
        selection = (SelectionOptions)index;
    }

    void ChangeUI()
    {
        if (protestModel == null)
            return;

        title.text = (protestModel.name.Length > 9) ? protestModel.name.Substring(0, 12) + "..." : protestModel.name;
        string likesCountString = "";
        if (protestModel.likes.Length >= 1000)
        {
            likesCountString = (protestModel.likes.Length / 1000.0f).ToString() + "k";
        }
        else
            likesCountString = protestModel.likes.Length.ToString();

        if (protestModel.likes.Length >= 1000000)
        {
            likesCountString = (protestModel.likes.Length / 1000000.0f).ToString() + "m";
        }
        
        likesCount.text = likesCountString;
    }

    public void Like()
    {
        ProtestController.instance.Like();
        liked = !liked;
    }

    public void Share()
    {
        ProtestController.instance.Share();
    }

    public void Return()
    {
        ProtestController.instance.Return();
    }

    public void Going()
    {
        ProtestController.instance.Going();
        going = !going;
    }
}