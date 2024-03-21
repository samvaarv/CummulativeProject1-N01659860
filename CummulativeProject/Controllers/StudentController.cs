using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CummulativeProject.Models;

namespace CummulativeProject.Controllers
{
    // Controller class for handling teacher related actions.
    public class StudentController : Controller
    {
        /// <summary>
        /// Action method for displaying a list of teachers.
        /// </summary>
        /// <returns>
        /// View displaying a list of teachers
        /// </returns>
        /// <example>
        /// GET: Student/List -> List of teachers
        /// </example>
        public ActionResult List()
        {
            // Initialize a list of teachers
            List<Student> Students = new List<Student>();

            // Create an instance of StudentDataController to interact with the data
            StudentDataController Controller = new StudentDataController();

            // Retrieve the list of teachers from the data controller
            Students = Controller.ListStudents();

            // Pass the list of teachers to the view for display.
            return View(Students);
        }


        /// <summary>
        /// Action method for displaying details of a specific teacher
        /// </summary>
        /// <param name="id">The unique identifier of the teacher</param>
        /// <returns>
        /// View displaying details of the selected teacher
        /// </returns>
        /// <example>
        /// //GET: Student/Show/{id} -> a webpage containing information of that particular teacher
        /// </example>
        public ActionResult Show(int id)
        {
            StudentDataController Controller = new StudentDataController();
            Student ViewModel = Controller.FindStudent(id);
            return View(ViewModel);
        }
    }
}