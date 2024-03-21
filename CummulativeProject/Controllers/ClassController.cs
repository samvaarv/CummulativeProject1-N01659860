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

    }
}