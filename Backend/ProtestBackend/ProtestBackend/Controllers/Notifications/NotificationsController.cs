using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProtestBackend.DLL;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using ProtestBackend.Models;
using System.Threading.Tasks;
using ProtestBackend.DAL;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ProtestBackend.Controllers.Notifications
{
    public class NotificationsController : Controller
    {
        // Get: Create
        // Post: Create
        public ActionResult Create()
        {
            #region Get Data
            string from = Request.QueryString["fromIndex"];
            string index = Request.QueryString["index"];
            string body = Request.QueryString["body"];
            string type = Request.QueryString["type"];
            if (String.IsNullOrEmpty(from))
                from = Request.Form["fromIndex"];
            if (String.IsNullOrEmpty(index))
                index = Request.Form["index"];
            if (String.IsNullOrEmpty(body))
                body = Request.Form["body"];
            if (String.IsNullOrEmpty(type))
                type = Request.Form["type"];

            int indexint = -1;
            int fromInt = -1;
            if (!int.TryParse(index, out indexint))
            {
                return Content(Error.Create("Index could not be parsed"));
            }

            if (!int.TryParse(from, out fromInt))
            {
                return Content(Error.Create("Index could not be parsed"));
            }
            #endregion

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT profilePicture FROM Users WHERE id=" + indexint;
            DataTable table = ConnectionManager.CreateQuery(command);
            if(table.Rows.Count <= 0)
            {
                return Content(Error.Create("Index does not exist."));
            }

            NotificationManager.SendNotification(fromInt, indexint, body, (NotificationManager.Type) Enum.Parse(typeof(NotificationManager.Type), type, true));
            NotificationManager.CreateNotification(fromInt, indexint, table.Rows[0].Field<string>("profilePicture"), body, (NotificationManager.Type)Enum.Parse(typeof(NotificationManager.Type), type, true));

            return Content(Success.Create("Sent notification"));
        }

        // GET: FindNotifications
        // POST: FindNotifications
        public ActionResult FindNotifications()
        {
            string indexQuery = Request.QueryString["index"];
            if (String.IsNullOrEmpty(indexQuery))
                indexQuery = Request.Form["index"];

            string protestQuery = Request.QueryString["protestIndex "];
            if (String.IsNullOrEmpty(protestQuery))
                protestQuery = Request.Form["protestIndex "];

            string nameQuery = Request.QueryString["searchString"];
            if (String.IsNullOrEmpty(nameQuery))
                nameQuery = Request.Form["name"];

            if (String.IsNullOrEmpty(nameQuery))
                nameQuery = "";

            if (!String.IsNullOrEmpty(indexQuery) && Regex.IsMatch(indexQuery, @"^([0-9]+,?)+$"))
            {
                SqlCommand command = new SqlCommand();
                if (!String.IsNullOrEmpty(indexQuery) && Regex.IsMatch(indexQuery, @"^([0-9]+,?)+$"))
                {
                    if(String.IsNullOrEmpty(nameQuery))
                    {
                        command.CommandText = "SELECT TOP 500 * FROM Notifications WHERE targetIndex IN (" + indexQuery + ")  AND type='Protest' AND text LIKE '%' + @nameQuery + '%' ORDER BY id DESC";
                        command.Parameters.AddWithValue("@nameQuery", nameQuery);

                        command.CommandText = "SELECT TOP 500 * FROM Notifications WHERE targetIndex IN (" + indexQuery + ")  AND type='Protest' ORDER BY id DESC";
                    }
                }

                if (String.IsNullOrEmpty(nameQuery))
                {
                    command.CommandText = "SELECT TOP 500 * FROM Notifications WHERE userIndex IN (" + indexQuery + ") AND type='Follow' ORDER BY id DESC";
                }
                else
                {
                    command.CommandText = "SELECT TOP 500 * FROM Notifications WHERE userIndex IN (" + indexQuery + ")  AND type='Follow' AND text LIKE '%' + @nameQuery + '%' ORDER BY id DESC";
                    command.Parameters.AddWithValue("@nameQuery", nameQuery);
                }
                DataTable table = ConnectionManager.CreateQuery(command);

                if (table.Rows.Count <= 0)
                {
                    return Content(Error.Create("Index does not exist"));
                }
                else
                {
                    NotificationModel[] users = table.Select().Select(s => new NotificationModel(s)).ToArray();
                    string result = JsonConvert.SerializeObject(users);
                    return Content(result);
                }
            }
            return Content(Error.Create("Invalid request"));
        }

        // GET: FindNotificationsCount
        // POST: FindNotificationsCount
        public ActionResult FindNotificationCount()
        {
            string indexQuery = Request.QueryString["index"];
            if (String.IsNullOrEmpty(indexQuery))
                indexQuery = Request.Form["index"];

            string protestQuery = Request.QueryString["protestIndex "];
            if (String.IsNullOrEmpty(protestQuery))
                protestQuery = Request.Form["protestIndex "];

            if (!String.IsNullOrEmpty(indexQuery) && Regex.IsMatch(indexQuery, @"^([0-9]+,?)+$"))
            {
                int protests = 0;
                SqlCommand command = new SqlCommand();
                if (!String.IsNullOrEmpty(protestQuery) && Regex.IsMatch(protestQuery, @"^([0-9]+,?)+$"))
                {
                    command = new SqlCommand();
                    command.CommandText = "SELECT COUNT(*) FROM Notifications WHERE targetIndex IN (" + protestQuery + ") AND type='Protest'";
                    protests = ConnectionManager.CreateScalar(command);
                }
                command.CommandText = "SELECT COUNT(*) FROM Notifications WHERE userIndex IN (" + indexQuery + ") AND type='Follow'";
                int response = ConnectionManager.CreateScalar(command);
                if (response <= 0)
                {
                    return Content(Error.Create("Index does not exist"));
                }
                else
                {
                    return Content(SuccessCreation.Create("Successfully got the count", response + protests));
                }
            }
            return Content(Error.Create("Invalid request"));
        }
    }
}