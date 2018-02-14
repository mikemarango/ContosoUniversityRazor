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

        public async Task OnGetAsync(int? id)
        {
            InstructorVM.Instructors = await _context.Instructor
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                .ThenInclude(i => i.Course).AsNoTracking()
                .OrderBy(i => i.LastName)
                .ToListAsync();

            if (id != null) InstructorID = id.Value;
        }
    }
}
