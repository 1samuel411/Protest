using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ProtestBackend.DAL
{
    public class ConnectionManager
    {
        private const string SERVERNAMEFIELD = "ServerName"; // Server Name from Web.config
        private const string SERVERPORTFIELD = "ServerPort"; // Server Port from Web.config
        private const string SERVERUSERFIELD = "ServerUser"; // Server User from web.config
        private const string SERVERPASSFIELD = "ServerPass"; // Server pass from web.config

        public static SqlConnection OpenConnection()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            SqlConnection connection = new SqlConnection();

            builder.DataSource = WebConfigurationManager.AppSettings[SERVERNAMEFIELD] + "," + WebConfigurationManager.AppSettings[SERVERPORTFIELD];
            builder.UserID = WebConfigurationManager.AppSettings[SERVERUSERFIELD];
            builder.Password = WebConfigurationManager.AppSettings[SERVERPASSFIELD];
            builder.InitialCatalog = "ProtestDevelop";
            connection.ConnectionString = builder.ConnectionString;

            connection.Open();

            return connection;
        }

        public static DataTable MakeRequest(string request, SqlConnection connection = null)
        {
            if (connection == null)
                connection = OpenConnection();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(request, connection);
            adapter.Fill(table);
            connection.Close();
            return table;
        }
    }
}