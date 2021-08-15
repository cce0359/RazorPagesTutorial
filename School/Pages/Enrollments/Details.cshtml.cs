using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using School.Data;
using School.Models;

namespace School.Pages.Enrollments
{
    public class DetailsModel : PageModel
    {
        private readonly School.Data.SchoolContext _context;

        public DetailsModel(School.Data.SchoolContext context)
        {
            _context = context;
        }

        public Enrollment Enrollment { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student).FirstOrDefaultAsync(m => m.EnrollmentID == id);

            if (Enrollment == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
