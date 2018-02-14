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

namespace ContosoUniversity.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; }
        //public IList<CourseViewModel> CourseVM { get; set; }

        public async Task OnGetAsync()
        {
            Course = await _context.Course
                .Include(c => c.Department).ToListAsync();

            //CourseVM = await _context.Course.Select
            //    (c => new CourseViewModel
            //    {
            //        CourseID = c.CourseID,
            //        Title = c.Title,
            //        Credits = c.Credits,
            //        DepartmentName = c.Department.Name
            //    }).ToListAsync();
        }
    }
}
