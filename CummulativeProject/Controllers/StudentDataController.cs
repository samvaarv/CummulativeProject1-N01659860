using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CummulativeProject.Models;
using MySql.Data.MySqlClient;

namespace CummulativeProject.Controllers
{
    // Controller class for handling student data-related action
    public class StudentDataController : ApiController
    {
        // Access the school database context
        private SchoolDbContext SchoolDbContext = new SchoolDbContext();

        /// <summary>
        /// Retrieves a list of students from the database
        /// </summary>
        /// <returns>
        /// A list of students
        /// </returns>
        /// <example>
        /// GET ../api/studentdata/liststudents -> ["Alexander Bennett", "Caitlin Cummings"]
        /// </example>
        [HttpGet]
        [Route("api/studentdata/Liststudents")]
        public List<Student> ListStudents()
        {
            // Create connection to database    
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to access the information
            string querry = "SELECT * FROM students";

            //Set the command text to the SQL query
            Cmd.CommandText = querry;

            // Execute the command and retrieve the result set
            MySqlDataReader ResultSet = Cmd.ExecuteReader();

            List<Student> Students = new List<Student>();

            while (ResultSet.Read())
            {
                // Retrieve data from each row of the result set
                int StudentId = Convert.ToInt32(ResultSet["studentid"]);
                string StudentFname = ResultSet["studentfname"].ToString();
                string StudentLname = ResultSet["studentlname"].ToString();
                string StudentNumber = ResultSet["studentnumber"].ToString();
                DateTime EnrolDate = Convert.ToDateTime(ResultSet["enroldate"]);

                // Create a new Student object and populate it with data
                Student NewStudent = new Student();
                NewStudent.StudentId = StudentId;
                NewStudent.StudentFname = StudentFname;
                NewStudent.StudentLname = StudentLname;
                NewStudent.StudentNumber = StudentNumber;
                NewStudent.EnrolDate = EnrolDate;

                // Add the new student to the list of students
                Students.Add(NewStudent);
            }

            // Close the database connection
            Conn.Close();

            // Return the list of students
            return Students;
        }


        /// <summary>
        /// Retrieves details of a specific student from the database based on student ID
        /// </summary>
        /// <param name="StudentId">The unique identifier of the student</param>
        /// <returns>
        /// Details of the selected student
        /// </returns>
        /// <example>
        /// GET ../api/studentdata/findstudent/2 -> {"StudentId":"2", "StudentFname":"Jennifer","StudentLname":"Faulkner","StudentNumber":"N1679","EnrollDate":"2018-08-02 12:00:00 AM"}
        /// </example>

        [HttpGet]
        [Route("api/studentdata/findstudent/{StudentId}")]
        public Student FindStudent(int StudentId)
        {
            // Create connection to database  
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to access the information
            string studentQuery = "SELECT * FROM students WHERE studentid =" + StudentId;

            //Set the command text to the SQL query
            Cmd.CommandText = studentQuery;

            // Execute the command and retrieve the result set
            MySqlDataReader StudentResultSet = Cmd.ExecuteReader();

            // Create a student object
            Student SelectedStudent = new Student();

            while (StudentResultSet.Read())
            {
                // Retrieve data from the result set
                SelectedStudent.StudentId = Convert.ToInt32(StudentResultSet["studentid"]);
                SelectedStudent.StudentFname = StudentResultSet["studentfname"].ToString();
                SelectedStudent.StudentLname = StudentResultSet["studentlname"].ToString();
                SelectedStudent.StudentNumber = StudentResultSet["studentnumber"].ToString();
                SelectedStudent.EnrolDate = Convert.ToDateTime(StudentResultSet["enroldate"]);
            }

            StudentResultSet.Close();

            // Close the database connection
            Conn.Close();

            return SelectedStudent;
        }

    }
}