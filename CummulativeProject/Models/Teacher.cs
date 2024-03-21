using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CummulativeProject.Models
{
    public class Teacher
    {
        // Unique identifier for the teacher.
        public int TeacherId { get; set; }

        // First name of the teacher.
        public string TeacherFname { get; set; }

        // Last name of the teacher.
        public string TeacherLname { get; set; }

        // Employee number of the teacher.
        public string EmployeeNumber { get; set; }

        // Date when the teacher was hired.
        public DateTime HireDate { get; set; }

        // Salary of the teacher.
        public decimal Salary { get; set; }
    }
}