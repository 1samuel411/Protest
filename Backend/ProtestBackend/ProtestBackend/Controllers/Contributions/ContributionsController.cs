using Newtonsoft.Json;
using ProtestBackend.DAL;
using ProtestBackend.DLL;
using ProtestBackend.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ProtestBackend.Controllers.Contributions
{
    public class ContributionsController : Controller
    {
        // GET: Create
        // POST: Create
        public ActionResult Create()
        {
            #region Get Data
            string name = Request.QueryString["name"];
            if (String.IsNullOrEmpty(name))
                name = Request.Form["name"];

            int amountNeededInt = 0;
            string amountNeeded = Request.QueryString["amountNeeded"];
            if (String.IsNullOrEmpty(amountNeeded))
                amountNeeded = Request.Form["amountNeeded"];

            int protestInt = 0;
            string protest = Request.QueryString["protest"];
            if (String.IsNullOrEmpty(protest))
                protest = Request.Form["protest"];

            string sessionToken = Request.QueryString["sessionToken"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];

            if (!int.TryParse(amountNeeded, out amountNeededInt))
            {
                return Content(Error.Create("Invalid request"));
            }

            if (!int.TryParse(protest, out protestInt))
            {
                return Content(Error.Create("Invalid request"));
            }
            #endregion

            // check if protest exists
            SqlCommand command = new SqlCommand("SELECT userCreated FROM Protests WHERE id=" + protestInt);
            DataTable table = new DataTable();
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }
            int userCreated = table.Rows[0].Field<int>("userCreated");

            // Check that session tokens matches the user who created it
            command = new SqlCommand("SELECT sessionToken,name FROM Users WHERE id=" + userCreated);
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }

            string nameString = table.Rows[0].Field<string>("name");
            string sessionTokenConfirm = table.Rows[0].Field<string>("sessionToken");
            if (sessionToken != sessionTokenConfirm)
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            // create
            command = new SqlCommand("INSERT INTO Contributions (name, amountNeeded, protest, time) VALUES (@name, @amountNeeded, @protest, @time)");
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@amountNeeded", amountNeededInt);
            command.Parameters.AddWithValue("@protest", protestInt);
            command.Parameters.AddWithValue("@time", Parser.UnparseDate(DateTime.UtcNow));

            int response = ConnectionManager.CreateCommand(command);

            if (response <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }

            command.Parameters.Clear();
            command = new SqlCommand("SELECT id FROM Contributions WHERE name=@name AND amountNeeded=@amountNeeded AND protest=@protest AND time=@time");
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@amountNeeded", amountNeededInt);
            command.Parameters.AddWithValue("@protest", protestInt);
            command.Parameters.AddWithValue("@time", Parser.UnparseDate(DateTime.UtcNow));
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Internal error"));
            }

            int newContribution = table.Rows[0].Field<int>("id");

            command.Parameters.Clear();
            command.CommandText = "SELECT userId FROM GOING WHERE protestId=" + protest;
            table = ConnectionManager.CreateQuery(command);
            int targetIndex;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                targetIndex = table.Rows[i].Field<int>("userId");
                if (targetIndex != userCreated)
                    NotificationManager.SendNotification(userCreated, targetIndex, nameString + " added a new request for contributions to a protest you are attending.", NotificationManager.Type.Protest, "protest");
            }
            command = new SqlCommand("SELECT protestPicture,name FROM Protests WHERE id=" + protestInt);
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Internal error"));
            }
            string picture = table.Rows[0].Field<string>("protestPicture");
            string nameProtest = table.Rows[0].Field<string>("name");
            NotificationManager.CreateNotification(userCreated, protestInt, picture, nameString + " added a new contribution request for, " + nameProtest, NotificationManager.Type.Protest);

            return Content(SuccessCreation.Create("Successfully created contribution", newContribution));
        }

        // GET: Delete
        // POST: Delete
        public ActionResult Delete()
        {
            #region Get Data
            int indexInt = 0;
            string index = Request.QueryString["index"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];

            string sessionToken = Request.QueryString["sessionToken"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];

            if (!int.TryParse(index, out indexInt))
            {
                return Content(Error.Create("Invalid request"));
            }
            #endregion

            // check if contribution exists
            SqlCommand command = new SqlCommand("SELECT protest FROM Contributions WHERE id=" + indexInt);
            DataTable table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }

            int protestInt = table.Rows[0].Field<int>("protest");
            // check if protest exists
            command = new SqlCommand("SELECT userCreated FROM Protests WHERE id=" + protestInt);
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }
            int userCreated = table.Rows[0].Field<int>("userCreated");

            // Check that session tokens matches the user who created it
            command = new SqlCommand("SELECT sessionToken FROM Users WHERE id=" + userCreated);
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }

            string sessionTokenConfirm = table.Rows[0].Field<string>("sessionToken");
            if (sessionToken != sessionTokenConfirm)
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            // delete
            command = new SqlCommand("DELETE FROM Contributions WHERE id=" + index);

            int response = ConnectionManager.CreateCommand(command);

            if (response <= 0)
            {
                return Content(Error.Create("Internal error"));
            }

            return Content(Success.Create("Successfully deleted"));
        }

        // GET: Add
        // POST: Add
        public ActionResult Add()
        {
            #region Get Data
            int indexInt = 0;
            string index = Request.QueryString["index"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];

            string sessionToken = Request.QueryString["sessionToken"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];

            if (!int.TryParse(index, out indexInt))
            {
                return Content(Error.Create("Invalid request"));
            }
            #endregion

            // check if contribution exists
            SqlCommand command = new SqlCommand("SELECT 1 FROM Contributions WHERE id=" + indexInt);
            DataTable table = ConnectionManager.CreateQuery(command);
            if(table.Rows.Count <= 0)
                return Content(Error.Create("Invalid request"));

            // check if user exists
            command = new SqlCommand("SELECT name,id FROM Users WHERE sessionToken=@sessionToken");
            command.Parameters.AddWithValue("@sessionToken", sessionToken);
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Invalid request"));
            }

            string userName = table.Rows[0].Field<string>("name");
            int  userId = table.Rows[0].Field<int>("id");
            command.Parameters.Clear();

            // Add
            command = new SqlCommand("UPDATE Contributions SET currentAmount = currentAmount + 1 WHERE id=" + indexInt);

            int response = ConnectionManager.CreateCommand(command);

            if (response <= 0)
            {
                return Content(Error.Create("Internal error"));
            }

            command = new SqlCommand("SELECT protest FROM Contributions WHERE id=" + indexInt);
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Internal error"));
            }
            command = new SqlCommand("SELECT userCreated,name,protestPicture FROM Protests WHERE id=" + table.Rows[0].Field<int>("protest"));
            table = ConnectionManager.CreateQuery(command);
            if (table.Rows.Count <= 0)
            {
                return Content(Error.Create("Internal error"));
            }

            int targetIndex = table.Rows[0].Field<int>("userCreated");
            string protestPicture = table.Rows[0].Field<string>("protestPicture");
            string name = table.Rows[0].Field<string>("name");
            if (targetIndex != userId)
                NotificationManager.SendNotification(userId, targetIndex, userName + " contributed to your protest.", NotificationManager.Type.Protest, "protest");

            NotificationManager.CreateNotification(userId, indexInt, protestPicture, userName + " contributed to, " + name, NotificationManager.Type.Protest);

            return Content(Success.Create("Successfully contributed"));
        }

        // GET: Find
        // POST: Find
        public ActionResult Find()
        {
            #region Get Data
            string index = Request.QueryString["index"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];
            #endregion

            if (!String.IsNullOrEmpty(index) && Regex.IsMatch(index, @"^([0-9]+,?)+$"))
            {
                SqlCommand command = new SqlCommand("SELECT TOP 500 id, name, amountNeeded, currentAmount, protest FROM Contributions WHERE id IN (" + index + ") ORDER BY id DESC");
                DataTable table = ConnectionManager.CreateQuery(command);

                ContributionModel[] contribution = table.Select().Select(s => new ContributionModel(s)).ToArray();
                string result = JsonConvert.SerializeObject(contribution);

                return Content(result);
            }

            return Content(Error.Create("Invalid request"));
        }
    }
}