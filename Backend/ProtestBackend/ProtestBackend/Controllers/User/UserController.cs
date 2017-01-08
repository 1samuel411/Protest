using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using Newtonsoft.Json;
using ProtestBackend.Models;
using ProtestBackend.DAL;
using System.Text.RegularExpressions;
using ProtestBackend.DLL;

namespace ProtestBackend.Controllers.User
{
    public class UserController : Controller
    {

        // GET: Find
        // POST: FIND
        public ActionResult Find()
        {
            int index;
            string indexQuery = Request.QueryString["index"];
            string sessionQuery = Request.QueryString["sessionToken"];
            if (String.IsNullOrEmpty(indexQuery))
                indexQuery = Request.Form["index"];
            if (String.IsNullOrEmpty(sessionQuery))
                sessionQuery = Request.Form["sessionToken"];

            if (!String.IsNullOrEmpty(indexQuery))
            {
                if (int.TryParse(indexQuery, out index))
                {
                    DataTable table = ConnectionManager.CreateQuery("SELECT id, profilePicture, email, name, bio, snapchatUser, facebookUser, instagramUser, twitterUser, notifyLikesComments, notifyFollowers, notifyFollowing, lastLogin FROM Users WHERE id=" + index);
                    
                    if (table.Rows.Count <= 0)
                    {
                        return Content(Error.Create("Index does not exist"));
                    }
                    else
                    {
                        UserModel user = new UserModel(table.Rows[0]);
                        user.HideData();

                        // Find following
                        DataTable tableInfo = ConnectionManager.CreateQuery("SELECT followingId FROM Following WHERE userId=" + index);
                        user.following = Parser.ParseColumnsToIntArray(tableInfo.Rows, 0);

                        // Find followers
                        tableInfo = ConnectionManager.CreateQuery("SELECT userId FROM Following WHERE followingId=" + index);
                        user.followers = Parser.ParseColumnsToIntArray(tableInfo.Rows, 0);

                        string result = JsonConvert.SerializeObject(user);
                        return Content(result);
                    }
                }
                else
                {
                    return Content(Error.Create("Index could not be parsed"));
                }
            }
            else if(String.IsNullOrEmpty(sessionQuery))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE sessionToken=@sessionToken");
                command.Parameters.AddWithValue("@sessionToken", sessionQuery);
                DataTable table = ConnectionManager.CreateQuery(command);
                if (table.Rows.Count <= 0)
                {
                    return Content(Error.Create("SessionToken does not exist"));
                }
                else
                {
                    UserModel user = new UserModel(table.Rows[0]);

                    // Find following
                    DataTable tableInfo = ConnectionManager.CreateQuery("SELECT followingId FROM Following WHERE userId=" + user.index);
                    user.following = Parser.ParseColumnsToIntArray(tableInfo.Rows, 0);

                    // Find followers
                    tableInfo = ConnectionManager.CreateQuery("SELECT followingId FROM Following WHERE followingId=" + user.index);
                    user.followers = Parser.ParseColumnsToIntArray(tableInfo.Rows, 0);

                    string result = JsonConvert.SerializeObject(user);
                    return Content(result);
                }
            }
            else
            {
                return Content(Error.Create("Invalid request"));
            }
        }

        // GET: FindUsers
        // POST: FindUsers
        public ActionResult FindUsers()
        {
            string indexQuery = Request.QueryString["index"];
            if (String.IsNullOrEmpty(indexQuery))
                indexQuery = Request.Form["index"];

            string nameQuery = Request.QueryString["name"];
            if (String.IsNullOrEmpty(nameQuery))
                nameQuery = Request.Form["name"];

            System.Diagnostics.Debug.WriteLine("name: " + nameQuery);
            if (!String.IsNullOrEmpty(indexQuery) && Regex.IsMatch(indexQuery, @"^([0-9]+,?)+$"))
            {
                string command = "SELECT id, profilePicture, name FROM Users WHERE id IN (" + indexQuery + ")";
                DataTable table = ConnectionManager.CreateQuery(command);
                if (table.Rows.Count <= 0)
                {
                    return Content(Error.Create("Index does not exist"));
                }
                else
                {
                    UserModel[] users = table.Select().Select(s => new UserModel(s, true)).ToArray();
                    string result = JsonConvert.SerializeObject(users);
                    return Content(result);
                }
            }
            else if(!String.IsNullOrEmpty(nameQuery))
            {
                SqlCommand command = new SqlCommand("SELECT id, profilePicture, name FROM Users WHERE name LIKE '%' + @nameQuery + '%'");
                command.Parameters.AddWithValue("@nameQuery", nameQuery);
                DataTable table = ConnectionManager.CreateQuery(command);
                if (table.Rows.Count <= 0)
                {
                    return Content(Error.Create("Index does not exist"));
                }
                else
                {
                    UserModel[] users = table.Select().Select(s => new UserModel(s, true)).Take(256).ToArray();
                    string result = JsonConvert.SerializeObject(users);
                    return Content(result);
                }
            }
            else
            {
                return Content(Error.Create("Invalid request"));
            }
        }

        // POST: Update
        public ActionResult Update()
        {
            #region Get data
            string sessionToken = Request.Form["sessionToken"];

            string profilePicture = Request.Form["profilePicture"];
            string email = Request.Form["email"];
            string name = Request.Form["name"];
            string bio = Request.Form["bio"];

            string snapchatUser = Request.Form["snapchatUser"];
            string facebookUser = Request.Form["facebookUser"];
            string instagramUser = Request.Form["instagramUser"];
            string twitterUser = Request.Form["twitterUser"];

            bool notifyLikesComments, notifyFollowers, notifyFollowing;
            bool notifyLikesCommentsSuccess = bool.TryParse(Request.Form["notifyLikesComments"], out notifyLikesComments);
            bool notifyFollowersSuccess = bool.TryParse(Request.Form["notifyFollowers"], out notifyFollowers);
            bool notifyFollowingSuccess = bool.TryParse(Request.Form["notifyFollowing"], out notifyFollowing);
            #endregion

            // No session token input
            if (String.IsNullOrEmpty(sessionToken))
            {
                return Content(Error.Create("Invalid request"));
            }

            // Token doesn't exist, return error
            if (!ConnectionManager.CheckExistance("Users", "sessionToken", sessionToken))
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            #region Create command
            string sqlQuery = "UPDATE Users SET";
            SqlCommand command = new SqlCommand(sqlQuery);

            if (!String.IsNullOrEmpty(profilePicture))
                command = ConnectionManager.AddProperty(command, "profilePicture", profilePicture);
            if (!String.IsNullOrEmpty(email))
                command = ConnectionManager.AddProperty(command, "email", email);
            if (!String.IsNullOrEmpty(name))
                command = ConnectionManager.AddProperty(command, "name", name);
            if (!String.IsNullOrEmpty(bio))
                command = ConnectionManager.AddProperty(command, "bio", bio);
            if (!String.IsNullOrEmpty(snapchatUser))
                command = ConnectionManager.AddProperty(command, "snapchatUser", snapchatUser);
            if (!String.IsNullOrEmpty(facebookUser))
                command = ConnectionManager.AddProperty(command, "facebookUser", snapchatUser);
            if (!String.IsNullOrEmpty(instagramUser))
                command = ConnectionManager.AddProperty(command, "instagramUser", instagramUser);
            if (!String.IsNullOrEmpty(twitterUser))
                command = ConnectionManager.AddProperty(command, "twitterUser", twitterUser);
            if (notifyLikesCommentsSuccess)
                command = ConnectionManager.AddProperty(command, "notifyLikesComments", notifyLikesComments.ToString());
            if (notifyFollowersSuccess)
                command = ConnectionManager.AddProperty(command, "notifyFollowers", notifyFollowers.ToString());
            if (notifyFollowingSuccess)
                command = ConnectionManager.AddProperty(command, "notifyFollowing", notifyFollowing.ToString());

            // Check if anything was added, if not then return an error
            if (command.CommandText == "UPDATE Users SET")
            {
                return Content(Error.Create("No values set"));
            }

            // Define which row
            command.CommandText+= " WHERE sessionToken=@sessionToken";
            command = ConnectionManager.AddParameter(command, "sessionToken", sessionToken);
            #endregion

            // Get response and send the command
            int response = ConnectionManager.CreateCommand(command);

            // No rows affected, so session key was invalid or something went wrong.
            if(response <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }
            else
            {
                // Success!
                return Content(Success.Create("Successfully updated the user"));
            }
        }

        // POST: Create
        public ActionResult Create()
        {
            #region Get data
            string lastLogin = "";
            string sessionToken = Request.Form["sessionToken"];

            string facebookUserToken = Request.Form["facebookUserToken"];
            string googleUserToken = Request.Form["googleUserToken"];

            string profilePicture = Request.Form["profilePicture"];
            string email = Request.Form["email"];
            string name = Request.Form["name"];
            string bio = Request.Form["bio"];

            string facebookUser = Request.Form["facebookUser"];
            #endregion

            // Check if user exists
            if (ConnectionManager.CheckExistance("Users", "sessionToken", sessionToken))
            {
                return Content(Error.Create("SessionToken already exists"));
            }

            #region Create command
            string sqlQuery = "INSERT INTO Users (sessionToken, email, profilePicture, name, bio, facebookUserToken, googleUserToken, lastLogin, facebookUser) VALUES (";
            SqlCommand command = new SqlCommand(sqlQuery);

            command = ConnectionManager.AddProperty(command, "sessionToken", sessionToken, true);
            command = ConnectionManager.AddProperty(command, "email", email, true);
            command = ConnectionManager.AddProperty(command, "profilePicture", profilePicture, true);
            command = ConnectionManager.AddProperty(command, "name", name, true);
            command = ConnectionManager.AddProperty(command, "bio", bio, true);

            if (String.IsNullOrEmpty(bio) || String.IsNullOrEmpty(sessionToken) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(profilePicture) || String.IsNullOrEmpty(name))
                return Content(Error.Create("Invalid request"));

            string originalQuery = command.CommandText;

            #region Optional Data
            if (!String.IsNullOrEmpty(facebookUserToken))
            {
                command = ConnectionManager.AddProperty(command, "facebookUserToken", facebookUserToken, true);
                command = ConnectionManager.AddProperty(command, "googleUserToken", "", true);
                lastLogin = "facebook";
            }
            else if(!String.IsNullOrEmpty(googleUserToken))
            {
                command = ConnectionManager.AddProperty(command, "facebookUserToken", "", true);
                command = ConnectionManager.AddProperty(command, "googleUserToken", googleUserToken, true);
                lastLogin = "google";
            }
            else
            {
                return Content(Error.Create("Invalid request"));
            }

            command = ConnectionManager.AddProperty(command, "lastLogin", lastLogin, true);

            if (String.IsNullOrEmpty(facebookUser))
                facebookUser = "";
            command = ConnectionManager.AddProperty(command, "facebookUser", facebookUser, true);
            #endregion

            if (command.CommandText == originalQuery)
            {
                return Content(Error.Create("Invalid request"));
            }

            command.CommandText += ")";

            #endregion

            string checkSession = "";
            if(lastLogin == "facebook")
                checkSession = FacebookManager.ValidateSession(sessionToken);

            if (checkSession != ((lastLogin == "facebook") ? facebookUserToken : googleUserToken))
            {
                return Content(Error.Create("SessionToken could not be validated"));
            }

            int response = ConnectionManager.CreateCommand(command);

            if (response <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }
            else
            {
                SuccessCreation success = new SuccessCreation("Successfully created the user", -1);
                DataTable table = ConnectionManager.CreateQuery("SELECT id FROM Users WHERE sessionToken='" + sessionToken + "'");
                if (table.Rows.Count <= 0)
                    return Content(Error.Create("Created but could not find."));

                success.index = table.Rows[0].Field<int>("id");
                return Content(success.ToString());
            }
        }

        // POST: Set Icon
        public ActionResult SetIcon()
        {
            return Content("");
        }

        // POST: Authenticate
        public ActionResult Authenticate()
        {
            #region Get data
            string lastLogin = "";
            string sessionToken = Request.Form["sessionToken"];

            string facebookUserToken = Request.Form["facebookUserToken"];
            string googleUserToken = Request.Form["googleUserToken"];

            string profilePicture = Request.Form["profilePicture"];
            string email = Request.Form["email"];
            string name = Request.Form["name"];
            string bio = Request.Form["bio"];

            string platform = Request.Form["platform"];

            string facebookUser = Request.Form["facebookUser"];
            #endregion

            if (String.IsNullOrEmpty(email))
                email = "";

            if (!String.IsNullOrEmpty(facebookUserToken))
                lastLogin = "facebook";
            else if (!String.IsNullOrEmpty(googleUserToken))
                lastLogin = "google";

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT (id) FROM Users WHERE facebookUserToken=@facebookUserToken";
            command.Parameters.AddWithValue("@facebookUserToken", facebookUserToken);
            DataTable table = ConnectionManager.CreateQuery(command);
            if(table.Rows.Count > 0)
            {
                // Exists

                // Update sessionToken
                command.Parameters.Clear();
                command.CommandText = "UPDATE Users SET lastLogin=@lastLogin, sessionToken=@sessionToken, lastDevice=@lastDevice WHERE id=@id";
                command.Parameters.AddWithValue("@sessionToken", sessionToken);
                command.Parameters.AddWithValue("@lastLogin", lastLogin);
                command.Parameters.AddWithValue("@lastDevice", platform);
                command.Parameters.AddWithValue("@id", table.Rows[0].Field<int>("id"));
                int response = ConnectionManager.CreateCommand(command);
                System.Diagnostics.Debug.WriteLine(response + " ------------------------------");
                if (response > 0)
                    return Content(SuccessCreation.Create("Successfully authenticated the user", table.Rows[0].Field<int>("id")).ToString());
                else
                    return Content(Error.Create("Invalid request"));
            }
            else
            {
                // Create

                command.Parameters.Clear();

                #region Create command
                string sqlQuery = "INSERT INTO Users (sessionToken, email, profilePicture, name, bio, lastDevice, facebookUserToken, googleUserToken, lastLogin, facebookUser) VALUES (";
                command.CommandText = sqlQuery;

                command = ConnectionManager.AddProperty(command, "sessionToken", sessionToken, true);
                command = ConnectionManager.AddProperty(command, "email", email, true);
                command = ConnectionManager.AddProperty(command, "profilePicture", profilePicture, true);
                command = ConnectionManager.AddProperty(command, "name", name, true);
                command = ConnectionManager.AddProperty(command, "bio", bio, true);
                command = ConnectionManager.AddProperty(command, "lastDevice", platform, true);

                if (String.IsNullOrEmpty(sessionToken) || String.IsNullOrEmpty(profilePicture) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(platform))
                    return Content(Error.Create("Invalid request"));

                #region Optional Data
                if (!String.IsNullOrEmpty(facebookUserToken))
                {
                    command = ConnectionManager.AddProperty(command, "facebookUserToken", facebookUserToken, true);
                    command = ConnectionManager.AddProperty(command, "googleUserToken", "", true);
                    lastLogin = "facebook";
                }
                else if (!String.IsNullOrEmpty(googleUserToken))
                {
                    command = ConnectionManager.AddProperty(command, "facebookUserToken", "", true);
                    command = ConnectionManager.AddProperty(command, "googleUserToken", googleUserToken, true);
                    lastLogin = "google";
                }
                else
                {
                    return Content(Error.Create("Invalid request"));
                }
                if (String.IsNullOrEmpty(facebookUser))
                    facebookUser = "";
                command = ConnectionManager.AddProperty(command, "lastLogin", lastLogin, true);
                command = ConnectionManager.AddProperty(command, "facebookUser", facebookUser, true);
                #endregion
                #endregion
                command.CommandText += ")";
                int response = ConnectionManager.CreateCommand(command);
                if(response <= 0)
                {
                    // Failed
                    return Content(Error.Create("Invalid request"));
                }
                else
                {
                    command.Parameters.Clear();
                    command.CommandText = "SELECT (id) FROM Users WHERE facebookUserToken=@facebookUserToken";
                    command.Parameters.AddWithValue("@facebookUserToken", facebookUserToken);
                    table = ConnectionManager.CreateQuery(command);
                    return Content(SuccessCreation.Create("Succesfully created the user", table.Rows[0].Field<int>("id")));
                }
            }
        }

        // POST: Follow
        // GET: Follow
        public ActionResult Follow()
        {
            #region Get Data
            string sessionToken = Request.QueryString["sessionToken"];
            string index = Request.QueryString["index"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];

            int indexint = -1;
            if(!int.TryParse(index, out indexint))
            {
                return Content(Error.Create("Index could not be parsed"));
            }
            #endregion

            SqlCommand command = new SqlCommand("SELECT id, name FROM Users WHERE sessionToken=@sessionToken");
            command.Parameters.AddWithValue("@sessionToken", sessionToken);
            
            // Check that the user we are going to modify exists
            DataTable user = ConnectionManager.CreateQuery(command);
            if(user.Rows.Count <= 0)
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            // Check that we aren't following ourselves
            if(user.Rows[0].Field<int>("id") == indexint)
            {
                return Content(Error.Create("Cannot follow yourself"));
            }

            // Check that user we are going to follow exists
            command = new SqlCommand("SELECT (notifyFollowers) FROM Users WHERE id=" + indexint);
            DataTable tableIndex = ConnectionManager.CreateQuery(command);

            if(tableIndex.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }

            int id = user.Rows[0].Field<int>("id");
            bool notifyUser = tableIndex.Rows[0].Field<bool>("notifyFollowers");

            // Check if Following already exists
            command = new SqlCommand("SELECT id FROM Following WHERE userId=" + id + " AND followingId=" + indexint);
            DataTable checkFollowing = ConnectionManager.CreateQuery(command);

            // Delete if so
            if(checkFollowing.Rows.Count >= 1)
            {
                command = new SqlCommand("DELETE FROM Following WHERE id=" + checkFollowing.Rows[0].Field<int>("id"));
                int responseDelete = ConnectionManager.CreateCommand(command);
                if (responseDelete >= 1)
                {
                    if(notifyUser)
                    {
                        // notify user we unfollowed
                        NotificationManager.SendNotification(indexint, user.Rows[0].Field<string>("name") + " unfollowed you.");
                    }
                    return Content(Success.Create("Successfully unfollowed user"));
                }
                else
                {
                    return Content(Error.Create("Internal error"));
                }
            }

            // Create if not
            command = new SqlCommand("INSERT INTO Following (userId, followingId) VALUES (" + id + "," + indexint + ")");
            int response = ConnectionManager.CreateCommand(command);
            if (response >= 1)
            {
                if(notifyUser)
                {
                    //notify user we followed
                    NotificationManager.SendNotification(indexint, user.Rows[0].Field<string>("name") + " followed you.");
                }
                return Content(Success.Create("Successfully followed user"));
            }
            else
                return Content(Error.Create("Internal error"));
        }

        // POST: Report
        // Get: Report
        public ActionResult Report()
        {
            #region Get Data
            string index = Request.QueryString["index"];
            string reason = Request.QueryString["reason"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];
            if (String.IsNullOrEmpty(reason))
                reason = Request.Form["reason"];

            int indexint = -1;
            if (!int.TryParse(index, out indexint))
            {
                return Content(Error.Create("Index could not be parsed"));
            }
            #endregion

            SqlCommand command = new SqlCommand("SELECT 1 FROM Users WHERE id=" + indexint);
            DataTable table = ConnectionManager.CreateQuery(command);
            if(table.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }
            command = new SqlCommand("INSERT INTO ReportsUsers VALUES (@index, @reason)");
            command.Parameters.AddWithValue("@index", indexint);
            command.Parameters.AddWithValue("@reason", reason);
            int response = ConnectionManager.CreateCommand(command);
            if (response > 0)
            {
                return Content(SuccessCreation.Create("Successfully reported user", indexint));
            }
            else
                return Content(Error.Create("Internal error"));
        }

    }
}