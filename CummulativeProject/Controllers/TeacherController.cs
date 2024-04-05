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
        public ActionResult List(string Name, DateTime? MinHireDate, DateTime? MaxHireDate, decimal? Salary)
        {
            // Initialize a list of teachers
            List<Teacher> Teachers = new List<Teacher>();

            // Create an instance of TeacherDataController to interact with the data
            TeacherDataController Controller = new TeacherDataController();

            // Retrieve the list of teachers from the data controller
            Teachers = Controller.ListTeachers(Name, MinHireDate, MaxHireDate, Salary);

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

        /// <summary>
        /// Action method for displaying the form to add a new teacher.
        /// </summary>
        /// <returns>
        /// View displaying the form to add a new teacher
        /// </returns>
        public ActionResult New() {
        return View();
        }

        /// <summary>
        /// Action method for processing the form submission to add a new teacher.
        /// </summary>
        /// <param name="TeacherFname">The first name of the new teacher.</param>
        /// <param name="TeacherLname">The last name of the new teacher.</param>
        /// <param name="EmployeeNumber">The employee number of the new teacher.</param>
        /// <param name="HireDate">The hire date of the new teacher.</param>
        /// <param name="Salary">The salary of the new teacher.</param>
        /// <returns>
        /// Redirects to the list of teachers after successfully adding a new teacher.
        /// </returns>
        /// <example>
        /// POST: Teacher/Create
        /// </example>
        [HttpPost]
        public ActionResult Create(string TeacherFname, string TeacherLname, string EmployeeNumber, DateTime HireDate, decimal Salary) {

            // Create a new teacher object with the provided information
            Teacher NewTeacher = new Teacher();
            NewTeacher.TeacherFname = TeacherFname;
            NewTeacher.TeacherLname = TeacherLname;
            NewTeacher.EmployeeNumber = EmployeeNumber;
            NewTeacher.HireDate = HireDate;
            NewTeacher.Salary = Salary;

            // Create an instance of TeacherDataController to interact with the data
            TeacherDataController DataController = new TeacherDataController();

            // Add the new teacher to the database
            DataController.AddTeacher(NewTeacher);

            // Redirect to the list of teachers
            return RedirectToAction("List");
        }

        /// <summary>
        /// Action method for displaying the confirmation page to delete a teacher.
        /// </summary>
        /// <param name="id">The unique identifier of the teacher to delete.</param>
        /// <returns>
        /// View displaying the confirmation page to delete the specified teacher.
        /// </returns>
        /// /// <example>
        /// GET: Teacher/DeleteConfirm/{id} -> A confirmation page to delete the specified teacher
        /// </example>
        public ActionResult DeleteConfirm(int id)
        {
            // Create an instance of TeacherDataController to interact with the data
            TeacherDataController DataController = new TeacherDataController();

            // Retrieve the view model for the specified teacher
            TeacherViewModel SelectedTeacher = DataController.FindTeacher(id);

            // Return the view displaying the confirmation page to delete the specified teacher
            return View(SelectedTeacher);
        }

        /// <summary>
        /// Action method for processing the form submission to delete a teacher.
        /// </summary>
        /// <param name="id">The unique identifier of the teacher to delete.</param>
        /// <returns>
        /// Redirects to the list of teachers after successfully deleting the specified teacher.
        /// </returns>
        /// /// <example>
        /// POST: Teacher/Delete/{id} -> Redirects to the teachers list page
        /// </example>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            // Create an instance of TeacherDataController to interact with the data
            TeacherDataController DataController = new TeacherDataController();

            // Update related records in the classes table
            DataController.UpdateClassesWithDeletedTeacher(id);

            // Delete the specified teacher from the database
            DataController.DeleteTeacher(id);

            // Redirect to the list of teachers
            return RedirectToAction("List");
        }

    }
}