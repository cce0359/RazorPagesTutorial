using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using School.Data;
using School.Models;

namespace School.Pages.Students
{
    public class EditModel : StudentCoursesPageModel
    {
        private readonly School.Data.SchoolContext _context;
        private readonly ILogger<StudentCoursesPageModel> _logger;

        public EditModel(School.Data.SchoolContext context,
                          ILogger<StudentCoursesPageModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public Student Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .Include(s => s.Enrollments)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Student == null)
            {
                return NotFound();
            }

            var enrolls = await _context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Where(e => e.StudentID == Student.ID).ToListAsync();

            PopulateAssignedCourseData(_context, Student);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, List<string> grades, List<int> courseid, string[] selectedCourses)
        {
            var studentToUpdate = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (studentToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "student",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate, s => s.Enrollments))
            {
                for(int i = 0; i < courseid.Count; i++)
                {
                    var thisCourse = studentToUpdate.Enrollments.Single(e => e.CourseID == courseid[i]);
                    switch (grades[i])
                    {
                        case "A": thisCourse.Grade = Grade.A;
                            break;
                        case "B":
                            thisCourse.Grade = Grade.B;
                            break;
                        case "C":
                            thisCourse.Grade = Grade.C;
                            break;
                        case "D":
                            thisCourse.Grade = Grade.D;
                            break;
                        case "F":
                            thisCourse.Grade = Grade.F;
                            break;
                        default:
                            thisCourse.Grade = null;
                            break;
                    }

                    
                }

                UpdateStudentCourses(selectedCourses, studentToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            UpdateStudentCourses(selectedCourses, studentToUpdate);
            PopulateAssignedCourseData(_context, Student);
            return Page();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }

        public void UpdateStudentCourses(string[] selectedCourses,
                                            Student studentToUpdate)
        {
            if (selectedCourses == null)
            {
                studentToUpdate.Enrollments = new List<Enrollment>();
                //instructorToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var studentCourses = new HashSet<int>
                (studentToUpdate.Enrollments.Select(e => e.CourseID));
            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!studentCourses.Contains(course.CourseID))
                    {
                        Enrollment emptyEnr = new Enrollment();
                        emptyEnr.CourseID = course.CourseID;
                        emptyEnr.StudentID = studentToUpdate.ID;
                        emptyEnr.Course = course;
                        emptyEnr.Student = studentToUpdate;
                        _context.Enrollments.Add(emptyEnr);

                        course.Enrollments.Add(emptyEnr);
                        studentToUpdate.Enrollments.Add(emptyEnr);
                    }
                }
                else
                {
                    if (studentCourses.Contains(course.CourseID))
                    {
                        var courseToRemove = studentToUpdate.Enrollments.Single(
                                                        e => e.CourseID == course.CourseID);
                        studentToUpdate.Enrollments.Remove(courseToRemove);
                    }
                }
            }
        }
    }
}
