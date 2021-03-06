﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace ProtestBackend.DAL
{
    public class ConnectionManager
    {
        private const string SERVERNAMEFIELD = "Database";  // Connection string name from Web.config
        
        public static SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(WebConfigurationManager.ConnectionStrings[SERVERNAMEFIELD].ConnectionString);

            connection.Open();

            return connection;
        }

        public static DataTable CreateQuery(string request, SqlConnection connection = null)
        {
            if (connection == null)
                connection = OpenConnection();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(request, connection);
            adapter.Fill(table);
            adapter.Dispose();
            connection.Close();
            return table;
        }

        public static DataTable CreateQuery(SqlCommand request, SqlConnection connection = null)
        {
            if (connection == null)
                connection = OpenConnection();
            request.Connection = connection;
            DataTable table = new DataTable();
            SqlDataAdapter reader = new SqlDataAdapter(request);
            reader.Fill(table);
            reader.Dispose();
            connection.Close();
            return table;
        }

        public static int CreateCommand(string command, SqlConnection connection = null)
        {
            if (connection == null)
                connection = OpenConnection();

            SqlCommand updateCommand = new SqlCommand(command, connection);
            int response = updateCommand.ExecuteNonQuery();

            connection.Close();
            return response;
        }

        public static int CreateCommand(SqlCommand command, SqlConnection connection = null)
        {
            if (connection == null)
                connection = OpenConnection();

            command.Connection = connection;
            int response = command.ExecuteNonQuery();

            connection.Close();
            return response;
        }

        public static int CreateScalar(SqlCommand command, SqlConnection connection = null)
        {
            if (connection == null)
                connection = OpenConnection();

            command.Connection = connection;
            int response = Convert.ToInt32(command.ExecuteScalar());

            connection.Close();
            return response;
        }

        public static bool CheckExistance(string tableName, string key, string value)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = ("SELECT 1 FROM " + tableName + " WHERE " + key + "=@" + key);
            command = AddParameter(command, key, value);
            return (CreateQuery(command).Rows.Count > 0);
        }

        public static SqlCommand AddProperty(SqlCommand command, string property, string value, bool basic = false)
        {
            command.CommandText += ((command.Parameters.Count <= 0) ? " " : ", ") + (basic ? "" : property + "=") + "@" + property;
            command.Parameters.AddWithValue("@" + property, value);
            return command;
        }

        public static SqlCommand AddProperty(SqlCommand command, string property, int value, bool basic = false)
        {
            command.CommandText += ((command.Parameters.Count <= 0) ? " " : ", ") + (basic ? "" : property + "=") + value;
            return command;
        }

        public static SqlCommand AddParameter(SqlCommand command, string property, string value)
        {
            command.Parameters.AddWithValue("@" + property, value);
            return command;
        }

        public static string GetResponseText(string address)
        {
            var request = (HttpWebRequest)WebRequest.Create(address);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var encoding = Encoding.GetEncoding(response.CharacterSet);

                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new StreamReader(responseStream, encoding))
                        return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                return ex.ToString();
            }
        }
    }
}