using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ProtestBackend.Models
{
    public class NotificationModel
    {
        public int index;
        public int userIndex;
        public int targetIndex;
        public string text;
        public string picture;
        public string type;
        public string time;

        public NotificationModel (DataRow dataTable)
        {
            this.index = int.Parse(dataTable["id"].ToString());

            if (dataTable.Table.Columns.Contains("userIndex"))
                this.userIndex = int.Parse(dataTable["userIndex"].ToString());
            if (dataTable.Table.Columns.Contains("targetIndex"))
                this.targetIndex = int.Parse(dataTable["targetIndex"].ToString());
            if (dataTable.Table.Columns.Contains("text"))
                this.text = dataTable["text"].ToString();
            if (dataTable.Table.Columns.Contains("picture"))
                this.picture = dataTable["picture"].ToString();
            if (dataTable.Table.Columns.Contains("type"))
                this.type = dataTable["type"].ToString();
            if (dataTable.Table.Columns.Contains("time"))
                this.time = dataTable["time"].ToString();
        }
    }
}