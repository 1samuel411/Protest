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

            this.protestsAttended = DataLogicLayer.ParseString(protestsAttended);
            this.protestsCreated = DataLogicLayer.ParseString(protestsCreated);
            this.followers = DataLogicLayer.ParseString(followers);
            this.following = DataLogicLayer.ParseString(following);
        }

        public UserModel(DataRow dataTable)
        {
            this.index = int.Parse(dataTable["id"].ToString());

            this.sessionToken = dataTable["sessionToken"].ToString();
            this.facebookUserToken = dataTable["facebookUserToken"].ToString();
            this.googleUserToken = dataTable["googleUserToken"].ToString();

            this.profilePicture = dataTable["profilePicture"].ToString();
            this.name = dataTable["profilePicture"].ToString();
            this.bio = dataTable["profilePicture"].ToString();

            this.protestsAttended = DataLogicLayer.ParseString(dataTable["protestsAttended"].ToString());
            this.protestsCreated = DataLogicLayer.ParseString(dataTable["protestsCreated"].ToString());
            this.followers = DataLogicLayer.ParseString(dataTable["followers"].ToString());
            this.following = DataLogicLayer.ParseString(dataTable["following"].ToString());

            this.notifyLikesComments = bool.Parse(dataTable["notifyLikesComments"].ToString());
            this.notifyFollowers = bool.Parse(dataTable["notifyFollowers"].ToString());
            this.notifyFollowing = bool.Parse(dataTable["notifyFollowing"].ToString());
        }
    }
}