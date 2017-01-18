using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        InvokeRepeating("RefreshData", 3.0f, 1.5f);
    }

    public bool canChat = true;
    public void SendChat(string chat)
    {
        if (!canChat)
            return;
        DataParser.SendChat(ProtestController.instance.GetModel().index, chat, SendChatCallback);
        StartCoroutine(ChatCooldown());
        loading = true;
        DataParser.GetChats(ProtestController.instance.GetModel().index, GetChatsCallback);
    }

    void SendChatCallback()
    {
        Debug.Log("Sent chat callback");
    }

    WaitForSeconds seconds = new WaitForSeconds(1.5f);
    IEnumerator ChatCooldown()
    {
        canChat = false;
        yield return seconds;
        canChat = true;
    }

    public bool ready = false;
    public bool loading = false;
    public bool firstRun = true;
    public void PopulateFromServer()
    {
        PoolManager.instance.SetPath(4);
        PoolManager.instance.Clear();

        _view.ChangeUI();
        chatData = null;
        oldChatData = new ChatModel[0];

        firstRun = true;
        RefreshData();

        Log.Create(1, "Populating from server", "ProtestChatsController");
    }

    public void RefreshData()
    {
        if (!ready || loading)
            return;
        if (!ProtestChatController.instance.view.gameObject.activeInHierarchy)
            return;

        loading = true;
        DataParser.GetChats(ProtestController.instance.GetModel().index, GetChatsCallback);
    }

    HashSet<int> oldChatHash;
    void GetChatsCallback(ChatModel[] models)
    {
        loading = false;
        Debug.Log("Chats gotten: " + models.Length);
        oldChatHash = new HashSet<int>(oldChatData.Select(x => x.index));
        chatData = models.Where(x => !oldChatHash.Contains(x.index)).ToArray();
        oldChatData = models;
        PopulateList();
    }

    public ChatModel[] oldChatData;

    private PoolObject _obj;
    private ChatListsObjectView _data;

    private ChatModel[] typedChat;

    public void PopulateList()
    {
        if (chatData == null)
            return;
        PoolManager.instance.SetPath(4);

        Log.Create(1, "Populating List", "ProtestChatsController");
        
        // Populate List
        for (int i = 0; i < chatData.Length; i++)
        {
            PoolManager.instance.SetPath(4);
            
            if(firstRun)
            {
                if(i > 0)
                {
                    if(chatData[i].user == oldChatData[i - 1].user)
                    {
                        _data.AddBody(chatData[i].body);
                        continue;
                    }
                }
            }
            else
            {
                if (chatData[i].user == oldChatData[chatData.Length - i].user)
                {
                    _data = _view.listHolder.GetChild(i).GetComponent<ChatListsObjectView>();
                    _data.AddBody(chatData[i].body);
                    continue;
                }
            }

            _obj = PoolManager.instance.Create(_view.listHolder);
            if(!firstRun)
                _obj.transform.SetAsFirstSibling();
            _data = _obj.GetComponent<ChatListsObjectView>();
            _data.ChangeInfo(chatData[i]);
            _data.AddBody(chatData[i].body);
        }
        firstRun = false;
    }
}