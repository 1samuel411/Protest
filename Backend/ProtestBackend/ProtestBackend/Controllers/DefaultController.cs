using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProtestBackend.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            return Content("hello there, please check the api, <a href=\"https://github.com/1samuel411/Protest/wiki\"Here</a>");
            //return Redirect("https://github.com/1samuel411/Protest/wiki");
        }
    }
}