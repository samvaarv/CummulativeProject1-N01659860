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
        /// <param name="MinHireDateKey">The minimum hire date of the teacher to search for (optional).</param>
        /// <param name="MaxHireDateKey">The maximum hire date of the teacher to search for (optional).</param>
        /// <param name="MinSalaryKey">The minimum salary of the teacher to search for (optional).</param>
        /// <param name="MaxSalaryKey">The maximum salary of the teacher to search for (optional).</param>
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
        public List<Teacher> ListTeachers(string NameKey = null, DateTime? MinHireDateKey = null, DateTime? MaxHireDateKey = null, decimal? MinSalaryKey = null, decimal? MaxSalaryKey = null)
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
            if (MinHireDateKey != null || MaxHireDateKey != null)
            {
                query += $" AND hiredate >= '{MinHireDateKey.Value.ToString("yyyy-MM-dd")}' AND hiredate <= '{MaxHireDateKey.Value.ToString("yyyy-MM-dd")}'";
            }

            if (MinSalaryKey != null || MaxSalaryKey != null)
            {
                query += $" AND salary >= {MinSalaryKey} AND salary <= {MaxSalaryKey}";
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
        /// GET ../api/teacherdata/findteacher/2 -> {"TeacherId":"2", "TeacherFname":"Caitlyn","TeacherLname":"Cummings","EmployeeNumber":"T381","HireDate":"2014-06-10 12:00:00 AM","Salary":"62.77",
        ///     "ClassesTaught":
        ///     [
        ///     {"ClassCode":"HTTP5102","ClassId":"2","ClassName":"Project Management","FinishDate":"2018-12-14T00:00:00","StartDate":"2018-09-04T00:00:00"},
        ///     
        ///     {,"ClassCode":"HTTP5201","ClassId":"6","ClassName":"Security & Quality Assurance","FinishDate":"2019-04-27T00:00:00","StartDate":"2019-01-08T00:00:00"}
        ///     ]
        /// }
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

        //Add Teacher Data
        /// <summary>
        /// Adds a new teacher to the database.
        /// </summary>
        /// <param name="NewTeacher">The Teacher object containing the details of the new teacher to be added.</param>
        /// <example>
        /// POST ../api/teacherdata/addteacher
        /// curl -H "Content-Type: application/json" -d @teacher.json ../api/teacherdata/addteacher
        /// {
        ///     "TeacherFname": "John",
        ///     "TeacherLname": "Doe",
        ///     "EmployeeNumber": "T123",
        ///     "HireDate": "2024-03-16",
        ///     "Salary": 55
        /// }
        /// </example>
        [HttpPost]
        public void AddTeacher([FromBody]Teacher NewTeacher)
        {
            // Establish a connection to the database
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command to execute SQL queries
            MySqlCommand Cmd = Conn.CreateCommand();

            // Define the SQL query to insert a new teacher
            string query = "INSERT INTO teachers (teacherfname, teacherlname, employeenumber, hiredate, salary) VALUES (@teacherfname, @teacherlname, UPPER(@employeenumber), @hiredate, @salary)";
            Cmd.CommandText = query;

            // Set the parameters for the SQL query
            Cmd.Parameters.AddWithValue("@teacherfname", NewTeacher.TeacherFname);
            Cmd.Parameters.AddWithValue("@teacherlname", NewTeacher.TeacherLname);
            Cmd.Parameters.AddWithValue("@employeenumber", NewTeacher.EmployeeNumber);
            Cmd.Parameters.AddWithValue("@hiredate", NewTeacher.HireDate);
            Cmd.Parameters.AddWithValue("@salary", NewTeacher.Salary);
            Cmd.Prepare();

            // Execute the SQL query to insert the new teacher
            Cmd.ExecuteNonQuery();

            // Close the database connection
            Conn.Close();

        }

        /// <summary>
        /// Deletes a teacher from the database based on their unique identifier (TeacherId).
        /// </summary>
        /// <param name="TeacherId">The unique identifier of the teacher to be deleted.</param>
        /// <example>
        /// DELETE ../api/teacherdata/deleteteacher/3
        /// </example>
        [HttpGet]
        [Route("api/TeacherDate/DeleteTeacher/{TeacherId}")]
        public void DeleteTeacher(int TeacherId)
        {
            // Establish a connection to the database
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Define the SQL query to delete the teacher
            string query = "DELETE FROM teachers WHERE teacherid = @teacherid";

            // Create a command to execute SQL queries
            MySqlCommand Cmd = Conn.CreateCommand();
            Cmd.CommandText = query;

            // Set the parameter for the SQL query
            Cmd.Parameters.AddWithValue("@teacherid", TeacherId);
            Cmd.Prepare();

            // Execute the SQL query to delete the teacher
            Cmd.ExecuteNonQuery();

            // Close the database connection
            Conn.Close();

        }

        /// <summary>
        /// Updates the classes in the database that were previously assigned to the deleted teacher.
        /// Sets the teacher ID to NULL for those classes to maintain referential integrity.
        /// </summary>
        /// <param name="teacherId">The ID of the teacher that has been deleted.</param>
        /// <example>
        /// This method is called after a teacher has been deleted from the system to ensure that
        /// any classes previously assigned to that teacher are no longer associated with them.
        /// </example>
        [HttpPost]
        public void UpdateClassesWithDeletedTeacher(int teacherId)
        {
            // Create a connection to the database
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to update classes with the deleted teacher ID
            string query = "UPDATE classes SET teacherid = NULL WHERE teacherid = @teacherid";
            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@teacherid", teacherId);
            Cmd.Prepare();

            // Execute the SQL command to update the classes
            Cmd.ExecuteNonQuery();

            // Close the database connection
            Conn.Close();
        }

        /// <summary>
        /// Checks if the given employee number is unique in the database.
        /// </summary>
        /// <param name="employeeNumber">The employee number to check for uniqueness.</param>
        /// <returns>True if the employee number is unique; otherwise, false.</returns>
        public bool IsEmployeeNumberUnique(string employeeNumber)
        {
            // Create connection to database    
            MySqlConnection Conn = SchoolDbContext.AccessDatabase();
            Conn.Open();

            // Create a command for SQL query
            MySqlCommand Cmd = Conn.CreateCommand();

            // Query to check if the employee number already exists
            string query = "SELECT COUNT(*) FROM teachers WHERE employeenumber = @EmployeeNumber";
            Cmd.CommandText = query;
            Cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);

            // Execute the command and retrieve the result
            int count = Convert.ToInt32(Cmd.ExecuteScalar());

            // Close the database connection
            Conn.Close();

            // If count > 0, employee number already exists; otherwise, it's unique
            return count == 0;
        }
    }
}