using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBarView : View
{

    UserModel userToDisplay;

    public Image profileImage;
    public Text nameText;
    public Text bioText;
    public Button attendedButton;
    public Text attendedText;
    public Button protestsButton;
    public Text protestsText;
    public Button followersButton;
    public Text followersText;
    public Button followingButton;
    public Text followingText;

    public void ChangeInfo(int userModel)
    {
        DataParser.GetUser(userModel, GetUserCallback);
    }

    public void GetUserCallback(UserModel userModel)
    {
        userToDisplay = userModel;

        DataParser.SetSprite(profileImage, userToDisplay.profilePicture);

        nameText.text = userToDisplay.name;
        bioText.text = userToDisplay.bio;

        attendedButton.onClick.AddListener(ProtestsAttendedCallback);
        attendedText.text = DataParser.GetCount(userToDisplay.protestsAttended.Length);

        protestsButton.onClick.AddListener(ProtestsCreatedCallback);
        protestsText.text = DataParser.GetCount(userToDisplay.protestsCreated.Length);

        followersButton.onClick.AddListener(FollowersCallback);
        followersText.text = DataParser.GetCount(userToDisplay.followers.Length);

        followingButton.onClick.AddListener(FollowingCallback);
        followingText.text = DataParser.GetCount(userToDisplay.following.Length);
    }

    private void ProtestsAttendedCallback()
    {
        ListController.instance.Show(ListController.ShowType.attended, userToDisplay, ProtestListController.instance);
    }

    private void ProtestsCreatedCallback()
    {
        ListController.instance.Show(ListController.ShowType.created, userToDisplay, ProtestListController.instance);
    }

    private void FollowersCallback()
    {
        ListController.instance.Show(ListController.ShowType.followers, userToDisplay, ProtestListController.instance);
    }

    private void FollowingCallback()
    {
        ListController.instance.Show(ListController.ShowType.following, userToDisplay, ProtestListController.instance);
    }

    public void OpenProfile()
    {
        ProfileViewController.instance.Show(userToDisplay.index, ProtestListController.instance);
    }
    
    public void Logout()
    {
        Popup.Create("Logout", "Are you sure you want to logout?", LogoutPopupCallback, "Popup", "Continue", "Cancel");
    }

    void LogoutPopupCallback(int response)
    {
        if(response == 1)
        {
            ProtestListController.instance.GetView().HideMenu();
            Authentication.Logout();
        }
    }

    public void OpenNews()
    {
        ListController.instance.Show(ListController.ShowType.news, Authentication.userModel, ProtestListController.instance);
    }
}
