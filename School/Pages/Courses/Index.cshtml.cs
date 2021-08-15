using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using School.Data;
using School.Models;

namespace School.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly School.Data.SchoolContext _context;
        private readonly IConfiguration Configuration;

        public IndexModel(School.Data.SchoolContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        public string NameSort { get; set; }
        public string CreditSort { get; set; }
        public string DeptSort { get; set; }
        public string IDSort { get; set; }
        public string CurrentSort { get; set; }
        public PaginatedList<Course> Courses { get;set; }

        public async Task OnGetAsync(string sortOrder, int? pageIndex)
        {

            CurrentSort = sortOrder;
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CreditSort = sortOrder == "Credit" ? "credit_desc" : "Credit";
            DeptSort = sortOrder == "Department" ? "dept_desc" : "Department";
            IDSort = sortOrder == "ID" ? "id_desc" : "ID";

            IQueryable<Course> coursesIQ = from c in _context.Courses.Include(c => c.Department).AsNoTracking()
                                             select c;

            switch (sortOrder)
            {
                case "name_desc":
                    coursesIQ = coursesIQ.OrderByDescending(c => c.Title);
                    break;
                case "Credit":
                    coursesIQ = coursesIQ.OrderBy(c => c.Credits);
                    break;
                case "credit_desc":
                    coursesIQ = coursesIQ.OrderByDescending(c => c.Credits);
                    break;
                case "Department":
                    coursesIQ = coursesIQ.OrderBy(c => c.Department.Name);
                    break;
                case "dept_desc":
                    coursesIQ = coursesIQ.OrderByDescending(c => c.Department.Name);
                    break;
                case "ID":
                    coursesIQ = coursesIQ.OrderBy(c => c.CourseID);
                    break;
                case "id_desc":
                    coursesIQ = coursesIQ.OrderByDescending(c => c.CourseID);
                    break;
                default:
                    coursesIQ = coursesIQ.OrderBy(c => c.Title);
                    break;
            }

            var pageSize = Configuration.GetValue("PageSize", 4);
            Courses = await PaginatedList<Course>.CreateAsync(
                coursesIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
