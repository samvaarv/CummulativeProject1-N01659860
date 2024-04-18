using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CummulativeProject.Models;

namespace CummulativeProject.Controllers
{
    public class ClassController : Controller
    {
        /// <summary>
        /// Action method for displaying a list of classes.
        /// </summary>
        /// <returns>
        /// View displaying a list of classes
        /// </returns>
        /// <example>
        /// GET: Class/List -> List of classes
        /// </example>
        public ActionResult List()
        {           

            // Initialize a list of classes
            List<Class> Classes = new List<Class>();

            // Create an instance of ClassDataController to interact with the data
            ClassDataController Controller = new ClassDataController();

            // Retrieve the list of classes from the data controller
            Classes = Controller.ListClasses();

            // Pass the list of classes to the view for display.
            return View(Classes);
        }

        /// <summary>
        /// Displays the details of a specific class.
        /// </summary>
        /// <param name="id">The ID of the class to display.</param>
        /// <returns>The view displaying the details of the specified class.</returns>
        /// <example>
        /// GET: /Class/Show/1 -> a webpage containing information of that particular class
        /// </example>
        public ActionResult Show(int id)
        {
            // Create an instance of ClassDataController to interact with the data
            ClassDataController ClassController = new ClassDataController();

            // Retrieve the class information
            Class ClassInfo = ClassController.FindClass(id);

            // Create an instance of TeacherDataController to interact with the data
            TeacherDataController TeacherController = new TeacherDataController();

            // Retrieve the teacher information
            TeacherViewModel TeacherInfo = TeacherController.FindTeacher(ClassInfo.TeacherId);

            // Pass both class and teacher information to the view
            ViewBag.ClassInfo = ClassInfo;
            ViewBag.TeacherInfo = TeacherInfo;

            return View(ClassInfo);
        }

        /// <summary>
        /// Adds a teacher to the specified class.
        /// </summary>
        /// <param name="classId">The ID of the class to which the teacher will be added.</param>
        /// <param name="teacherId">The ID of the teacher to be added.</param>
        /// <returns>Redirects to the Show action for the class.</returns>
        [HttpPost]
        public ActionResult AddTeacher(int classId, int teacherId)
        {
            // Call the data controller to add the teacher to the class
            ClassDataController ClassDataController = new ClassDataController();
            ClassDataController.AddOrUpdateTeacher(classId, teacherId);

            // Redirect back to the Show action for the class
            return RedirectToAction("Show", new { id = classId });
        }

        /// <summary>
        /// Deletes the teacher from the specified class.
        /// </summary>
        /// <param name="classId">The ID of the class from which the teacher will be deleted.</param>
        /// <returns>Redirects to the Show action for the class.</returns>
        [HttpPost]
        public ActionResult DeleteTeacher(int classId)
        {
            // Call the data controller to delete the teacher from the class
            ClassDataController ClassDataController = new ClassDataController();
            ClassDataController.DeleteTeacherFromClass(classId);

            // Redirect back to the Show action for the class
            return RedirectToAction("Show", new { id = classId });
        }

    }
}