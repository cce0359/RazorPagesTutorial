using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using School.Data;
using School.Models;

namespace School.Pages.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly School.Data.SchoolContext _context;

        public DetailsModel(School.Data.SchoolContext context)
        {
            _context = context;
        }

        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Course = await _context.Courses.FirstOrDefaultAsync(m => m.CourseID == id);
            Course = await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .Include(c => c.Department)
                .Include(c => c.CourseAssignments)
                .ThenInclude(i => i.Instructor)
                .Include(c => c.CourseAssignments)
                .ThenInclude(i => i.Instructor.OfficeAssignment)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);

            if (Course == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
