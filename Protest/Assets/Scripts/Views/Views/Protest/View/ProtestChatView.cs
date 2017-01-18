using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProtestChatView : View
{
    public RectTransform listHolder;
    public ChatInputField chatInput;

    public void ChangeUI()
    {
        chatInput.text = "";

        chatInput.onSubmit_.RemoveAllListeners();
        chatInput.onSubmit_.AddListener(SendChat);
    }

    public void SendChat(string text)
    {
        if (ProtestChatController.instance.canChat == false)
            return;

        if (chatInput.text.Length <= 0)
            return;

        ProtestChatController.instance.SendChat(chatInput.text);
        chatInput.text = "";
    }
}