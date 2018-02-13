using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly SchoolContext _context;

        public IndexModel(SchoolContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public IList<Student> Student { get;set; }

        public async Task OnGetAsync(string sortOrder)
        {
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            var student = from s in _context.Student
                          select s;
            switch (sortOrder)
            {
                case "name_desc":
                    student = student.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    student = student.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    student = student.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    student = student.OrderBy(s => s.LastName);
                    break;
            }
            Student = await student.AsNoTracking().ToListAsync();
        }
    }
}
