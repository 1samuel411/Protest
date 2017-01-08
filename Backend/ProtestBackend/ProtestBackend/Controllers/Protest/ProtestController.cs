using ProtestBackend.Models;
using System;
using System.Collections.Generic;
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
            string protestPicture = Request.QueryString["protestPicture"];
            string name = Request.QueryString["name"];
            string description = Request.QueryString["description"];
            string location = Request.QueryString["location"];
            string date = Request.QueryString["date"];
            string donationsEmail = Request.QueryString["donationsEmail"];
            string donationsTarget = Request.QueryString["donationsTarget"];
            string sessionToken = Request.QueryString["sessionToken"];

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

            if(!float.TryParse(donationsTarget, out donationsTargetFloat))
            {
                return Content(Error.Create("Donations target could not be parsed"));
            }
            #endregion


            return Content("Internal error");
        }
    }
}