
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{
    public class SuccessCoordinates
    {
        public static string Create(string message, string address, float x, float y)
        {
            return (new SuccessCoordinates(message, address, x, y)).ToString();
        }

        public string success;
        public string address;
        public float x;
        public float y;

        public SuccessCoordinates(string successMessage, string address, float x, float y)
        {
            this.x = x;
            this.y = y;
            this.address = address;
            success = successMessage;
        }

        public new string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}