using ProtestBackend.Models;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Text;
using System.IO;
using System.Data.SqlClient;

namespace ProtestBackend.DAL
{
    public class NotificationManager
    {
        private const string ONESIGNALID =  "OneSignalId";
        private const string ONESIGNALAPI = "OneSignalAPI";

        public enum Type { Follow, Protest };

        public static void SendNotification(int from, int targetId, string body, Type type)
        {
            #region One Signal
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic " + WebConfigurationManager.AppSettings[ONESIGNALAPI]);

            byte[] byteArray = Encoding.UTF8.GetBytes("{"
                                                    + "\"app_id\": \"" + WebConfigurationManager.AppSettings[ONESIGNALID] + "\","
                                                    + "\"contents\": {\"en\": \"" + body + "\"},"
                                                    + "\"filters\": [{\"field\": \"tag\", \"key\": \"identification\", \"relation\": \"=\", \"value\": \"" + targetId.ToString() + "\"}]}");

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            #endregion

            #region Database
            SqlCommand command = new SqlCommand();
            #endregion
        }
    }
}