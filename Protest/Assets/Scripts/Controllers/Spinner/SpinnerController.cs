using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerController : Controller
{

    public static new SpinnerController instance;
    private new Animation animation;

    private float timeOutTime = 12.0f;
    private float curTimeOutTime;

    public void Awake()
    {
        animation = view.GetComponentInChildren<Animation>();
        instance = this;
    }

    public new void Show()
    {
        curTimeOutTime = Time.time + timeOutTime;
        animation.Play();
        view.gameObject.SetActive(true);
    }

    public new void Hide()
    {
        animation.Stop();
        view.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Time.time >= curTimeOutTime && view.gameObject.activeInHierarchy)
        {
            Hide();
            Popup.Create("Timed out", "The request has timed out, please try again or contact us, please include a brief description of what you were doing to recieve this error.", null, "Popup", "Ok");
        }
    }
}
