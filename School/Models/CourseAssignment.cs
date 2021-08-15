﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace School.Models
{
    public class CourseAssignment
    {
        public int CourseAssignmentID { get; set; }
        public int CourseID { get; set; }
        public Course Course { get; set; }
        public int InstructorID { get; set; }
        public Instructor Instructor { get; set; }
    }
}
