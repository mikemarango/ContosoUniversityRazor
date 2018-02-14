using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Courses
{
    public class DepartmentPageModel : PageModel
    {
        public SelectList DepartmentList { get; set; }

        public void PopulateDepartmentList(SchoolContext _context, object selectedDepartment = null)
        {
            var departments = from d in _context.Department
                              orderby d.Name
                              select d;

            DepartmentList = new SelectList(departments.AsNoTracking(),
                "DepartmentID", "Name", selectedDepartment);
        }
    }
}
