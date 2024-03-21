using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CummulativeProject.Models;

namespace CummulativeProject.Controllers
{
    // Controller class for handling teacher related actions.
    public class TeacherController : Controller
    {
        /// <summary>
        /// Action method for displaying a list of teachers.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns>
        /// View displaying a list of teachers
        /// </returns>
        /// <example>
        /// GET: Teacher/List?Name={key} -> List of teachers matched with the key
        /// </example>
        [HttpGet]
        public ActionResult List(string Name, DateTime? HireDate, decimal? Salary)
        {
            // Initialize a list of teachers
            List<Teacher> Teachers = new List<Teacher>();

            // Create an instance of TeacherDataController to interact with the data
            TeacherDataController Controller = new TeacherDataController();

            // Retrieve the list of teachers from the data controller
            Teachers = Controller.ListTeachers(Name, HireDate, Salary);

            // Pass the list of teachers to the view for display.
            return View(Teachers);
        }


        /// <summary>
        /// Action method for displaying details of a specific teacher
        /// </summary>
        /// <param name="id">The unique identifier of the teacher</param>
        /// <returns>
        /// View displaying details of the selected teacher
        /// </returns>
        /// <example>
        /// //GET: Teacher/Show/{id} -> a webpage containing information of that particular teacher
        /// </example>
        public ActionResult Show(int id) 
        {
            TeacherDataController Controller = new TeacherDataController();
            TeacherViewModel ViewModel = Controller.FindTeacher(id);
            return View(ViewModel);
        }
    }
}