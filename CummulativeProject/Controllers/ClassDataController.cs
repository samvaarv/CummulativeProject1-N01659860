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

                // Check for null value in TeacherId column
                int? TeacherIdNullable = ResultSet["teacherid"] != DBNull.Value ? Convert.ToInt32(ResultSet["teacherid"]) : (int?)null;

                // Explicitly cast nullable int to int, or set it to a default value if it's null
                int TeacherId = TeacherIdNullable.HasValue ? TeacherIdNullable.Value : 0;

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

        /// <summary>
        /// Retrieves information about a single class from the database based on its ID.
        /// </summary>
        /// <param name="classId">The ID of the class to retrieve.</param>
        /// <returns>The class information if found, otherwise null.</returns>
        /// <example>
        /// GET ../api/classdata/findclass/2 -> {"ClassId":"2", "ClassCode":"http5102","TeacherId":"1","StartDate":"2018-09-04","FinishDate":"2018-12-14","ClassName":"Project Management",
        ///     "ClassTeacher":
        ///     [
        ///     {"TeacherId":"2", "TeacherFname":"Caitlyn","TeacherLname":"Cummings","EmployeeNumber":"T381","HireDate":"2014-06-10 12:00:00 AM","Salary":"62.77",},
        ///     ]
        /// }
        /// </example>

        [HttpGet]
        [Route("api/classdata/findclass/{classId}")]
        public Class FindClass(int classId)
        {
            // Create connection to the database
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to retrieve a single class by its ID
            string query = "SELECT * FROM classes WHERE classid = @ClassId";

            // Set the command text to the SQL query
            Cmd.CommandText = query;

            // Add parameters to the command
            Cmd.Parameters.AddWithValue("@ClassId", classId);

            // Execute the command and retrieve the result set
            MySqlDataReader ResultSet = Cmd.ExecuteReader();

            // Initialize a class object to store the result
            Class classInfo = null;

            // Check if there are any rows returned
            if (ResultSet.Read())
            {
                // Retrieve data from the result set
                int ClassId = Convert.ToInt32(ResultSet["classid"]);
                string ClassName = ResultSet["classname"].ToString();
                string ClassCode = ResultSet["classcode"].ToString();
                DateTime StartDate = Convert.ToDateTime(ResultSet["startdate"]);
                DateTime FinishDate = Convert.ToDateTime(ResultSet["finishdate"]);

                // Check if TeacherId is null
                int? TeacherIdNullable = ResultSet["teacherid"] != DBNull.Value ? Convert.ToInt32(ResultSet["teacherid"]) : (int?)null;
                int TeacherId = TeacherIdNullable.HasValue ? TeacherIdNullable.Value : 0;
                string teacherInfo = TeacherId != 0 ? $"Teacher ID: {TeacherId}" : "No teacher assigned";

                // Create a new Class object and populate it with data
                classInfo = new Class
                {
                    ClassId = ClassId,
                    ClassName = ClassName,
                    ClassCode = ClassCode,
                    TeacherId = TeacherId,
                    StartDate = StartDate,
                    FinishDate = FinishDate
                };
            }

            // Close the database connection
            Conn.Close();

            // Return the class information
            return classInfo;
        }

        /// <summary>
        /// Adds or updates the teacher assigned to a class.
        /// </summary>
        /// <param name="classId">The ID of the class.</param>
        /// <param name="teacherId">The ID of the teacher to be assigned.</param>
        /// <returns>
        /// IHttpActionResult indicating the result of the operation:
        /// - Ok if the teacher is added or updated successfully.
        /// - BadRequest if there is an issue with the request or the teacher cannot be added/updated.
        /// - NotFound if the class with the specified ID is not found.
        /// </returns>
        /// <example>
        /// This example demonstrates how to add or update a teacher for a class:
        /// POST ../api/classdata/AddOrUpdateTeacher/1/1 -> This request adds or updates the teacher with ID 101 to the class with ID 1.
        /// </example>

        [HttpPost]
        [Route("api/classdata/AddOrUpdateTeacher/{classId}/{teacherId}")]
        public IHttpActionResult AddOrUpdateTeacher(int classId, int teacherId)
        {
            // Check if the class exists
            Class classInfo = FindClass(classId);
            if (classInfo == null)
            {
                return NotFound(); // Return 404 if class not found
            }

            // Check if teacherId is valid
            if (teacherId <= 0)
            {
                return BadRequest("Invalid teacher ID"); // Return bad request if teacherId is invalid
            }

            // Check if the class already has a teacher assigned
            if (classInfo.TeacherId != 0)
            {
                // Delete the existing teacher from the class
                DeleteTeacherFromClass(classId);
            }

            // Update the class with the new teacherId
            // Create connection to the database
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to update a teacher for the selected class
            string query = "UPDATE classes SET teacherid = @teacherId WHERE classid = @classId";

            // Set the command text to the SQL query
            Cmd.CommandText = query;

            // Add parameters to the command
            Cmd.Parameters.AddWithValue("@teacherId", teacherId);
            Cmd.Parameters.AddWithValue("@classId", classId);

            // Execute the command and retrieve the result set
            int RowsAffected = Cmd.ExecuteNonQuery();
            Conn.Close();

            if (RowsAffected > 0)
            {
                return Ok("Teacher added/updated successfully");
            }
            else
            {
                return BadRequest("Failed to add/update teacher");
            }
        }

        /// <summary>
        /// Deletes the teacher from the specified class.
        /// </summary>
        /// <param name="classId">The ID of the class from which to delete the teacher.</param>
        /// <returns>
        /// IHttpActionResult indicating the result of the operation.
        /// If successful, returns Ok with a message indicating the teacher was deleted successfully.
        /// If the class is not found, returns NotFound.
        /// If the operation fails, returns BadRequest with a message indicating the failure.
        /// </returns>
        /// <example>
        /// This example demonstrates how to call the DeleteTeacherFromClass API endpoint using HTTP DELETE:
        /// DELETE ../api/classdata/DeleteTeacherFromClass/1 -> This request deletes the teacher from the class with ID 1.
        /// </example>

        [HttpGet]
        [Route("api/classdata/DeleteTeacherFromClass/{classId}")]
        public IHttpActionResult DeleteTeacherFromClass(int classId)
        {
            // Check if the class exists
            Class classInfo = FindClass(classId);
            if (classInfo == null)
            {
                return NotFound(); // Return 404 if class not found
            }

            // Update the class to remove the teacher
            // Create connection to the database
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to update a teacher for the selected class
            string query = "UPDATE classes SET teacherid = NULL WHERE classid = @classId";

            // Set the command text to the SQL query
            Cmd.CommandText = query;

            // Add parameters to the command
            Cmd.Parameters.AddWithValue("@classId", classId);

            // Execute the command and retrieve the result set
            int RowsAffected = Cmd.ExecuteNonQuery();
            Conn.Close();

            if (RowsAffected > 0)
            {
                return Ok("Teacher deleted successfully");
            }
            else
            {
                return BadRequest("Failed to delete teacher");
             }
        }

    }
 }
