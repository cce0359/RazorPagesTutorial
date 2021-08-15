using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using School.Data;
using School.Models;

namespace School.Pages.Instructors
{
    public class DetailsModel : PageModel
    {
        private readonly School.Data.SchoolContext _context;

        public DetailsModel(School.Data.SchoolContext context)
        {
            _context = context;
        }

        public Instructor Instructor { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instructor = await _context.Instructors.FirstOrDefaultAsync(m => m.ID == id);

            if (Instructor == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
