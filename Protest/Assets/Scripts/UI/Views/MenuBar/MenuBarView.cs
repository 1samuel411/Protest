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
        userToDisplay = DataParser.GetUser(userModel);

        DataParser.SetSprite(profileImage, userToDisplay.profilePicture);

        nameText.text = userToDisplay.name;
        bioText.text = userToDisplay.bio;

        attendedButton.onClick.AddListener(ProtestsAttendedCallback);
        attendedText.text = userToDisplay.protestsAttended.Length.ToString();

        protestsButton.onClick.AddListener(ProtestsCreatedCallback);
        protestsText.text = userToDisplay.protestsCreated.Length.ToString();

        followersButton.onClick.AddListener(FollowersCallback);
        followersText.text = userToDisplay.followers.Length.ToString();

        followingButton.onClick.AddListener(FollowingCallback);
        followingText.text = userToDisplay.following.Length.ToString();
    }

    private void ProtestsAttendedCallback()
    {
        ListController.instance.Show(ListController.ShowType.attended, DataParser.GetUser(Authentication.user.index));
    }

    private void ProtestsCreatedCallback()
    {
        ListController.instance.Show(ListController.ShowType.created, DataParser.GetUser(Authentication.user.index));
    }

    private void FollowersCallback()
    {
        ListController.instance.Show(ListController.ShowType.followers, DataParser.GetUser(Authentication.user.index));
    }

    private void FollowingCallback()
    {
        ListController.instance.Show(ListController.ShowType.following, DataParser.GetUser(Authentication.user.index));
    }

    public void OpenProfile()
    {
        ProfileViewController.instance.Show(userToDisplay.index, ProtestListController.instance);
    }
}
