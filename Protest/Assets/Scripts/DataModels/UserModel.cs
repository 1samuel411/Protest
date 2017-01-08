using System.Collections;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class UserModel : Model
{

    public int index;

    // raw information
    public string sessionToken;
    public string facebookUserToken;
    public string googleUserToken;
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
            string sessionToken, string facebookUserToken, string googleUserToken, 
            string profilePicture, string email, string name, string bio,
            int[] protestsAttended, int[] protestsCreated,
            int[] followers, int[] following, 
            string snapchatUser, string facebookUser, string instagramUser, string twitterUser, 
            bool notifyLikesComments, bool notifyFollowers, bool notifyFollowing
        )
    {
        this.index = index;

        this.sessionToken = sessionToken;
        this.facebookUserToken = facebookUserToken;
        this.googleUserToken = googleUserToken;

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

    public UserModel(JSONObject jsonObj)
    {
        index = int.Parse(jsonObj.GetField("index").ToString());
        sessionToken = jsonObj.GetField("sessionToken").str;
        facebookUserToken = jsonObj.GetField("facebookUserToken").str;
        googleUserToken = jsonObj.GetField("googleUserToken").str;
        profilePicture = jsonObj.GetField("profilePicture").str;
        email = jsonObj.GetField("email").str;
        name = jsonObj.GetField("name").str;
        bio = jsonObj.GetField("bio").str;
        if(!string.IsNullOrEmpty(bio))
            bio = bio.Replace(@"\t", "");

        protestsAttended = new int[0];
        protestsCreated = new int[0];
        followers = new int[0];
        following = new int[0];

        if (jsonObj.HasField("protestsAttended") && jsonObj.GetField("protestsAttended").list != null)
            protestsAttended = DataParser.ParseJsonToIntArray(jsonObj.GetField("protestsAttended").list);
        if (jsonObj.HasField("protestsCreated") && jsonObj.GetField("protestsCreated").list != null)
            protestsCreated = DataParser.ParseJsonToIntArray(jsonObj.GetField("protestsCreated").list);
        if (jsonObj.HasField("followers") && jsonObj.GetField("followers").list != null)
            followers = DataParser.ParseJsonToIntArray(jsonObj.GetField("followers").list);
        if (jsonObj.HasField("following") && jsonObj.GetField("following").list != null)
            following = DataParser.ParseJsonToIntArray(jsonObj.GetField("following").list);

        snapchatUser = jsonObj.GetField("snapchatUser").str;
        facebookUser = jsonObj.GetField("facebookUser").str;
        instagramUser = jsonObj.GetField("instagramUser").str;
        twitterUser = jsonObj.GetField("twitterUser").str;

        notifyLikesComments = jsonObj.GetField("notifyLikesComments").b;
        notifyFollowers = jsonObj.GetField("notifyFollowers").b;
        notifyFollowing = jsonObj.GetField("notifyFollowing").b;
    }
}
