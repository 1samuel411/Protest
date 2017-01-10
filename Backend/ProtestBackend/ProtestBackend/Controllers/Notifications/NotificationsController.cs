﻿using System;
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

            NotificationManager.SendNotification(fromInt, indexint, body, (NotificationManager.Type) Enum.Parse(typeof(NotificationManager.Type), type, true));

            return Content(Success.Create("Sent notification"));
        }
    }
}