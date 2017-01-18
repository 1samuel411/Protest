using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{

    public class ChatModel
    {
        public int index;
        public string body;
        public int user;
        public string time;
        public int protest;

        public string picture;

        public ChatModel(DataRow dataTable, bool hide = false)
        {
            this.index = int.Parse(dataTable["id"].ToString());

            this.body = dataTable["body"].ToString();
            this.time = dataTable["time"].ToString();

            this.user = int.Parse(dataTable["user"].ToString());
            this.protest = int.Parse(dataTable["protest"].ToString());

            this.picture = dataTable["picture"].ToString(); 
        }
    }
}