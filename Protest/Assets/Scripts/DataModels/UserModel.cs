using System.Collections;
using System.Collections.Generic;

public class UserModel : Model
{

    public int index;

    // raw information
    public string authTokenSession;
    public string authTokenFacebook;
    public string authTokenGoogle;
    public string profilePicture;
    public string email;
    public string name;
    public string bio;

    public ProtestModel[] protestsAttended;
    public ProtestModel[] protestsCreated;

    // user followers and following
    public UserModel[] followers;
    public UserModel[] following;

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
            string authTokenSession, string authTokenFacebook, string authTokenGoogle, string profilePicture, string email, string name, string bio, 
            ProtestModel[] protestsAttended, ProtestModel[] protestsCreated, 
            UserModel[] followers, UserModel[] following, 
            string snapchatUser, string facebookUser, string instagramUser, string twitterUser, 
            bool notifyLikesComments, bool notifyFollowers, bool notifyFollowing
        )
    {
        this.index = index;

        this.authTokenSession = authTokenSession;
        this.authTokenFacebook = authTokenFacebook;
        this.authTokenGoogle = authTokenGoogle;
        this.profilePicture = profilePicture;
        this.email = email;
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
