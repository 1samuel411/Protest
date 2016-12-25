using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class UserModel : Model
{

    public int index;

    // raw information
    public string authTokenSession;
    public string profilePicture;
    public string email;
    public string name;
    public string bio;

    public int[] protestsAttended;
    public int[] protestsCreated;

    // user followers and following
    public int[] followers;
    public int[] following;

    // social
    public string snapchatUser;
    public string facebookUser;
    public string instagramUser;
    public string twitterUser;

    // notifcation settings
    public bool notifyLikesComments;
    public bool notifyFollowers;
    public bool notifyFollowing;

    // constructor
    public UserModel
        (
            int index,
            string authTokenSession, string profilePicture, string email, string name, string bio,
            int[] protestsAttended, int[] protestsCreated,
            int[] followers, int[] following, 
            string snapchatUser, string facebookUser, string instagramUser, string twitterUser, 
            bool notifyLikesComments, bool notifyFollowers, bool notifyFollowing
        )
    {
        this.index = index;

        this.authTokenSession = authTokenSession;
        this.profilePicture = profilePicture;
        this.email = email;
        this.name = name;
        this.bio = bio;

        this.protestsAttended = protestsAttended;
        this.protestsCreated = protestsCreated;
        this.followers = followers;
        this.following = following;

        this.snapchatUser = snapchatUser;
        this.facebookUser = facebookUser;
        this.instagramUser = instagramUser;
        this.twitterUser = twitterUser;

        this.notifyLikesComments = notifyLikesComments;
        this.notifyFollowers = notifyFollowers;
        this.notifyFollowing = notifyFollowing;
    }
}
