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

    public string searchString;

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

    public enum ShowType { followers, following, attended, created };
    private ShowType showType;
    private UserModel model;

    public void Show(ShowType showType, int index)
    {
        SpinnerController.instance.Show();
        this.showType = showType;
        DataParser.GetUser(index, GetUserCallback);
    }

    void GetUserCallback(UserModel userModel)
    {
        SpinnerController.instance.Hide();
        model = userModel;
        Show();
        PopulateFromServer();
        _view.UpdateUI(showType);
    }

    public void Return()
    {
        ProtestListController.instance.Load(ReturnCallback);
    }

    private void ReturnCallback(int response)
    {
        if (response == 0)
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
        _beginIndex = 0;
        _endIndex = 0;

        // Clear
        PoolManager.instance.SetPath(1);
        if (showType == ShowType.attended || showType == ShowType.created)
            PoolManager.instance.SetPath(0);
        PoolManager.instance.Clear();

        protestsData = new ProtestModel[0];
        usersData = new UserModel[0];
        listIndex = 1;

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

    public void GetUsers()
    {
        
    }

    public void GetUsersCallback(UserModel[] userModels)
    {
        if (userModels.Length <= 0)
        {
            SpinnerController.instance.Hide();
            return;
        }
        usersData = userModels;
        
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
        PoolManager.instance.Clear();
     
        Log.Create(1, "Populating List", "ListController");
        
        if (_pageLength <= 0 || listIndex >= _pageLength)
            _endIndex = (showType == ShowType.attended || showType == ShowType.created ? (showType == ShowType.created ? model.protestsCreated.Length : model.protestsAttended.Length) : (showType == ShowType.followers ? model.followers.Length : model.following.Length));
        else
            _endIndex = _pageSize * listIndex;

        _beginIndex = listIndex * _pageSize;
        _beginIndex -= _pageSize;

        // Update Button
        _view.pageForwardButton.interactable = (listIndex <= _pageLength - 1);
        _view.pageBackButton.interactable = (listIndex > 1);

        SpinnerController.instance.Show();

        if (showType == ShowType.attended)
            protestsData = DataParser.GetProtests(model.protestsAttended);
        else if (showType == ShowType.created)
            protestsData = DataParser.GetProtests(model.protestsAttended);
        else if (showType == ShowType.followers)
            DataParser.GetUsers(model.followers.Skip(_beginIndex).Take(_endIndex).ToArray(), GetUsersCallback);
        else if (showType == ShowType.following)
            DataParser.GetUsers(model.following.Skip(_beginIndex).Take(_endIndex).ToArray(), GetUsersCallback);


        // Get Atlas for protests: _beginIndex to _endIndex
        SpinnerController.instance.Show();
    }

    private void PopulateWithAtlas(Texture2D _atlas)
    {
        SpinnerController.instance.Hide();

        // Clear
        PoolManager.instance.SetPath(1);
        if (showType == ShowType.attended || showType == ShowType.created)
            PoolManager.instance.SetPath(0);
        PoolManager.instance.Clear();

        _rect.position = new Vector2(0, _atlas.height - _rect.height);

        // Populate List
        for (int i = _beginIndex; i < _endIndex; i++)
        {
            _obj = PoolManager.instance.Create(_view.listHolder);
            _sprite = Sprite.Create(_atlas, _rect, Vector2.zero);

            if (showType == ShowType.attended || showType == ShowType.created)
                _obj.GetComponent<ProtestListObjectView>().ChangeInfo(protestsData[i], _sprite, SelectProtest);
            else
                _obj.GetComponent<SearchListObjectView>().ChangeInfoUser(usersData[i], _sprite, SelectUser);

            _rect.x += _rect.width;
            if (_rect.x >= (_atlas.width))
            {
                _rect.y -= _rect.height;
                _rect.x = 0;
            }
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
