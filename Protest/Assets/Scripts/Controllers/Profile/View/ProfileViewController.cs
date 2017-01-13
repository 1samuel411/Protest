using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileViewController : Controller
{

    public new static ProfileViewController instance;
    private ProfileView _view;

    void Awake()
    {
        instance = this;
        _view = view.GetComponent<ProfileView>();
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

    public void Return()
    {
        Hide();
        _previousController.Show();
    }

    private Controller _previousController;
    public void Show(int userModel, Controller previousController)
    {
        SpinnerController.instance.Show();
        _previousController = previousController;
        DataParser.GetUser(userModel, GetUserCallback);
    }

    public void GetUserCallback(UserModel user)
    {
        SpinnerController.instance.Hide();
        _previousController.Hide();
        view.gameObject.SetActive(true);
        _view.userModel = user;
    }

    public void EditProfile(UserModel user)
    {
        Log.Create(1, "Opening Edit Profile", "ProfileViewController");
        ProfileEditController.instance.Show(_view.userModel, this);
    }

    private UserModel _reportUser;
    public void ReportProfile(UserModel user)
    {
        Log.Create(1, "Opening Report Profile", "ProfileViewController");
        _reportUser = user;
        Popup.Create("Report User", "", CallbackReport, "Popup", "Abusive Language", "Inappropriate Content", "Harrasment", "Other");
    }

    void CallbackReport(int response)
    {
        if(response != 0)
            SpinnerController.instance.Show();

        if (response == 1)
        {
            Log.Create(1, "Abusive Language Report Sent", "ProfileViewController");
            DataParser.SendReportUser(_reportUser.index, "Language", SendReportCallback);
        }
        else if (response == 2)
        {
            Log.Create(1, "Content Report Sent", "ProfileViewController");
            DataParser.SendReportUser(_reportUser.index, "Content", SendReportCallback);
        }
        else if (response == 3)
        {
            Log.Create(1, "Harassment Report Sent", "ProfileViewController");
            DataParser.SendReportUser(_reportUser.index, "Harassment", SendReportCallback);
        }
        else if (response == 4)
        {
            DataParser.SendReportUser(_reportUser.index, "Other", SendReportCallback);
            Log.Create(1, "Other Report Sent", "ProfileViewController");
        }
    }

    void SendReportCallback()
    {
        SpinnerController.instance.Hide();
        Popup.Create("Report Sent", "Your report will be reviewed, Thank you for your submission", null, "Popup", "Okay");
    }

    PoolObject _obj;
    private UserModel userModel;
    public void PopulateProtests(UserModel user)
    {
        userModel = user;
        PoolManager.instance.SetPath(2);
        PoolManager.instance.Clear();

        for (int i = 0; i < user.protestsAttended.Length; i++)
        {
            if (i > 16)
                return;

            if (user.protestsAttended.Length <= 0)
                return;

            _obj = PoolManager.instance.Create(_view.protestsHolder);
            ProtestIconObjectView view = _obj.GetComponent<ProtestIconObjectView>();
            view.ChangeInfo(user.protestsAttended[i], OpenProtest);
        }

        for (int i = 0; i < user.protestsCreated.Length; i++)
        {
            if (i > 16)
                return;

            if (user.protestsCreated.Length <= 0)
                return;

            _obj = PoolManager.instance.Create(_view.protestsHolder);
            ProtestIconObjectView view = _obj.GetComponent<ProtestIconObjectView>();
            view.ChangeInfo(user.protestsCreated[i], OpenProtest);
        }
    }

    public void OpenProtest(int protest)
    {
        Log.Create(1, "Opening Protest", "ProfileViewController");
        ProtestController.instance.Show(protest, this);
    }

    public void OpenFollowers()
    {
        Log.Create(1, "Opening Followers", "ProfileViewController");
        ListController.instance.Show(ListController.ShowType.followers, userModel, ProtestListController.instance);
    }

    public void OpenFollowing()
    {
        Log.Create(1, "Opening Following", "ProfileViewController");
        ListController.instance.Show(ListController.ShowType.following, userModel, ProtestListController.instance);
    }

    public void OpenProtestsCreated()
    {
        Log.Create(1, "Opening Protests", "ProfileViewController");
        ListController.instance.Show(ListController.ShowType.created, userModel, ProtestListController.instance);
    }

    public void OpenAttended()
    {
        Log.Create(1, "Opening Attended", "ProfileViewController");
        ListController.instance.Show(ListController.ShowType.attended, userModel, ProtestListController.instance);
    }

    public void OpenSnapchat(string url)
    {
        string openUrl = ("http://www.snapchat.com/add/" + url);
        Application.OpenURL(openUrl);
        //InAppBrowser.OpenURL(openUrl);
    }

    public void OpenFacebook(string url)
    {
        string openUrl = ("http://www.facebook.com/" + url);
        Application.OpenURL(openUrl);
        //InAppBrowser.OpenURL(openUrl);
    }

    public void OpenInstagram(string url)
    {
        string openUrl = ("https://www.instagram.com/" + url);
        Application.OpenURL(openUrl);
        //InAppBrowser.OpenURL(openUrl);
    }

    public void OpenTwitter(string url)
    {
        string openUrl = ("https://twitter.com/" + url);
        Application.OpenURL(openUrl);
        //InAppBrowser.OpenURL(openUrl);
    }

    public void Follow(int index)
    {
        SpinnerController.instance.Show();
        DataParser.Follow(index, FollowCallback);
    }

    void FollowCallback(bool following)
    {
        Authentication.RefreshUserModel(RefreshCallback);
    }

    void RefreshCallback()
    {
        DataParser.GetUser(_view.userModel.index, RefreshProfileViewCallback);
    }

    void RefreshProfileViewCallback(UserModel user)
    {
        SpinnerController.instance.Hide();
        _view.userModel = user;
    }
}
