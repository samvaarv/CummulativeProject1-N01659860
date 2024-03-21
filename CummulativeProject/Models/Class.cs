using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CummulativeProject.Models
{
    public class Class
    {
        // Property to store the unique identifier of the class
        public int ClassId { get; set; }

        // Property to store the name of the class
        public string ClassName { get; set; }

        // Property to store the code of the class
        public string ClassCode { get; set; }

        // Property to store the unique identifier of the teacher associated with the class
        public int TeacherId { get; set; }

        // Property to store the start date of the class
        public DateTime StartDate { get; set; }

        // Property to store the finish date of the class
        public DateTime FinishDate { get; set; }
    }
}