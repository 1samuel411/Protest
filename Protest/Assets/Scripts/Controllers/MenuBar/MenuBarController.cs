using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBarController : Controller
{

    public new static MenuBarController instance;
    private MenuBarView _view;

    void Awake()
    {
        instance = this;
        _view = view.GetComponent<MenuBarView>();
    }

    public void UpdateProfile()
    {
        _view.ChangeInfo(Authentication.userIndex);
    }
}
