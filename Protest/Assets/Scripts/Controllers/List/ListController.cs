using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : Controller
{

    public new static ListController instance;
    private ListView _view;

    private ProtestModel[] protestsData;
    private UserModel[] usersData;

    private int _listIndex;
    private int listIndex
    {
        get
        {
            return _listIndex;
        }
        set
        {
            _listIndex = value;
            PopulateList();
            _view.listHolder.anchoredPosition = new Vector2(_view.listHolder.anchoredPosition.x, 0);
            _view.pageText.text = _listIndex.ToString();
        }
    }

    private string _searchString;
    public string searchString
    {
        get
        {
            return _searchString;
        }
        set
        {
            _searchString = value;
            if (model != null)
                if (showType == ShowType.news)
                {
                    SpinnerController.instance.Show();
                    DataParser.GetNews((model.following), searchString, GetNewsCallback);
                }
                else
                    PopulateList(); 
        }
    }

    public void UpdateSearch(string input)
    {
        searchString = input;
    }

    void Update()
    {
        if (!view.gameObject.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
    }

    void Awake()
    {
        instance = this;
        _view = view.GetComponent<ListView>();
    }

    public enum ShowType { followers, following, attended, created, news };
    private ShowType showType;
    private UserModel model;
    private Controller previousController;
    public void Show(ShowType showType, UserModel model, Controller previousController)
    {
        this.previousController = previousController;
        _view.searchInput.text = "";
        searchString = "";
        this.model = model;
        SpinnerController.instance.Show();
        this.showType = showType;
        previousController.Hide();
        Show();
        if (showType == ShowType.news)
        {
            if (model.following.Length <= 0)
                Return();
            DataParser.GetNews((model.following), searchString, GetNewsCallback);
        }
        else
        {
            PopulateFromServer();
        }
        _view.UpdateUI(showType);
    }

    private NewsModel[] newsModels;

    void GetNewsCallback(NewsModel[] models)
    {
        _listIndex = 1;
        _beginIndex = 0;
        _endIndex = 0;

        newsModels = models;

        _pageLength = (newsModels.Length / _pageSize) + 1;

        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = newsModels.Length;
        else
            _endIndex = _pageSize * listIndex;

        _beginIndex = listIndex * _pageSize;
        _beginIndex -= _pageSize;

        if (models.Length <= 0)
        {
            SpinnerController.instance.Hide();
            return;
        }
        DataParser.GetAtlas(newsModels.Skip(_beginIndex).Take(_endIndex).Select(x => x.picture).ToArray(), PopulateWithAtlas);
    }

    public void Return()
    {
        model = null;
        newsModels = null;
        previousController.Show();
        Debug.Log("Returning to: " + previousController.name);
        Hide();
    }

    public void PageBack()
    {
        if (listIndex > 1)
            listIndex--;
    }

    public void PageForward()
    {
        if (listIndex <= _pageLength - 1)
            listIndex++;
    }

    public int GetPage()
    {
        return listIndex;
    }

    public void PopulateFromServer()
    {
        listIndex = 1;

        _beginIndex = 0;
        _endIndex = 0;

        // Clear
        PoolManager.instance.SetPath(1);
        if (showType == ShowType.attended || showType == ShowType.created)
            PoolManager.instance.SetPath(0);
        if (showType == ShowType.news)
            PoolManager.instance.SetPath(1);
        PoolManager.instance.Clear();

        protestsData = new ProtestModel[0];
        usersData = new UserModel[0];
        
        Log.Create(1, "Populating from server", "ListController");

        if (showType == ShowType.attended)
            _pageLength = (model.protestsAttended.Length / _pageSize) + 1;
        else if (showType == ShowType.created)
            _pageLength = (model.protestsCreated.Length / _pageSize) + 1;
        else if (showType == ShowType.followers)
            _pageLength = (model.followers.Length / _pageSize) + 1;
        else if (showType == ShowType.following)
            _pageLength = (model.following.Length / _pageSize) + 1;
        
        PopulateList();
    }
    
    public void GetUsersCallback(UserModel[] userModels)
    {
        usersData = userModels;

        if (userModels.Length <= 0)
        {
            SpinnerController.instance.Hide();
            return;
        }

        _pageLength = (usersData.Length / _pageSize) + 1;

        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = userModels.Length;
        else
            _endIndex = _pageSize * listIndex;

        _beginIndex = listIndex * _pageSize;
        _beginIndex -= _pageSize;

        DataParser.GetAtlas(usersData.Skip(_beginIndex).Take(_endIndex).Select(x => x.profilePicture).ToArray(), PopulateWithAtlas);
    }

    public Texture2D _atlas;
    private Sprite _sprite;
    private PoolObject _obj;
    private Rect _rect = new Rect(0, 0, 128, 128);
    private int _beginIndex;
    private int _endIndex;
    private int _pageLength;
    private int _pageSize = 32;
    public void PopulateList()
    {
        // Clear
        PoolManager.instance.SetPath(1);
        if (showType == ShowType.attended || showType == ShowType.created)
            PoolManager.instance.SetPath(0);
        if(showType == ShowType.news)
            PoolManager.instance.SetPath(1);

        PoolManager.instance.Clear();
     
        Log.Create(1, "Populating List", "ListController");
        
        
        // Update Button
        _view.pageForwardButton.interactable = (listIndex <= _pageLength - 1);
        _view.pageBackButton.interactable = (listIndex > 1);

        SpinnerController.instance.Show();

        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = showType == ShowType.news ? newsModels.Length : (showType == ShowType.attended || showType == ShowType.created ? (showType == ShowType.created ? model.protestsCreated.Length : model.protestsAttended.Length) : (showType == ShowType.followers ? model.followers.Length : model.following.Length));
        else
            _endIndex = _pageSize * listIndex;

        _beginIndex = listIndex * _pageSize;
        _beginIndex -= _pageSize;

        if (showType == ShowType.attended)
            protestsData = DataParser.GetProtests(model.protestsAttended);
        else if (showType == ShowType.created)
            protestsData = DataParser.GetProtests(model.protestsAttended);
        else if (showType == ShowType.followers)
            DataParser.GetUsers(model.followers.Skip(_beginIndex).Take(_endIndex).ToArray(), searchString, GetUsersCallback);
        else if (showType == ShowType.following)
            DataParser.GetUsers(model.following.Skip(_beginIndex).Take(_endIndex).ToArray(), searchString, GetUsersCallback);
        else if (showType == ShowType.news)
            DataParser.GetAtlas(newsModels.Skip(_beginIndex).Take(_endIndex).Select(x => x.picture).ToArray(), PopulateWithAtlas);
    }

    private void PopulateWithAtlas(Texture2D _atlas)
    {
        SpinnerController.instance.Hide();

        // Clear
        PoolManager.instance.SetPath(1);
        if (showType == ShowType.attended || showType == ShowType.created)
            PoolManager.instance.SetPath(0);
        if (showType == ShowType.news)
            PoolManager.instance.SetPath(1);
        PoolManager.instance.Clear();

        _rect.position = new Vector2(0, _atlas.height - _rect.height);

        // Populate List
        for (int i = _beginIndex; i < _endIndex; i++)
        {
            _obj = PoolManager.instance.Create(_view.listHolder);
            _sprite = Sprite.Create(_atlas, _rect, Vector2.zero);

            if (showType == ShowType.attended || showType == ShowType.created)
                _obj.GetComponent<ProtestListObjectView>().ChangeInfo(protestsData[i], _sprite, SelectProtest);
            else if (showType == ShowType.followers || showType == ShowType.following)
                _obj.GetComponent<SearchListObjectView>().ChangeInfoUser(usersData[i], _sprite, SelectUser);
            else if (showType == ShowType.news)
                _obj.GetComponent<SearchListObjectView>().ChangeInfoNews(newsModels[i], _sprite, SelectNews);

            _rect.x += _rect.width;
            if (_rect.x >= (_atlas.width))
            {
                _rect.y -= _rect.height;
                _rect.x = 0;
            }
        }
    }

    public void SelectNews(NewsModel newsModel)
    {
        Log.Create(1, "Pressed News", "ListController");
        if(newsModel.type == NewsModel.Type.Follow)
        {
            ProfileViewController.instance.Show(newsModel.targetIndex, this);
        }
        else if(newsModel.type == NewsModel.Type.Protest)
        {
            ProtestController.instance.Show(newsModel.targetIndex, this);
        }
    }

    public void SelectProtest(int protest)
    {
        Log.Create(1, "Pressed Protest", "ListController");
        ProtestController.instance.Show(protest, this);
    }

    public void SelectUser(int user)
    {
        Log.Create(1, "Pressed User", "ListController");
        ProfileViewController.instance.Show(user, this);
    }
}
