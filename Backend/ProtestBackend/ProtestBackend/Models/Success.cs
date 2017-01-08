using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{
    public class Success
    {
        public static string Create(string message)
        {
            return (new Success(message)).ToString();
        }

        public string success;

        public Success(string successMessage)
        {
            success = successMessage;
        }

        public new string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}