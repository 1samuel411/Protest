using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageViewController : Controller
{

    public static new ImageViewController instance;
    [HideInInspector]
    public ImageView _view;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(_view.gameObject.activeInHierarchy)
                Hide();
        }
    }

    public void Awake()
    {
        _view = view.GetComponent<ImageView>();
        instance = this;
    }

    private Controller previousController;
    public void Show(Sprite spriteToShow, Controller previousController)
    {
        this.previousController = previousController;
        previousController.Hide();
        base.Show();
        _view.SetImage(spriteToShow);
    }

    public new void Hide()
    {
        previousController.Show();
        view.gameObject.SetActive(false);
    }
}
