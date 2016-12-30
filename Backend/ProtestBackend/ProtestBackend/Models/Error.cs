using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{
    public class Error
    {
        public string error;

        public Error(string errorMessage)
        {
            error = errorMessage;
        }
    }
}