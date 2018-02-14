using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.SchoolViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Instructors
{
    public class InstructorCoursePageModel : PageModel
    {
        public List<AssignedCourseViewModel> AssignedCourses;
        public async Task PopulateAssignableCourses(SchoolContext context, Instructor instructor)
        {
            var courses = await context.Course.ToListAsync();
            var instructorCourses = new HashSet<int>(
                instructor.CourseAssignments.Select(c => c.CourseID));
            AssignedCourses = new List<AssignedCourseViewModel>();
            foreach (var course in courses)
            {
                AssignedCourses.Add(new AssignedCourseViewModel
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
        }

        public async Task UpdateInstructorCourses(SchoolContext context, string[] selectedCourses,
            Instructor instructor)
        {
            if (selectedCourses == null)
            {
                instructor.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedCourse = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructor.CourseAssignments.Select(c => c.Course.CourseID));

            foreach (var course in context.Course)
            {
                if (selectedCourse.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructor.CourseAssignments.Add(new CourseAssignment
                        {
                            InstructorID = instructor.ID,
                            CourseID = course.CourseID
                        });
                    }
                }

                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        var courseAssignment = instructor.CourseAssignments.FirstOrDefault(
                            i => i.CourseID == course.CourseID);

                        context.Remove(course);
                    }
                }
            }

            await Task.CompletedTask;
        }
    }
}
