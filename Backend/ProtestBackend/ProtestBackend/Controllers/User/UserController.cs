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
using ProtestBackend.Controllers;

namespace ProtestBackend.Controllers.User
{
    public class UserController : Controller
    {

        // GET: User
        public ActionResult Find()
        {
            int index;
            string indexQuery = Request.QueryString["index"];
            string sessionQuery = Request.QueryString["sessionKey"];
            if (indexQuery != "" && indexQuery != null)
            {
                if (int.TryParse(indexQuery, out index))
                {
                    DataTable table = ConnectionManager.MakeRequest("SELECT * FROM Users WHERE id=" + index);
                    UserModel user = new UserModel(table);
                    string result = JsonConvert.SerializeObject(table, Formatting.Indented);
                    return Content(result);
                }
                else
                {
                    Error error = new Error("Index could not be parsed into a string");
                    return Content(JsonConvert.SerializeObject(error, Formatting.Indented));
                }
            }
            else if(sessionQuery != "" && sessionQuery != null)
            {
                return Content(sessionQuery);
            }
            else
            {
                Error error = new Error("Invalid request");
                return Content(JsonConvert.SerializeObject(error, Formatting.Indented));
            }
        }
    }
}