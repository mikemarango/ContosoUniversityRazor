using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Instructors
{
    public class CreateModel : InstructorCoursePageModel
    {
        private readonly SchoolContext _context;

        public CreateModel(SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instructor Instructor { get; set; } = new Instructor();

        public async Task<IActionResult> OnGetAsync()
        {
            Instructor.CourseAssignments = new List<CourseAssignment>();
            await PopulateAssignableCourses(_context, Instructor);
            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string[] selectedCourses)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (selectedCourses != null)
            {
                Instructor.CourseAssignments = new List<CourseAssignment>();
                foreach (var course in selectedCourses)
                {
                    var courseAssignment = new CourseAssignment
                    {
                        CourseID = int.Parse(course)
                    };
                    Instructor.CourseAssignments.Add(courseAssignment);
                }
            }

            if (await TryUpdateModelAsync(Instructor, "instructor", 
                i => i.FirstMidName, i => i.LastName,
                i => i.HireDate, i => i.OfficeAssignment))
            {
                _context.Instructor.Add(Instructor);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            await PopulateAssignableCourses(_context, Instructor);
            return Page();
        }
    }
}