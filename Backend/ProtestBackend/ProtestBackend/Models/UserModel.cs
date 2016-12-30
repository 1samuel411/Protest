using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtestBackend.Controllers;
using System.Data;

namespace ProtestBackend.Models
{
    public class UserModel
    {
        public int index;

        public string sessionToken;
        public string facebookUserToken;
        public string googleUserToken;

        public string profilePicture;

        public string email;
        public string name;
        public string bio;

        public int[] protestsAttended;
        public int[] protestsCreated;
        public int[] followers;
        public int[] following;

        public string snapchatUser;
        public string facebookUser;
        public string instagramUser;
        public string twitterUser;

        public bool notifyLikesComments;
        public bool notifyFollowers;
        public bool notifyFollowing;

        public UserModel
            (
                int index,
                string sessionToken, string facebookUserToken, string googleUserToken,
                string profilePicture,
                string email, string name, string bio,
                string protestsAttended, string protestsCreated, string followers, string following,
                string snapchatUser, string facebookUser, string twitterUser,
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

            this.protestsAttended = IntegerArrayParser.ParseString(protestsAttended);
            this.protestsCreated = IntegerArrayParser.ParseString(protestsCreated);
            this.followers = IntegerArrayParser.ParseString(followers);
            this.following = IntegerArrayParser.ParseString(following);
        }

        public UserModel(DataTable dataTable)
        {

        }
    }
}