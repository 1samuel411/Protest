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

            string nameQuery = Request.QueryString["searchString"];
            if (String.IsNullOrEmpty(nameQuery))
                nameQuery = Request.Form["name"];

            if (!String.IsNullOrEmpty(indexQuery) && Regex.IsMatch(indexQuery, @"^([0-9]+,?)+$"))
            {
                SqlCommand command = new SqlCommand();
                if (String.IsNullOrEmpty(nameQuery))
                {
                    command.CommandText = "SELECT * FROM Notifications WHERE userIndex IN (" + indexQuery + ")";
                }
                else
                {
                    command.CommandText = "SELECT * FROM Notifications WHERE userIndex IN (" + indexQuery + ") AND text LIKE '%' + @nameQuery + '%'";
                    command.Parameters.AddWithValue("@nameQuery", nameQuery);
                }
                DataTable table = ConnectionManager.CreateQuery(command);
                DataTable outputTable = table.Clone();

                for (int i = table.Rows.Count - 1; i >= 0; i--)
                {
                    outputTable.ImportRow(table.Rows[i]);
                }

                table = outputTable;
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

            if (!String.IsNullOrEmpty(indexQuery) && Regex.IsMatch(indexQuery, @"^([0-9]+,?)+$"))
            {
                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT * FROM Notifications WHERE userIndex IN (" + indexQuery + ")";
                DataTable table = ConnectionManager.CreateQuery(command);
                if (table.Rows.Count <= 0)
                {
                    return Content(Error.Create("Index does not exist"));
                }
                else
                {
                    return Content(SuccessCreation.Create("Successfully got the count", table.Rows.Count));
                }
            }
            return Content(Error.Create("Invalid request"));
        }
    }
}