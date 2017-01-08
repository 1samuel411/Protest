using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{
    public class SuccessCreation
    {
        public static string Create(string message, int index)
        {
            return (new SuccessCreation(message, index)).ToString();
        }

        public string success;
        public int index;

        public SuccessCreation(string successMessag0e, int index)
        {
            this.index = index;
            success = successMessag0e;
        }

        public new string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}