using Newtonsoft.Json;
using ProtestBackend.DAL;
using ProtestBackend.DLL;
using ProtestBackend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
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

            int targetIndex = 0;
            for(int i = 0; i < table.Rows.Count; i++)
            {
                targetIndex = table.Rows[i].Field<int>("userId");
                NotificationManager.SendNotification(userIndex, targetIndex, userName + " created a new Protest.", NotificationManager.Type.Protest, "protest");
                NotificationManager.CreateNotification(userIndex, targetIndex, userProfilePicture, userName + " created a new Protest, " + name, NotificationManager.Type.Protest);
            }

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
                    DataTable table = ConnectionManager.CreateQuery("SELECT id, protestPicture, name, description, location, date, time, donationsEmail, donationCurrent, donationTarget, userCreated, time, latitude, longitude, active FROM Protests WHERE id=" + index + " AND deleted='false'");

                    if (table.Rows.Count <= 0)
                    {
                        return Content(Error.Create("Index does not exist"));
                    }
                    else
                    {
                        ProtestModel protest = new ProtestModel(table.Rows[0]);

                        if (protest.active)
                        {
                            if (Parser.ParseDate(protest.date) <= (DateTime.UtcNow.AddHours(-2)))
                            {
                                protest.active = false;
                                int response = ConnectionManager.CreateCommand("UPDATE Protests SET active=False WHERE id=" + protest.index);
                            }
                        }

                        protest.going = new int[0];
                        protest.likes = new int[0];
                        protest.contributions = new int[0];
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
            command.CommandText = "UPDATE Protests SET protestPicture=@protestPicture, name=@name, description=@description, location=@location, date=@date, donationsEmail=@donationsEmail, donationTarget=@donationTarget, userCreated=@userCreated, latitude=@latitude, longitude=longitude WHERE id=" + index;
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
            command.CommandText = "SELECT userId FROM Following WHERE followingId=" + userIndex;
            table = ConnectionManager.CreateQuery(command);

            int targetIndex = 0;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                targetIndex = table.Rows[i].Field<int>("userId");
                NotificationManager.SendNotification(userIndex, targetIndex, userName + " updated a Protest.", NotificationManager.Type.Protest, "protest");
                NotificationManager.CreateNotification(userIndex, targetIndex, userProfilePicture, userName + " updated the Protest, " + name, NotificationManager.Type.Protest);
            }

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
    }
}