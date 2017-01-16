
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{
    public class SuccessUrl
    {
        public static string Create(string message, string url)
        {
            return (new SuccessUrl(message, url)).ToString();
        }

        public string success;
        public string url;

        public SuccessUrl(string successMessage, string url)
        {
            this.url = url;
            success = successMessage;
        }

        public new string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}