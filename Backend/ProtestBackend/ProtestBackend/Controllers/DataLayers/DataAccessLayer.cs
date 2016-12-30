using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace ProtestBackend.Controllers.DataLayers
{
    public class DataAccessLayer
    {
        private const string SERVERNAMEFIELD = "ProtestChange"; // Connection string name from Web.config

        public static SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = WebConfigurationManager.ConnectionStrings[SERVERNAMEFIELD].ConnectionString;

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