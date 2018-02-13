using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContosoUniversity.Data;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages
{
    public class AboutModel : PageModel
    {
        private readonly SchoolContext context;

        public AboutModel(SchoolContext context)
        {
            this.context = context;
        }
        public IList<EnrollmentDateGroup> Student { get; set; }

        public async Task OnGetAsync()
        {
            var stats = from s in context.Student
                        group s by s.EnrollmentDate into dateGroup
                        select new EnrollmentDateGroup()
                        {
                            EnrollmentDate = dateGroup.Key,
                            StudentCount = dateGroup.Count()
                        };
            Student = await stats.AsNoTracking().ToListAsync();
        }

    }
}
