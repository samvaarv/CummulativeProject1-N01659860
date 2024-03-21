using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;
using CummulativeProject.Models;
using MySql.Data.MySqlClient;
using Mysqlx.Datatypes;

namespace CummulativeProject.Controllers
{
    // Controller class for handling teacher data-related action
    public class TeacherDataController : ApiController
    {
        // Access the school database context
        private SchoolDbContext SchoolDbContext = new SchoolDbContext();

        /// <summary>
        /// Retrieves a list of teachers from the database which match the search key
        /// </summary>
        /// <param name="NameKey">The name of the teacher to search for (optional).</param>
        /// <param name="HireDateKey">The hire date of the teacher to search for (optional).</param>
        /// <param name="SalaryKey">The salary of the teacher to search for (optional).</param>
        /// <returns>
        /// A list of teachers matching the specified criteria.
        /// </returns>
        /// <example>
        /// GET ../api/teacherdata/listteachers/alexander -> [{"TeacherId":"1", "TeacherFname":"Alexander", "TeacherLname":"Bennett", "EmployeeNumber":"T301", "HireDate":"2016-08-05", "Salary":"55.30"}, 
        ///         {"TeacherId":"2", "TeacherFname":"Alexander", "TeacherLname":"Humming", "EmployeeNumber":"T303", "HireDate":"2017-06-15", "Salary":"57.20"},
        ///         {"TeacherId":"3", "TeacherFname":"Alexander", "TeacherLname":"Smith", "EmployeeNumber":"T302", "HireDate":"2016-09-24", "Salary":"25.30"},]
        /// </example>
        [HttpGet]
        [Route("api/teacherdata/Listteachers/{NameKey}")]
        public List<Teacher> ListTeachers(string NameKey = null, DateTime? HireDateKey = null, decimal? SalaryKey = null)
        {
            // Create connection to database    
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to access the information
            string query = "SELECT * FROM teachers WHERE 1=1";
            if (!string.IsNullOrEmpty(NameKey))
            {
                query += $" AND (LOWER(teacherfname) LIKE LOWER('%{NameKey}%') OR LOWER(teacherlname) LIKE LOWER('%{NameKey}%'))";
            }
            if (HireDateKey != null)
            {
                query += $" AND hiredate >= '{HireDateKey.Value.ToString("yyyy-MM-dd")}'";
            }

            if (SalaryKey != null)
            {
                query += $" AND salary >= {SalaryKey}";
            }
            //Set the command text to the SQL query
            Cmd.CommandText = query;
            Cmd.Prepare();

            // Execute the command and retrieve the result set
            MySqlDataReader ResultSet = Cmd.ExecuteReader();

            List<Teacher> Teachers = new List<Teacher>();

            while (ResultSet.Read())
            {
                // Retrieve data from each row of the result set
                int TeacherId = Convert.ToInt32(ResultSet["teacherid"]);
                string TeacherFname = ResultSet["teacherfname"].ToString();
                string TeacherLname = ResultSet["teacherlname"].ToString();
                string EmployeeNumber = ResultSet["employeenumber"].ToString();
                DateTime HireDate = Convert.ToDateTime(ResultSet["hiredate"]);
                decimal Salary = Convert.ToDecimal(ResultSet["salary"]);

                // Create a new Teacher object and populate it with data
                Teacher NewTeacher = new Teacher();
                NewTeacher.TeacherId = TeacherId;
                NewTeacher.TeacherFname = TeacherFname;
                NewTeacher.TeacherLname = TeacherLname;
                NewTeacher.EmployeeNumber = EmployeeNumber;
                NewTeacher.HireDate = HireDate;
                NewTeacher.Salary = Salary;

                // Add the new teacher to the list of teachers
                Teachers.Add(NewTeacher);
            }

            // Close the database connection
            Conn.Close();

            // Return the list of teachers
            return Teachers;
        }


        /// <summary>
        /// Retrieves details of a specific teacher from the database based on teacher ID
        /// </summary>
        /// <param name="TeacherId">The unique identifier of the teacher</param>
        /// <returns>
        /// Details of the selected teacher
        /// </returns>
        /// <example>
        /// GET ../api/teacherdata/findteacher/2 -> {"TeacherId":"2", "TeacherFname":"Caitlyn","TeacherLname":"Cummings","EmployeeNumber":"T381","HireDate":"2014-06-10 12:00:00 AM","Salary":"62.77"}
        /// </example>

        [HttpGet]
        [Route("api/teacherdata/findteacher/{TeacherId}")]
       public TeacherViewModel FindTeacher(int TeacherId)
        {
            // Create connection to database  
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to access the information
            string teacherQuery = "SELECT * FROM teachers WHERE teacherid =" + TeacherId;

            //Set the command text to the SQL query
            Cmd.CommandText = teacherQuery;

            // Execute the command and retrieve the result set
            MySqlDataReader TeacherResultSet = Cmd.ExecuteReader();

            // Create a teacher object
            Teacher SelectedTeacher = new Teacher();

            while (TeacherResultSet.Read())
            {
                // Retrieve data from the result set
                SelectedTeacher.TeacherId = Convert.ToInt32(TeacherResultSet["teacherid"]);
                SelectedTeacher.TeacherFname = TeacherResultSet["teacherfname"].ToString();
                SelectedTeacher.TeacherLname = TeacherResultSet["teacherlname"].ToString();
                SelectedTeacher.EmployeeNumber = TeacherResultSet["employeenumber"].ToString();
                SelectedTeacher.HireDate = Convert.ToDateTime(TeacherResultSet["hiredate"]);
                SelectedTeacher.Salary = Convert.ToDecimal(TeacherResultSet["salary"]);
            }

            TeacherResultSet.Close();

            // Query to retrieve classes taught by the teacher
            string classQuery = "SELECT * FROM classes WHERE teacherid =" + TeacherId;
            Cmd.CommandText = classQuery;

            // Execute the command and retrieve the result set
            MySqlDataReader ClassResultSet = Cmd.ExecuteReader();

            // Create a list to store classes taught by the teacher
            List<Class> ClassesTaught = new List<Class>();

            while (ClassResultSet.Read())
            {
                // Retrieve data from each row of the result set
                int ClassId = Convert.ToInt32(ClassResultSet["classid"]);
                string ClassName = ClassResultSet["classname"].ToString();
                string ClassCode = ClassResultSet["classcode"].ToString();
                DateTime StartDate = Convert.ToDateTime(ClassResultSet["startdate"]);
                DateTime FinishDate = Convert.ToDateTime(ClassResultSet["finishdate"]);

                // Create a new Class object and populate it with data
                Class NewClass = new Class();
                NewClass.ClassId = ClassId;
                NewClass.ClassName = ClassName;
                NewClass.ClassCode = ClassCode;
                NewClass.StartDate = StartDate;
                NewClass.FinishDate = FinishDate;

                // Add the class to the list of classes taught by the teacher
                ClassesTaught.Add(NewClass);
            }

            ClassResultSet.Close();

            // Close the database connection
            Conn.Close();

            // Create and return the ViewModel with teacher and classes taught
            TeacherViewModel TeacherViewModel = new TeacherViewModel
            {
                Teacher = SelectedTeacher,
                ClassesTaught = ClassesTaught
            };

            return TeacherViewModel;
        }

    }
}