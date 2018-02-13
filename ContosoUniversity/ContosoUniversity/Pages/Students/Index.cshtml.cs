using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Utils;

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

        public PaginatedList<Student> Student { get;set; }

        public async Task OnGetAsync(string sortOrder, string searchString, int? pageIndex, string currentFilter)
        {
            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
                pageIndex = 1;

            else searchString = currentFilter;

            CurrentFilter = searchString;

            var student = from s in _context.Student
                          select s;

            if (!string.IsNullOrEmpty(searchString))
                student = student.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));

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

            int pageSize = 3;
            Student = await PaginatedList<Student>.CreateAsync(
                student.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
