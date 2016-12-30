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
using ProtestBackend.Controllers.DataLayers;

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
                    DataTable table = DataAccessLayer.MakeRequest("SELECT * FROM Users WHERE id=" + index);
                    if (table.Rows.Count <= 0)
                    {
                        Error error = new Error("Index does not exist");
                        return Content(JsonConvert.SerializeObject(error));
                    }
                    else
                    {
                        UserModel user = new UserModel(table.Rows[0]);
                        string result = JsonConvert.SerializeObject(user);
                        return Content(result);
                    }
                }
                else
                {
                    Error error = new Error("Index could not be parsed into a string");
                    return Content(JsonConvert.SerializeObject(error));
                }
            }
            else if(sessionQuery != "" && sessionQuery != null)
            {
                return Content(sessionQuery);
            }
            else
            {
                Error error = new Error("Invalid request");
                return Content(JsonConvert.SerializeObject(error));
            }
        }

        // POST: User
        public ActionResult Update()
        {
            return Content("HI");
        }
    }
}