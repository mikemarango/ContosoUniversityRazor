using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;

namespace ContosoUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public InstructorIndexViewModel InstructorVM { get; set; } = new InstructorIndexViewModel();
        public int InstructorID { get; set; }
        public int CourseID { get; set; }

        public async Task OnGetAsync(int? id, int? courseID)
        {
            InstructorVM.Instructors = await _context.Instructor
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                        .ThenInclude(c => c.Department)
                .Include(i => i.CourseAssignments)
                    .ThenInclude(c => c.Course)
                        .ThenInclude(i => i.Enrollments)
                            .ThenInclude(i => i.Student)
                .AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null)
            {
                InstructorID = id.Value;
                var instructor = InstructorVM.Instructors.Where(
                    i => i.ID == id.Value).Single();
                InstructorVM.Courses = instructor.CourseAssignments.Select(
                    s => s.Course);
            }

            if (courseID != null)
            {
                CourseID = courseID.Value;
                InstructorVM.Enrollments = InstructorVM.Courses.Where(
                    c => c.CourseID == courseID).Single().Enrollments;
            }
        }
    }
}
