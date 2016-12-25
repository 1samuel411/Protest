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

    public void Return()
    {
        Hide();
        _previousController.Show();
    }

    private Controller _previousController;
    public void Show(int userModel, Controller previousController)
    {
        _previousController = previousController;
        _previousController.Hide();
        view.gameObject.SetActive(true);
        _view.userModel = DataParser.GetUser(userModel);
    }

    public void EditProfile(UserModel user)
    {
        Log.Create(1, "Opening Edit Profile", "ProfileViewController");
        ProfileEditController.instance.Show(_view.userModel);
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
        if (response == 1)
        {
            Log.Create(1, "Abusive Language Report Sent", "ProfileViewController");
            DataParser.SendReportUser(_reportUser.index, "Language");
        }
        else if (response == 2)
        {
            Log.Create(1, "Content Report Sent", "ProfileViewController");
            DataParser.SendReportUser(_reportUser.index, "Content");
        }
        else if (response == 3)
        {
            Log.Create(1, "Harassment Report Sent", "ProfileViewController");
            DataParser.SendReportUser(_reportUser.index, "Harassment");
        }
        else if (response == 4)
        {
            DataParser.SendReportUser(_reportUser.index, "Other");
            Log.Create(1, "Other Report Sent", "ProfileViewController");
        }
        if (response != 0)
        {
            Popup.Create("Report Sent", "Your report will be reviewed, Thank you for your submission", null, "Popup", "Okay");
        }
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
            if (user.protestsAttended.Length <= 0)
                return;

            _obj = PoolManager.instance.Create(_view.protestsHolder);
            ProtestIconObjectView view = _obj.GetComponent<ProtestIconObjectView>();
            view.ChangeInfo(user.protestsAttended[i], OpenProtest);
        }

        for (int i = 0; i < user.protestsCreated.Length; i++)
        {
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
        ListController.instance.Show(ListController.ShowType.followers, userModel);

    }

    public void OpenFollowing()
    {
        Log.Create(1, "Opening Following", "ProfileViewController");
        ListController.instance.Show(ListController.ShowType.following, userModel);
    }

    public void OpenProtestsCreated()
    {
        Log.Create(1, "Opening Protests", "ProfileViewController");
        ListController.instance.Show(ListController.ShowType.created, userModel);
    }

    public void OpenAttended()
    {
        Log.Create(1, "Opening Attended", "ProfileViewController");
        ListController.instance.Show(ListController.ShowType.attended, userModel);
    }

    public void OpenSnapchat(string url)
    {
        string openUrl = ("http://www.snapchat.com/add/" + url);
#if UNITY_EDITOR
        Application.OpenURL(openUrl);
#endif
        InAppBrowser.OpenURL(openUrl);
    }

    public void OpenFacebook(string url)
    {
        string openUrl = ("http://www.facebook.com/" + url);
#if UNITY_EDITOR
        Application.OpenURL(openUrl);
#endif
        InAppBrowser.OpenURL(openUrl);
    }

    public void OpenInstagram(string url)
    {
        string openUrl = ("https://www.instagram.com/" + url);
#if UNITY_EDITOR
        Application.OpenURL(openUrl);
#endif
        InAppBrowser.OpenURL(openUrl);
    }

    public void OpenTwitter(string url)
    {
        string openUrl = ("https://twitter.com/" + url);
#if UNITY_EDITOR
        Application.OpenURL(openUrl);
#endif
        InAppBrowser.OpenURL(openUrl);
    }

    public void Follow(int index)
    {
        DataParser.Follow(index);
    }
}
