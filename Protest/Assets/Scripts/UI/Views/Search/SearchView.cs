using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchView : View
{

    public RectTransform listHolder;

    public Color selectedColor;
    public Image peopleImage;
    public Image protestsImage;

    public Text pageText;

    public Button pageBackButton;
    public Button pageForwardButton;

    public enum SearchSelection { Users, Protests };
    private SearchSelection _selection;
    public SearchSelection selection
    {
        get
        {
            return _selection;
        }
        set
        {
            if(_selection != value)
            {
                _selection = value;
                SearchController.instance.PopulateFromServer();
            }
            _selection = value;
        }
    }

    void Start()
    {
        SearchController.instance.PopulateFromServer();
    }

    void Update()
    {
        peopleImage.color = (selection == SearchSelection.Users) ? selectedColor : Color.white;
        protestsImage.color = (selection == SearchSelection.Protests) ? selectedColor : Color.white;
    }

    public void SelectProtests()
    {
        selection = SearchSelection.Protests;
        SearchController.instance.PopulateFromServer();
    }

    public void SelectUsers()
    {
        selection = SearchSelection.Users;
    }

    public void Return()
    {
        SearchController.instance.Return();
    }

    public void PageBack()
    {
        SearchController.instance.PageBack();
    }

    public void PageForward()
    {
        SearchController.instance.PageForward();
    }

    public void ChangeSearch(string input)
    {
        if (SearchController.instance.searchString != input)
        {
            SearchController.instance.searchString = input;
            SearchController.instance.PopulateFromServer();
            Debug.Log("Changed: " + input);
        }
    }
}
