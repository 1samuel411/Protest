using Newtonsoft.Json;
using ProtestBackend.Controllers.Locations;
using ProtestBackend.DAL;
using ProtestBackend.DLL;
using ProtestBackend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ProtestBackend.Controllers.Protest
{
    public class ProtestController : Controller
    {
        //Get: Create
        //Post: Create
        public ActionResult Create()
        {
            #region Get Data
            float donationsTargetFloat = 0.0f;
            float latitudeFloat = 0.0f;
            float longitudeFloat = 0.0f;
            string protestPicture = Request.QueryString["protestPicture"];
            string name = Request.QueryString["name"];
            string description = Request.QueryString["description"];
            string location = Request.QueryString["location"];
            string date = Request.QueryString["date"];
            string donationsEmail = Request.QueryString["donationsEmail"];
            string donationsTarget = Request.QueryString["donationsTarget"];
            string sessionToken = Request.QueryString["sessionToken"];
            string latitude = Request.QueryString["latitude"];
            string longitude = Request.QueryString["longitude"];

            if (String.IsNullOrEmpty(protestPicture))
                protestPicture = Request.Form["protestPicture"];
            if (String.IsNullOrEmpty(name))
                name = Request.Form["name"];
            if (String.IsNullOrEmpty(description))
                description = Request.Form["description"];
            if (String.IsNullOrEmpty(location))
                location = Request.Form["location"];
            if (String.IsNullOrEmpty(date))
                date = Request.Form["date"];
            if (String.IsNullOrEmpty(donationsEmail))
                donationsEmail = Request.Form["donationsEmail"];
            if (String.IsNullOrEmpty(donationsTarget))
                donationsTarget = Request.Form["donationsTarget"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            if (String.IsNullOrEmpty(latitude))
                latitude = Request.Form["latitude"];
            if (String.IsNullOrEmpty(longitude))
                longitude = Request.Form["longitude"];

            if (!String.IsNullOrEmpty(donationsTarget))
            {
                if (!float.TryParse(donationsTarget, out donationsTargetFloat))
                {
                    return Content(Error.Create("Donations target could not be parsed"));
                }
            }
            else
            {
                donationsTargetFloat = 0.0f;
            }
            if (!float.TryParse(latitude, out latitudeFloat))
            {
                return Content(Error.Create("Coordinates could not be parsed"));
            }
            if (!float.TryParse(longitude, out longitudeFloat))
            {
                return Content(Error.Create("Coordinates could not be parsed"));
            }
            #endregion

            if (String.IsNullOrEmpty(protestPicture) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(description) || String.IsNullOrEmpty(location) || String.IsNullOrEmpty(date) || String.IsNullOrEmpty(sessionToken))
            {
                return Content(Error.Create("Invalid request"));
            }

            if (!String.IsNullOrEmpty(donationsEmail))
            {
                if (String.IsNullOrEmpty(donationsTarget))
                {
                    return Content(Error.Create("Invalid request"));
                }
            }
            else
                donationsEmail = "";

            SqlCommand command = new SqlCommand();
            DataTable table;
            command.CommandText = "SELECT id,name,profilePicture FROM Users WHERE sessionToken=@sessionToken";
            command.Parameters.AddWithValue("@sessionToken", sessionToken);
            table = ConnectionManager.CreateQuery(command);

            if(table.Rows.Count <= 0)
            {
                return Content("Session Token does not exist");
            }
            string time = Parser.UnparseDate(DateTime.UtcNow);
            command.Parameters.Clear();
            command.CommandText = "INSERT INTO Protests (protestPicture, name, description, location, date, donationsEmail, donationCurrent, donationTarget, userCreated, time, latitude, longitude)" +
                                                " VALUES (@protestPicture, @name, @description, @location, @date, @donationsEmail, 0, @donationTarget, @userCreated, @time, @latitude, @longitude)";
            command.Parameters.AddWithValue("@protestPicture", protestPicture);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@location", location);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@donationsEmail", donationsEmail);
            command.Parameters.AddWithValue("@donationTarget", donationsTargetFloat);
            command.Parameters.AddWithValue("@userCreated", table.Rows[0].Field<int>("id"));
            command.Parameters.AddWithValue("@time", time);
            command.Parameters.AddWithValue("@latitude", latitudeFloat);
            command.Parameters.AddWithValue("@longitude", longitudeFloat);
            int response = ConnectionManager.CreateCommand(command);
            if(response <= 0)
            {
                return Content("Internal error");
            }
            command.Parameters.Clear();
            command.CommandText = "SELECT id FROM Protests WHERE name=@name AND time=@time";
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@time", time);

            string userName = table.Rows[0].Field<string>("name");
            int userIndex = table.Rows[0].Field<int>("id");
            string userProfilePicture = table.Rows[0].Field<string>("profilePicture");
            table = ConnectionManager.CreateQuery(command);

            if (table.Rows.Count <= 0)
            {
                return Content("Internal error");
            }
            int indexCreated = table.Rows[0].Field<int>("id");

            command.Parameters.Clear();
            command.CommandText = "SELECT userId FROM Following WHERE followingId=" + userIndex;
            table = ConnectionManager.CreateQuery(command);
            int targetIndex;
            for(int i = 0; i < table.Rows.Count;i ++)
            {
                targetIndex = table.Rows[i].Field<int>("userId");
                if(targetIndex != userIndex)
                    NotificationManager.SendNotification(userIndex, targetIndex, userName + " created a new Protest.", NotificationManager.Type.Protest, "protest");
            }
            NotificationManager.CreateNotification(userIndex, indexCreated, userProfilePicture, userName + " created a new Protest, " + name, NotificationManager.Type.Protest);

            SqlCommand commandCreate = new SqlCommand("INSERT INTO Going (userId, protestId, time) VALUES (@user, @protest, @time)");
            commandCreate.Parameters.AddWithValue("@user", userIndex);
            commandCreate.Parameters.AddWithValue("@protest", indexCreated);
            commandCreate.Parameters.AddWithValue("@time", Parser.UnparseDate(DateTime.UtcNow));
            ConnectionManager.CreateCommand(commandCreate);

            return Content(SuccessCreation.Create("Successfully created protest", indexCreated));
        }

        // GET: Find
        // POST: FIND
        public ActionResult Find()
        {
            int index;
            string indexQuery = Request.QueryString["index"];
            if (String.IsNullOrEmpty(indexQuery))
                indexQuery = Request.Form["index"];

            if (!String.IsNullOrEmpty(indexQuery))
            {
                if (int.TryParse(indexQuery, out index))
                {
                    DataTable table = ConnectionManager.CreateQuery("SELECT id, protestPicture, name, description, location, date, time, donationsEmail, donationCurrent, donationTarget, userCreated, time, latitude, longitude, active FROM Protests WHERE deleted='False' AND id=" + index + "");

                    if (table.Rows.Count <= 0)
                    {
                        return Content(Error.Create("Index does not exist"));
                    }
                    else
                    {
                        ProtestModel protest = new ProtestModel(table.Rows[0]);

                        // Check expiration date
                        if (protest.active)
                        {
                            DateTime check = Parser.ParseDate(protest.date);
                            var request = WebRequest.Create(string.Format("https://maps.googleapis.com/maps/api/timezone/json?location={0},{1}&timestamp={2}&key={3}", Uri.EscapeDataString(protest.latitude.ToString()), Uri.EscapeDataString(protest.longitude.ToString()), Uri.EscapeDataString(Parser.ConvertToTimestamp(check).ToString()),Uri.EscapeDataString(WebConfigurationManager.AppSettings[LocationController.GEOCODINGAPITimeZone]))) as HttpWebRequest;
                            string responseContent = null;

                            try
                            {
                                using (var response = request.GetResponse() as HttpWebResponse)
                                {
                                    using (var reader = new StreamReader(response.GetResponseStream()))
                                    {
                                        responseContent = reader.ReadToEnd();
                                    }
                                }
                            }
                            catch (WebException ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex.Message);
                                return Content(Error.Create("Invalid request.."));
                            }

                            dynamic Data = JsonConvert.DeserializeObject(responseContent);
                            string timeZoneId = Data.timeZoneId.ToString();
                            TimeZoneInfo zone = TimeZoneInfo.FindSystemTimeZoneById(Parser.IanaToWindows(timeZoneId));
                            TimeZoneInfo.ConvertTime(check, zone);
                            if (check.ToUniversalTime() <= (DateTime.UtcNow.AddHours(-2)))
                            {
                                protest.active = false;
                                int response = ConnectionManager.CreateCommand("UPDATE Protests SET active='False' WHERE id=" + protest.index);
                            }
                        }

                        table = ConnectionManager.CreateQuery("SELECT userId FROM Going WHERE protestId=" + index);
                        protest.going = Parser.ParseColumnsToIntArray(table.Rows, 0);

                        table = ConnectionManager.CreateQuery("SELECT userId FROM Likes WHERE protestId=" + index);
                        protest.likes = Parser.ParseColumnsToIntArray(table.Rows, 0);

                        table = ConnectionManager.CreateQuery("SELECT id FROM Contributions WHERE protest=" + index);
                        protest.contributions = Parser.ParseColumnsToIntArray(table.Rows, 0);

                        protest.chats = new int[0];

                        string result = JsonConvert.SerializeObject(protest);
                        return Content(result);
                    }
                }
                else
                {
                    return Content(Error.Create("Index could not be parsed"));
                }
            }
            else
                return Content(Error.Create("Invalid request"));
        }

        // GET: Update
        // POST: Update
        public ActionResult Update()
        {
            #region Get Data
            float donationsTargetFloat = 0.0f;
            float latitudeFloat = 0.0f;
            float longitudeFloat = 0.0f;
            int indexInt = 0;
            string index = Request.QueryString["index"];
            string protestInt = Request.QueryString["protestPicture"];
            string protestPicture = Request.QueryString["protestPicture"];
            string name = Request.QueryString["name"];
            string description = Request.QueryString["description"];
            string location = Request.QueryString["location"];
            string date = Request.QueryString["date"];
            string donationsEmail = Request.QueryString["donationsEmail"];
            string donationsTarget = Request.QueryString["donationsTarget"];
            string sessionToken = Request.QueryString["sessionToken"];
            string latitude = Request.QueryString["latitude"];
            string longitude = Request.QueryString["longitude"];

            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];
            if (String.IsNullOrEmpty(protestPicture))
                protestPicture = Request.Form["protestPicture"];
            if (String.IsNullOrEmpty(name))
                name = Request.Form["name"];
            if (String.IsNullOrEmpty(description))
                description = Request.Form["description"];
            if (String.IsNullOrEmpty(location))
                location = Request.Form["location"];
            if (String.IsNullOrEmpty(date))
                date = Request.Form["date"];
            if (String.IsNullOrEmpty(donationsEmail))
                donationsEmail = Request.Form["donationsEmail"];
            if (String.IsNullOrEmpty(donationsTarget))
                donationsTarget = Request.Form["donationsTarget"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            if (String.IsNullOrEmpty(latitude))
                latitude = Request.Form["latitude"];
            if (String.IsNullOrEmpty(longitude))
                longitude = Request.Form["longitude"];

            if (!String.IsNullOrEmpty(donationsTarget))
            {
                if (!float.TryParse(donationsTarget, out donationsTargetFloat))
                {
                    return Content(Error.Create("Donations target could not be parsed"));
                }
            }
            else
            {
                donationsTargetFloat = 0.0f;
            }
            if (!int.TryParse(index, out indexInt))
            {
                return Content(Error.Create("Index could not be parsed"));
            }
            if (!float.TryParse(latitude, out latitudeFloat))
            {
                return Content(Error.Create("Coordinates could not be parsed"));
            }
            if (!float.TryParse(longitude, out longitudeFloat))
            {
                return Content(Error.Create("Coordinates could not be parsed"));
            }
            #endregion

            if (String.IsNullOrEmpty(protestPicture) || String.IsNullOrEmpty(name) || String.IsNullOrEmpty(description) || String.IsNullOrEmpty(location) || String.IsNullOrEmpty(date) || String.IsNullOrEmpty(sessionToken))
            {
                return Content(Error.Create("Invalid request"));
            }

            if (!String.IsNullOrEmpty(donationsEmail))
            {
                if (String.IsNullOrEmpty(donationsTarget))
                {
                    return Content(Error.Create("Invalid request"));
                }
            }
            else
                donationsEmail = "";

            SqlCommand command = new SqlCommand();
            DataTable table;
            command.CommandText = "SELECT userCreated FROM Protests WHERE id=" + indexInt;
            table = ConnectionManager.CreateQuery(command);

            if (table.Rows.Count <= 0)
            {
                return Content("Index does not exist");
            }
            int userCreated = table.Rows[0].Field<int>("userCreated");

            command.CommandText = "SELECT sessionToken,id,name,profilePicture FROM Users WHERE id=" + userCreated;
            table = ConnectionManager.CreateQuery(command);
            
            if (table.Rows.Count <= 0)
            {
                return Content("Index does not exist");
            }
            string sessionTokenCheck = table.Rows[0].Field<string>("sessionToken");
            if(sessionToken != sessionTokenCheck)
            {
                return Content("Session Token does not exist");
            }
            string time = Parser.UnparseDate(DateTime.UtcNow);
            command.Parameters.Clear();
            command.CommandText = "UPDATE Protests SET protestPicture=@protestPicture, name=@name, description=@description, location=@location, date=@date, donationsEmail=@donationsEmail, donationTarget=@donationTarget, userCreated=@userCreated, latitude=@latitude, longitude=@longitude WHERE id=" + index;
            command.Parameters.AddWithValue("@protestPicture", protestPicture);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@location", location);
            command.Parameters.AddWithValue("@date", date);
            command.Parameters.AddWithValue("@donationsEmail", donationsEmail);
            command.Parameters.AddWithValue("@donationTarget", donationsTargetFloat);
            command.Parameters.AddWithValue("@userCreated", table.Rows[0].Field<int>("id"));
            command.Parameters.AddWithValue("@latitude", latitudeFloat);
            command.Parameters.AddWithValue("@longitude", longitudeFloat);
            int response = ConnectionManager.CreateCommand(command);
            if (response <= 0)
            {
                return Content("Internal error");
            }
            command.Parameters.Clear();

            string userName = table.Rows[0].Field<string>("name");
            int userIndex = table.Rows[0].Field<int>("id");
            string userProfilePicture = table.Rows[0].Field<string>("profilePicture");

            command.Parameters.Clear();
            command.CommandText = "SELECT userId FROM GOING WHERE protestId=" + indexInt;
            table = ConnectionManager.CreateQuery(command);
            int targetIndex;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                targetIndex = table.Rows[i].Field<int>("userId");
                if(targetIndex != userIndex)
                    NotificationManager.SendNotification(userIndex, targetIndex, userName + " updated a Protest you are attending.", NotificationManager.Type.Protest, "protest");
            }
            NotificationManager.CreateNotification(userIndex, indexInt, userProfilePicture, userName + " updated the Protest, " + name, NotificationManager.Type.Protest);

            return Content(SuccessCreation.Create("Successfully updated protest", indexInt));
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

            SqlCommand command = new SqlCommand("SELECT 1 FROM Protests WHERE id=" + indexint);
            DataTable table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }
            command = new SqlCommand("INSERT INTO ReportsProtests VALUES (@index, @reason, @time)");
            command.Parameters.AddWithValue("@index", indexint);
            command.Parameters.AddWithValue("@reason", reason);
            command.Parameters.AddWithValue("@time", Parser.UnparseDate(DateTime.UtcNow));
            int response = ConnectionManager.CreateCommand(command);
            if (response > 0)
            {
                return Content(SuccessCreation.Create("Successfully reported protest", indexint));
            }
            else
                return Content(Error.Create("Internal error"));
        }
        
        // POST: FindProtests
        // GET: FindProtests
        public ActionResult FindProtests()
        {
            string indexQuery = Request.QueryString["index"];
            if (String.IsNullOrEmpty(indexQuery))
                indexQuery = Request.Form["index"];

            string nameQuery = Request.QueryString["name"];
            if (String.IsNullOrEmpty(nameQuery))
                nameQuery = Request.Form["name"];

            string latitude = Request.QueryString["latitude"];
            if (String.IsNullOrEmpty(latitude))
                latitude = Request.Form["latitude"];

            string longitude = Request.QueryString["longitude"];
            if (String.IsNullOrEmpty(longitude))
                longitude = Request.Form["longitude"];
            
            if (!String.IsNullOrEmpty(nameQuery))
                nameQuery = nameQuery.Trim();

            DataTable table;

            if (!String.IsNullOrEmpty(indexQuery) && Regex.IsMatch(indexQuery, @"^([0-9]+,?)+$"))
            {
                SqlCommand command = new SqlCommand();
                if (String.IsNullOrEmpty(nameQuery))
                {
                    command = new SqlCommand("SELECT TOP 500 id, protestPicture, name, location, date, latitude, longitude, userCreated FROM Protests WHERE deleted='False' AND id IN (" + indexQuery + ") ORDER BY id DESC");
                    table = ConnectionManager.CreateQuery(command);
                }
                else
                {
                    command = new SqlCommand("SELECT TOP 500 id, protestPicture, name, location, date, latitude, longitude, userCreated FROM Protests WHERE deleted='False' AND id IN (" + indexQuery + ") AND name LIKE '%' + @search + '%' OR location LIKE '%' + @search + '%' ORDER BY id DESC");
                    command.Parameters.AddWithValue("@search", nameQuery);
                    table = ConnectionManager.CreateQuery(command);
                }
            }
            else if (!String.IsNullOrEmpty(nameQuery))
            {
                SqlCommand command = new SqlCommand("SELECT TOP 500 id, protestPicture, name, location, date, latitude, longitude, userCreated FROM Protests WHERE deleted='False' AND name LIKE '%' + @search + '%' OR location LIKE '%' + @search + '%' ORDER BY id DESC");
                command.Parameters.AddWithValue("@search", nameQuery);
                table = ConnectionManager.CreateQuery(command);
            }
            else if (!String.IsNullOrEmpty(latitude) && !String.IsNullOrEmpty(longitude))
            {
                float lat, lng;
                if (!float.TryParse(latitude, out lat))
                    return Content(Error.Create("Latitude or longitude could not be parsed"));
                if (!float.TryParse(longitude, out lng))
                    return Content(Error.Create("Latitude or longitude could not be parsed"));

                SqlCommand command = new SqlCommand("SELECT TOP 500 id, protestPicture, name, location, date, latitude, longitude, userCreated FROM Protests WHERE active='True' AND deleted='False' ORDER BY ((latitude-@latitude)*(latitude-@latitude)) + ((longitude-@longitude)*(longitude-@longitude)) ASC, id DESC");
                command.Parameters.AddWithValue("@latitude", latitude);
                command.Parameters.AddWithValue("@longitude", longitude);
                table = ConnectionManager.CreateQuery(command);
            }
            else
            {
                return Content(Error.Create("Invalid request"));
            }

            if(table.Rows.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }

            string result = "";

            ProtestModel[] protests = table.Select().Select(s => new ProtestModel(s)).ToArray();
            for(int i = 0; i < protests.Length; i++)
            {
                protests[i].likesCount = ConnectionManager.CreateScalar(new SqlCommand("SELECT COUNT(*) FROM Likes WHERE protestId=" + protests[i].index.ToString()));
                protests[i].goingCount = ConnectionManager.CreateScalar(new SqlCommand("SELECT COUNT(*) FROM Going WHERE protestId=" + protests[i].index.ToString()));
            }
            result = JsonConvert.SerializeObject(protests);

            return Content(result);
        }

        // POST: Like
        // GET: Like
        public ActionResult Like()
        {
            #region Get Data
            string sessionToken = Request.QueryString["sessionToken"];
            string index = Request.QueryString["index"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];

            int indexint = -1;
            if (!int.TryParse(index, out indexint))
            {
                return Content(Error.Create("Index could not be parsed"));
            }
            #endregion

            SqlCommand command = new SqlCommand("SELECT id, name, profilePicture FROM Users WHERE sessionToken=@sessionToken");
            command.Parameters.AddWithValue("@sessionToken", sessionToken);

            // Check that the user we are going to modify exists
            DataTable user = ConnectionManager.CreateQuery(command);
            if (user.Rows.Count <= 0)
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            string userName = user.Rows[0].Field<string>("name");
            int id = user.Rows[0].Field<int>("id");
            string profilePicture = user.Rows[0].Field<string>("profilePicture");

            command.Parameters.Clear();
            command = new SqlCommand("SELECT userCreated, name FROM Protests WHERE id=" + indexint);
            DataTable tableIndex = ConnectionManager.CreateQuery(command);
            if (tableIndex.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }

            int userProtestCreated = tableIndex.Rows[0].Field<int>("userCreated");
            string protestName = tableIndex.Rows[0].Field<string>("name");

            // Check that user wants our notification
            command = new SqlCommand("SELECT notifyLikesComments, name FROM Users WHERE id=" + userProtestCreated);
            tableIndex = ConnectionManager.CreateQuery(command);

            if (tableIndex.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }

            bool notifyUser = tableIndex.Rows[0].Field<bool>("notifyLikesComments");

            // Check if Like already exists
            command = new SqlCommand("SELECT id FROM Likes WHERE userId=" + id + " AND protestId=" + indexint);
            DataTable check = ConnectionManager.CreateQuery(command);

            // Delete if so
            if (check.Rows.Count >= 1)
            {
                command = new SqlCommand("DELETE FROM Likes WHERE id=" + check.Rows[0].Field<int>("id"));
                int responseDelete = ConnectionManager.CreateCommand(command);
                if (responseDelete >= 1)
                {
                    if (notifyUser)
                    {
                        // notify user we unfollowed
                        if(id != indexint)
                            NotificationManager.SendNotification(id, userProtestCreated, userName + " unliked your Protest.", NotificationManager.Type.Protest, "likes");
                    }

                    NotificationManager.CreateNotification(id, indexint, profilePicture, userName + " unliked " + protestName + "", NotificationManager.Type.Protest);
                    return Content(Success.Create("Successfully unliked protest"));
                }
                else
                {
                    return Content(Error.Create("Internal error"));
                }
            }

            // Create if not
            command = new SqlCommand("INSERT INTO Likes (userId, protestId, time) VALUES (" + id + "," + indexint + "," + "@time" + ")");
            command.Parameters.AddWithValue("@time", Parser.UnparseDate(DateTime.UtcNow));
            int response = ConnectionManager.CreateCommand(command);
            if (response >= 1)
            {
                if (notifyUser)
                {
                    // notify user we unfollowed
                    if(id != indexint)
                        NotificationManager.SendNotification(id, userProtestCreated, userName + " liked your Protest.", NotificationManager.Type.Protest, "likes");
                }

                NotificationManager.CreateNotification(id, indexint, profilePicture, userName + " liked " + protestName + "", NotificationManager.Type.Protest);
                return Content(Success.Create("Successfully liked protest"));
            }
            else
                return Content(Error.Create("Internal error"));
        }

        // POST: Going
        // GET: Going
        public ActionResult Going()
        {
            #region Get Data
            string sessionToken = Request.QueryString["sessionToken"];
            string index = Request.QueryString["index"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];

            int indexint = -1;
            if (!int.TryParse(index, out indexint))
            {
                return Content(Error.Create("Index could not be parsed"));
            }
            #endregion

            SqlCommand command = new SqlCommand("SELECT id, name, profilePicture FROM Users WHERE sessionToken=@sessionToken");
            command.Parameters.AddWithValue("@sessionToken", sessionToken);

            // Check that the user we are going to modify exists
            DataTable user = ConnectionManager.CreateQuery(command);
            if (user.Rows.Count <= 0)
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            string userName = user.Rows[0].Field<string>("name");
            int id = user.Rows[0].Field<int>("id");
            string profilePicture = user.Rows[0].Field<string>("profilePicture");

            command.Parameters.Clear();
            command = new SqlCommand("SELECT userCreated, name FROM Protests WHERE id=" + indexint);
            DataTable tableIndex = ConnectionManager.CreateQuery(command);
            if (tableIndex.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }

            int userProtestCreated = tableIndex.Rows[0].Field<int>("userCreated");
            string protestName = tableIndex.Rows[0].Field<string>("name");

            // Check that user wants our notification
            command = new SqlCommand("SELECT notifyLikesComments, name FROM Users WHERE id=" + userProtestCreated);
            tableIndex = ConnectionManager.CreateQuery(command);

            if (tableIndex.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }

            bool notifyUser = tableIndex.Rows[0].Field<bool>("notifyLikesComments");

            // Check if Like already exists
            command = new SqlCommand("SELECT id FROM Going WHERE userId=" + id + " AND protestId=" + indexint);
            DataTable check = ConnectionManager.CreateQuery(command);

            // Delete if so
            if (check.Rows.Count >= 1)
            {
                command = new SqlCommand("DELETE FROM Going WHERE id=" + check.Rows[0].Field<int>("id"));
                int responseDelete = ConnectionManager.CreateCommand(command);
                if (responseDelete >= 1)
                {
                    if (notifyUser)
                    {
                        // notify user we unwent
                        NotificationManager.SendNotification(id, userProtestCreated, userName + " is not going to your Protest.", NotificationManager.Type.Protest, "going");
                    }

                    NotificationManager.CreateNotification(id, indexint, profilePicture, userName + " is not going to the Protest, " + protestName + "", NotificationManager.Type.Protest);
                    return Content(Success.Create("Successfully unwent to the protest"));
                }
                else
                {
                    return Content(Error.Create("Internal error"));
                }
            }

            // Create if not
            command = new SqlCommand("INSERT INTO Going (userId, protestId, time) VALUES (" + id + "," + indexint + "," + "@time" + ")");
            command.Parameters.AddWithValue("@time", Parser.UnparseDate(DateTime.UtcNow));
            int response = ConnectionManager.CreateCommand(command);
            if (response >= 1)
            {
                if (notifyUser)
                {
                    // notify user we are going
                    NotificationManager.SendNotification(id, userProtestCreated, userName + " is going to your Protest.", NotificationManager.Type.Protest, "going");
                }

                NotificationManager.CreateNotification(id, indexint, profilePicture, userName + " is going to " + protestName + "", NotificationManager.Type.Protest);
                return Content(Success.Create("Successfully going to the protest"));
            }
            else
                return Content(Error.Create("Internal error"));
        }

        // POST: Delete
        // GET: Delete
        public ActionResult Delete()
        {
            #region Get Data
            string sessionToken = Request.QueryString["sessionToken"];
            string index = Request.QueryString["index"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];

            int indexint = -1;
            if (!int.TryParse(index, out indexint))
            {
                return Content(Error.Create("Index could not be parsed"));
            }
            #endregion

            SqlCommand command = new SqlCommand("SELECT id, name, profilePicture FROM Users WHERE sessionToken=@sessionToken");
            command.Parameters.AddWithValue("@sessionToken", sessionToken);

            // Check that the user we are going to modify exists
            DataTable user = ConnectionManager.CreateQuery(command);
            if (user.Rows.Count <= 0)
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            command.Parameters.Clear();
            command = new SqlCommand("SELECT userCreated FROM Protests WHERE id=" + indexint);
            DataTable tableIndex = ConnectionManager.CreateQuery(command);

            if (tableIndex.Rows[0].Field<int>("userCreated") != user.Rows[0].Field<int>("id"))
                return Content(Error.Create("Not validated"));

            if (tableIndex.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist"));
            }

            // Delete
            command = new SqlCommand("UPDATE Protests SET deleted='True' WHERE id=" + indexint);
            int responseDelete = ConnectionManager.CreateCommand(command);
            if (responseDelete >= 1)
            {
                // Remove from Going and Likes and Chats
                command = new SqlCommand("DELETE FROM Going WHERE protestId=" + indexint);
                responseDelete = ConnectionManager.CreateCommand(command);
                command = new SqlCommand("DELETE FROM Likes WHERE protestId=" + indexint);
                responseDelete = ConnectionManager.CreateCommand(command);
                command = new SqlCommand("DELETE FROM Chats WHERE protest=" + indexint);
                responseDelete = ConnectionManager.CreateCommand(command);
                command = new SqlCommand("DELETE FROM Chats WHERE protest=" + indexint);
                responseDelete = ConnectionManager.CreateCommand(command);
                command = new SqlCommand("DELETE FROM Notifications WHERE Type='Protest' AND targetIndex=" + indexint);
                responseDelete = ConnectionManager.CreateCommand(command);
                return Content(Success.Create("Successfully deleted to the protest"));
            }
            else
            {
                return Content(Error.Create("Internal error"));
            }
        }

    }
}