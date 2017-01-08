using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{
    public class Error
    {
        public static string Create(string message)
        {
            return (new Error(message)).ToString();
        }

        public string error;

        public Error(string errorMessage)
        {
            error = errorMessage;
        }

        public new string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}