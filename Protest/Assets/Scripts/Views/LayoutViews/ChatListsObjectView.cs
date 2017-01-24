using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatListsObjectView : View
{

    public Text nameText;
    public Text dateText;

    public RectTransform listHolder;

    public Image iconImage;
    public RectTransform imageHolder;
    public RectTransform imageHolderBorder;

    public Image bgImage;

    public Color ourColor;
    public Color otherColor;

    public RectTransform identifierTransform;
    public RectTransform identifierOursTransform;

    public int userIndex;

    public void ChangeInfo(ChatModel model)
    {
        bool ours = model.user == Authentication.userIndex;

        userIndex = model.user;

        identifierOursTransform.gameObject.SetActive(ours);
        identifierTransform.gameObject.SetActive(!ours);
        bgImage.color = (ours) ? ourColor : otherColor;

        DateTime nowTime = DataParser.ParseDate(model.time).ToLocalTime();
        dateText.text = nowTime.ToShortDateString() + " " + nowTime.ToString("h:m tt");

        nameText.text = model.name;

        if(!ours)
        {
            DataParser.SetSprite(model.picture, GetSpriteIcon);
        }

        imageHolder.gameObject.SetActive(!ours);
        imageHolderBorder.gameObject.SetActive(!ours);
    }

    void GetSpriteIcon(Sprite sprite)
    {
        iconImage.sprite = sprite;
    }

    public RectTransform GetListHolder()
    {
        return listHolder;
    }

    public PoolObject AddBody(string body)
    {
        PoolManager.instance.SetPath(5);
        PoolObject _obj = PoolManager.instance.Create(listHolder);
        _obj.GetComponent<BodyObjectView>().ChangeInfo(body);
        if(ProtestChatController.instance.firstRun)
            _obj.transform.SetAsFirstSibling();

        return _obj;
    }

    public void OpenProfile()
    {
        ProfileViewController.instance.Show(userIndex, ProtestController.instance);
        ProtestController.instance.Hide();
    }
}
