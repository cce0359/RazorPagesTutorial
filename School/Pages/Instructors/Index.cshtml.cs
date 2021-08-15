using School.Models;
using School.Models.SchoolViewModels;  // Add VM
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace School.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly School.Data.SchoolContext _context;

        public IndexModel(School.Data.SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexData InstructorData { get; set; }
        public int InstructorID { get; set; }
        public int caID { get; set; }
        public int CourseID { get; set; }

        public async Task OnGetAsync(int? id, int? courseID)
        {
            InstructorData = new InstructorIndexData();
            InstructorData.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)

                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                        .ThenInclude(c => c.Department)

                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                        .ThenInclude(c => c.Enrollments)
                            .ThenInclude(e => e.Student)
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null)
            {
                InstructorID = id.Value;
                Instructor instructor = InstructorData.Instructors
                    .Where(i => i.ID == id.Value).Single();
                InstructorData.CourseAssignments = instructor.CourseAssignments;
            }

            if (courseID != null)
            {
                CourseID = courseID.Value;
                var selectedCourse = InstructorData.CourseAssignments
                    .Where(x => x.CourseID == courseID).Single();
                await _context.Entry(selectedCourse).Reference(x => x.Course)
                              //.Collection(x => x.Course)
                              .Query()
                              .Include(c => c.Enrollments)
                                .ThenInclude(e => e.Student)
                              .LoadAsync();
                foreach (Enrollment enrollment in selectedCourse.Course.Enrollments)
                {
                    await _context.Entry(enrollment).Reference(x => x.Student).LoadAsync();
                }
                InstructorData.Enrollments = selectedCourse.Course.Enrollments;
            }
        }
    }
}