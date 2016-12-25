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

    public void ChangeInfo(ChatModel model)
    {
        bool ours = model.userPosted == Authentication.user.index;

        identifierOursTransform.gameObject.SetActive(ours);
        identifierTransform.gameObject.SetActive(!ours);
        bgImage.color = (ours) ? ourColor : otherColor;

        dateText.text = (DataParser.ParseDate(model.datePosted)).ToShortDateString() + (DataParser.ParseDate(model.datePosted)).ToShortTimeString();

        UserModel userModel = DataParser.GetUser(model.userPosted);
        nameText.text = userModel.name;

        if(!ours)
        {
            DataParser.SetSprite(iconImage, userModel.name);
        }

        imageHolder.gameObject.SetActive(!ours);
        imageHolderBorder.gameObject.SetActive(!ours);
    }

    public RectTransform GetListHolder()
    {
        return listHolder;
    }

    public void AddBody(string body)
    {
        PoolManager.instance.SetPath(5);
        PoolObject _obj = PoolManager.instance.Create(listHolder);
        _obj.GetComponent<BodyObjectView>().ChangeInfo(body);
    }
}
