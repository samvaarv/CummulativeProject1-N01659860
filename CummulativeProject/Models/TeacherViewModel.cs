using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CummulativeProject.Models
{
    // ViewModel class for representing teacher information along with the classes they teach
    public class TeacherViewModel
    {
        // Property to hold information of a single teacher
        public Teacher Teacher { get; set; }

        // Property to hold a list of classes taught by the teacher
        public List<Class> ClassesTaught { get; set; }
    }
}