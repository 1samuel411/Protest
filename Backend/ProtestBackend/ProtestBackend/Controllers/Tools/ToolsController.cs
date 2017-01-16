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
using System.Data.SqlClient;
using System.Data;
using ProtestBackend.DAL;

namespace ProtestBackend.Controllers.Tools
{
    public class ToolsController : Controller
    {
        // GET: Tools
        // POST: Tools
        public ActionResult CreateAtlas()
        {
            string[] iconsString;
            #region GetData
            string icons = Request.QueryString["pictures"];
            if (String.IsNullOrEmpty(icons))
                icons = Request.Form["pictures"];

            iconsString = Parser.ParseStringToStringArray(icons);
            #endregion

            if (iconsString.Length > 32)
                return Content(Error.Create("Maximum amount entered"));

            WebClient client = new WebClient();
            Image img = null;
            MemoryStream stream = null;

            int x = 0, y = 0;

            using (Bitmap bmp = new Bitmap(1024, 512))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    for (int i = 0; i < iconsString.Length; i++)
                    {
                        stream = new MemoryStream(client.DownloadData(iconsString[i]));
                        img = Image.FromStream(stream);
                        g.DrawImage(img, x, y, 128, 128);
                        stream.Close();
                        x += 128;
                        if (x >= 1024)
                        {
                            y += 128;
                            x = 0;
                        }
                    }
                }

                MemoryStream ms = new MemoryStream();

                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                return File(ms.ToArray(), "image/jpeg");
            }
        }

        // POST: Set Icon
        public ActionResult UploadIcon()
        {
            #region Get Data
            string sessionToken = Request.QueryString["sessionToken"];
            if (String.IsNullOrEmpty(sessionToken))
                sessionToken = Request.Form["sessionToken"];
            #endregion

            if (Request.Files.Count <= 0)
            {
                return Content("Invalid request");
            }

            if (String.IsNullOrEmpty(sessionToken))
            {
                return Content("Invalid request");
            }

            SqlCommand command = new SqlCommand("SELECT id FROM Users WHERE sessionToken=@sessionToken");
            command.Parameters.AddWithValue("@sessionToken", sessionToken);

            // Check that the user we are going to modify exists
            DataTable user = ConnectionManager.CreateQuery(command);
            if (user.Rows.Count <= 0)
            {
                return Content(Error.Create("SessionToken does not exist"));
            }

            string url = StorageManager.UploadToStorage(Request.Files[0], "icons");
            return Content(SuccessUrl.Create("Successfully uploaded icon", url));
        }
    }
}