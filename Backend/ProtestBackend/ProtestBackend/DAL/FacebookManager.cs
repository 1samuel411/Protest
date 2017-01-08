using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ProtestBackend.DAL
{
    public class FacebookManager
    {
        private const string APPID = "FacebookAppId";   // App token string name from Web.config

        private const string URL = "https://graph.facebook.com/me?access_token=";   // URL to use to check for tokens

        // Return the facebook user Id
        public static string ValidateSession(string sessionToken)
        {
            string response = ConnectionManager.GetResponseText(URL + sessionToken);
            if (String.IsNullOrEmpty(response))
                return "";

            JavaScriptSerializer js = new JavaScriptSerializer();
            FacebookUser user = js.Deserialize<FacebookUser>(response);

            return user.id;
        }
    }

    public class FacebookUser
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}