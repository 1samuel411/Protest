using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ProtestBackend.DLL;
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

        public string lastLogin;

        public UserModel
            (
                int index,
                string sessionToken, string facebookUserToken, string googleUserToken,
                string profilePicture,
                string email, string name, string bio,
                string protestsAttended, string protestsCreated, string followers, string following,
                string snapchatUser, string facebookUser, string twitterUser,
                bool notifyLikesComments, bool notifyFollowers, bool notifyFollowing,
                string lastLogin
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

            this.protestsAttended = Parser.ParseStringToIntArray(protestsAttended);
            this.protestsCreated = Parser.ParseStringToIntArray(protestsCreated);
            this.followers = Parser.ParseStringToIntArray(followers);
            this.following = Parser.ParseStringToIntArray(following);

            this.lastLogin = lastLogin;
        }

        public UserModel(DataRow dataTable, bool hide = false)
        {
            this.index = int.Parse(dataTable["id"].ToString());

            if (dataTable.Table.Columns.Contains("sessionToken"))
            {
                this.sessionToken = dataTable["sessionToken"].ToString();
                this.facebookUserToken = dataTable["facebookUserToken"].ToString();
                this.googleUserToken = dataTable["googleUserToken"].ToString();

                this.email = dataTable["email"].ToString();
            }

            if(dataTable.Table.Columns.Contains("profilePicture"))
                this.profilePicture = dataTable["profilePicture"].ToString();
            if(dataTable.Table.Columns.Contains("name"))
                this.name = dataTable["name"].ToString();

            if (dataTable.Table.Columns.Contains("bio"))
                this.bio = dataTable["bio"].ToString();

            if(dataTable.Table.Columns.Contains("snapchatUSer"))
                this.snapchatUser = dataTable["snapchatUSer"].ToString();
            if(dataTable.Table.Columns.Contains("facebookUser"))
                this.facebookUser = dataTable["facebookUser"].ToString();
            if(dataTable.Table.Columns.Contains("instagramUser"))
                this.instagramUser = dataTable["instagramUser"].ToString();
            if(dataTable.Table.Columns.Contains("twitterUser"))
                this.twitterUser = dataTable["twitterUser"].ToString();

            if(dataTable.Table.Columns.Contains("protestsAttended"))
                this.protestsAttended = Parser.ParseStringToIntArray(dataTable["protestsAttended"].ToString());
            if(dataTable.Table.Columns.Contains("protestsCreated"))
                this.protestsCreated = Parser.ParseStringToIntArray(dataTable["protestsCreated"].ToString());
            if(dataTable.Table.Columns.Contains("followers"))
                this.followers = Parser.ParseStringToIntArray(dataTable["followers"].ToString());
            if(dataTable.Table.Columns.Contains("following"))
                this.following = Parser.ParseStringToIntArray(dataTable["following"].ToString());

            if(dataTable.Table.Columns.Contains("notifyLikesComments"))
                this.notifyLikesComments = bool.Parse(dataTable["notifyLikesComments"].ToString());
            if(dataTable.Table.Columns.Contains("notifyFollowers"))
                this.notifyFollowers = bool.Parse(dataTable["notifyFollowers"].ToString());
            if(dataTable.Table.Columns.Contains("notifyFollowing"))
                this.notifyFollowing = bool.Parse(dataTable["notifyFollowing"].ToString());

            if(dataTable.Table.Columns.Contains("lastLogin"))
                this.lastLogin = dataTable["lastLogin"].ToString();

            if (hide)
                HideData();
        }

        public void HideData()
        {
            sessionToken = "hidden";
            facebookUserToken = "hidden";
            googleUserToken = "hidden";
            email = "hidden";
        }
    }
}