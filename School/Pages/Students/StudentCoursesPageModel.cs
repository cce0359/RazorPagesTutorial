using School.Data;
using School.Models;
using School.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace School.Pages.Students
{
    public class StudentCoursesPageModel : PageModel
    {
        public List<AssignedCourseData> AssignedCourseDataList;
        public void PopulateAssignedCourseData(SchoolContext context,
                                               Student student)
        {
            var allCourses = context.Courses;
            var studentCourses = new HashSet<int>(student.Enrollments.Select(s => s.CourseID));
                //instructor.Courses.Select(c => c.CourseID));
            AssignedCourseDataList = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                AssignedCourseDataList.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = studentCourses.Contains(course.CourseID)
                });
            }
        }
    }
}