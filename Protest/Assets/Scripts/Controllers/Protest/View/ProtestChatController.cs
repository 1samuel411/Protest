using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtestChatController : Controller
{

    public new static ProtestChatController instance;

    private ProtestChatView _view;

    public ChatModel[] chatData;

    void Awake()
    {
        _view = view.GetComponent<ProtestChatView>();
        instance = this;

        InvokeRepeating("RefreshData", 0.8f, 0.8f);
    }

    public void PopulateFromServer()
    {
        _view.ChangeUI();
        chatData = null;
        oldChatData = null;

        RefreshData();

        Log.Create(1, "Populating from server", "ProtestChatsController");
    }

    public void RefreshData()
    {
        if (!ProtestChatController.instance.view.gameObject.activeInHierarchy)
            return;

        //chatData = DataParser.GetChats(ProtestController.instance.GetModel().chats);
        if (oldChatData.Length != chatData.Length)
        {
            oldChatData = chatData;
            PopulateList();
        }
    }

    private ChatModel[] oldChatData;

    private PoolObject _obj;
    private ChatListsObjectView _data;

    private ChatModel[] typedChat;

    public void PopulateList()
    {
        PoolManager.instance.SetPath(4);
        PoolManager.instance.Clear();

        Log.Create(1, "Populating List", "ProtestChatsController");
        
        // Populate List
        for (int i = 0; i < chatData.Length; i++)
        {
            if (chatData[i] == null)
                return;

            if (i > 0)
            {
                if (chatData[i - 1].index == chatData[i].index)
                    return;

                if (chatData[i - 1].userPosted == chatData[i].userPosted)
                {
                    _data.AddBody(chatData[i].body);
                    return;
                }
            }
            
            _obj = PoolManager.instance.Create(_view.listHolder);
            _data = _obj.GetComponent<ChatListsObjectView>();
            _data.ChangeInfo(chatData[i]);
            _data.AddBody(chatData[i].body);
        }
    }
}