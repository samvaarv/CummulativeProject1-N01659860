using System;
using System.Collections.Generic;
using System.Web.Http;
using CummulativeProject.Models;
using MySql.Data.MySqlClient;

namespace CummulativeProject.Controllers
{
    // Controller class for handling teacher data-related action
    public class ClassDataController : ApiController
    {
        // Access the school database context
        private SchoolDbContext SchoolDbContext = new SchoolDbContext();

        /// <summary>
        /// Retrieves a list of classes from the database
        /// </summary>
        /// <returns>
        /// A list of classes
        /// </returns>
        /// <example>
        /// GET ../api/classdata/ListClasses -> {"ClassId":"1","ClassName":"Web Application Development","ClassCode":"http5101","TeacherId":"1","StartDate":"2018-09-04","FinishDate":"2018-12-14"}
        /// </example>
        [HttpGet]
        [Route("api/classdata/ListClasses")]
        public List<Class> ListClasses()
        {
            // Create connection to database  
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to access the information
            string query = "SELECT * FROM classes";

            //Set the command text to the SQL query
            Cmd.CommandText = query;

            // Execute the command and retrieve the result set
            MySqlDataReader ResultSet = Cmd.ExecuteReader();

            // Create a list of Class object
            List<Class> Classes = new List<Class>();

            while (ResultSet.Read())
            {
                // Retrieve data from each row of the result set
                int ClassId = Convert.ToInt32(ResultSet["classid"]);
                string ClassName = ResultSet["classname"].ToString();
                string ClassCode = ResultSet["classcode"].ToString();
                int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);

                // Create a new Class object and populate it with data
                Class NewClass = new Class();
                NewClass.ClassId = ClassId;
                NewClass.ClassName = ClassName;
                NewClass.ClassCode = ClassCode;
                NewClass.TeacherId = TeacherId;
                NewClass.StartDate = StartDate;
                NewClass.FinishDate = FinishDate;

                // Add the new class to the list of classes
                Classes.Add(NewClass);
            }

            // Close the database connection
            Conn.Close();

            // Return the list of classes
            return Classes;
        }
    }
}
