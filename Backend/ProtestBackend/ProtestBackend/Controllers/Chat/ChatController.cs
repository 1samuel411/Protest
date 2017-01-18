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

namespace ProtestBackend.Controllers.Chat
{
    public class ChatController : Controller
    {
        // GET: Create
        // POST: Create
        public ActionResult Create()
        {
            #region Get Data
            string sessionToken = Request.QueryString["sessionToken"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            string body = Request.QueryString["body"];
            if (String.IsNullOrEmpty(body))
                body = Request.Form["body"];
            string protest = Request.QueryString["protest"];
            if (String.IsNullOrEmpty(protest))
                protest = Request.Form["protest"];
            #endregion

            if(String.IsNullOrEmpty(sessionToken) || String.IsNullOrEmpty(body) || String.IsNullOrEmpty(protest))
            {
                return Content(Error.Create("Invalid request"));
            }

            int protestInt = 0;
            if(!int.TryParse(protest, out protestInt))
            {
                return Content(Error.Create("Invalid request"));
            }

            // Check user existance and get their id
            SqlCommand command = new SqlCommand("SELECT id, name, profilePicture FROM Users WHERE sessionToken=@sessionToken");
            command.Parameters.AddWithValue("@sessionToken", sessionToken);
            DataTable table = ConnectionManager.CreateQuery(command);
            if(table.Rows.Count <= 0)
            {
                return Content(Error.Create("SessionToken invalid"));
            }
            int id = table.Rows[0].Field<int>("id");
            string userName = table.Rows[0].Field<string>("name");
            string userProfilePicture = table.Rows[0].Field<string>("profilePicture");

            command.Parameters.Clear();
            command = new SqlCommand("INSERT INTO Chats VALUES (@body, @user, @time, @protest, @picture, @name)");
            command.Parameters.AddWithValue("@body", body);
            command.Parameters.AddWithValue("@name", userName);
            command.Parameters.AddWithValue("@user", id);
            command.Parameters.AddWithValue("@time", Parser.UnparseDate(DateTime.UtcNow));
            command.Parameters.AddWithValue("@protest", protestInt);
            command.Parameters.AddWithValue("@picture", userProfilePicture);
            int response = ConnectionManager.CreateCommand(command);
            if (response <= 0)
                return Content(Error.Create("Internal error"));

            command.Parameters.Clear();
            command.CommandText = "SELECT userId FROM GOING WHERE protestId=" + protestInt;
            table = ConnectionManager.CreateQuery(command);
            int targetIndex;
            for (int i = 0; i < table.Rows.Count; i++)
            {
                targetIndex = table.Rows[i].Field<int>("userId");
                if (targetIndex != id)
                    NotificationManager.SendNotification(id, targetIndex, userName + " commented, " + body, NotificationManager.Type.Protest, "chat");
            }

            return Content(Success.Create("Successfully sent message"));
        }

        // GET: Find
        // POST: Find
        public ActionResult Find()
        {
            #region Get Data
            string protest = Request.QueryString["protest"];
            if (String.IsNullOrEmpty(protest))
                protest = Request.Form["protest"];

            int protestInt = 0;
            if(!int.TryParse(protest, out protestInt))
            {
                return Content(Error.Create("Invalid request"));
            }
            #endregion

            SqlCommand command = new SqlCommand("SELECT TOP 100 * FROM Chats WHERE protest=" + protestInt + " ORDER BY id DESC");
            DataTable table = ConnectionManager.CreateQuery(command);
            string result = JsonConvert.SerializeObject(table);
            return Content(result);
        }
    }
}