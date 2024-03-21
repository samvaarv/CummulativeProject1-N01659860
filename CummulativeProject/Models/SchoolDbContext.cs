using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace CummulativeProject.Models
{
    // Represents a database context for connecting to the school database
    public class SchoolDbContext
    {
        // Gets the username for database authentication
        private static string User { get { return "root"; } }

        // Gets the password for database authentication
        private static string Password { get { return ""; } }

        // Gets the name of the database
        private static string Database { get { return "school"; } }

        // Gets the server name where the database is hosted
        private static string Server { get { return "localhost"; } }

        // Gets the port number for database connection
        private static string Port { get { return "3306"; } }

        // Gets the connection string for connecting to the MySQL database
        protected static string ConnectionString
        {
            get
            {
                // Convert Zero Datetime is a setting that will interpret a 0000-00-00 as null
                // This makes it easier for C# to convert to a proper DateTime type
                return "server = " + Server
                    + "; user = " + User
                    + "; database = " + Database
                    + "; port = " + Port
                    + "; password = " + Password
                    + "; convert zero datetime = True";
            }
        }

        /// <summary>
        /// Accesses the database by creating a MySqlConnection object with the connection string
        /// </summary>
        /// <returns>
        /// MySqlConnection object representing a specific connection to the school database
        /// </returns>
        public MySqlConnection AccessDatabase()
        {
            // Instantiates a MySqlConnection object to establish a connection to the database
            // This connection object should be properly disposed of after use
            // to release resources and avoid memory leaks
            return new MySqlConnection(ConnectionString);
        }
    }
}