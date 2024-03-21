using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CummulativeProject.Models
{
    public class Student
    {
        // Unique identifier for the student
        public int StudentId { get; set; }

        // First name of the student
        public string StudentFname { get; set; }

        // Last name of the student
        public string StudentLname { get; set; }

        // Student number of the student
        public string StudentNumber { get; set; }

        // Date when the student was enrolled
        public DateTime EnrolDate { get; set; }

        // Salary of the student
        public decimal Salary { get; set; }
    }
}