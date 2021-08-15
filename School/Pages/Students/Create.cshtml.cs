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
    public class CreateModel : StudentCoursesPageModel
    {
        private readonly School.Data.SchoolContext _context;
        private readonly ILogger<StudentCoursesPageModel> _logger;

        public CreateModel(School.Data.SchoolContext context,
                          ILogger<StudentCoursesPageModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var student = new Student();
            student.Enrollments = new List<Enrollment>();

            PopulateAssignedCourseData(_context, student);
            return Page();
        }

        [BindProperty]
        public Student Student { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(string[] selectedCourses)
        {

            var newStudent = new Student();

            if (selectedCourses.Length > 0)
            {
                newStudent.Enrollments = new List<Enrollment>();
                // Load collection with one DB call.
                _context.Enrollments.Load();
            }

            // Add selected Courses courses to the new student.
            foreach (var course in selectedCourses)
            {
                var foundCourse = await _context.Courses.FindAsync(int.Parse(course));

                if (foundCourse != null)
                {
                    Enrollment newEnroll = new Enrollment();
                    newEnroll.CourseID = foundCourse.CourseID;
                    newEnroll.StudentID = newStudent.ID;
                    newEnroll.Course = foundCourse;
                    newEnroll.Student = newStudent;
                    _context.Enrollments.Add(newEnroll);

                    foundCourse.Enrollments.Add(newEnroll);
                    newStudent.Enrollments.Add(newEnroll);
                }
                else
                {
                    _logger.LogWarning("Course {course} not found", course);
                }
            }

            try
            {
                if (await TryUpdateModelAsync<Student>(
                                newStudent,
                                "student",
                                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
                {
                    _context.Students.Add(newStudent);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            PopulateAssignedCourseData(_context, Student);
            return Page();
        }
    }
}
