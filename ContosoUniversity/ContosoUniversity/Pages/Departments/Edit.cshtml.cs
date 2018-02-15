using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly SchoolContext _context;

        public EditModel(SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; }
        public SelectList Instructors { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == null) return NotFound();

            Department = await _context.Department
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentID == id);

            if (Department == null) return NotFound();

            Instructors = new SelectList(_context.Instructor, "ID", "FirstMidName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid) return Page();

            var department = await _context.Department
                .Include(i => i.Administrator)
                .FirstOrDefaultAsync(d => d.DepartmentID == id);

            if (department == null) await HandleDeletedDepartmentAsync();

            // Update the RowVersion to the value when this entity was
            // fetched. If the entity has been updated after it was
            // fetched, RowVersion won't match the DB RowVersion and
            // a DbUpdateConcurrencyException is thrown.
            // A second postback will make them match, unless a new
            // concurrency issue happens.
            _context.Entry(department).Property("RowVersion")
                .OriginalValue = Department.RowVersion;

            if (await TryUpdateModelAsync(department, "Department",
                d => d.Name, d => d.StartDate, d => d.Budget, d => d.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exception = ex.Entries.Single();
                    var clientValues = (Department)exception.Entity;
                    var databaseEntry = exception.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, $"Unable to save. The department was deleted by another user.");
                        return Page();
                    }

                    var dbValues = (Department)databaseEntry.ToObject();
                    await SetDbErrorMessageAsync(dbValues, clientValues, _context);

                    // Save the current RowVersion so the next postback
                    // matches unless a new concurrency issue happens.
                    Department.RowVersion = (byte[])dbValues.RowVersion;
                    // Must clear the model error for the next postback.
                    ModelState.Remove("Department.RowVersion");
                }
            }

            Instructors = new SelectList(_context.Instructor,
                "ID", "FullName", department.InstructorID);

            return RedirectToPage("./Index");
        }

        private async Task<IActionResult> HandleDeletedDepartmentAsync()
        {
            ModelState.AddModelError(string.Empty,
                "Unable to save. The department was deleted by another user.");

            Instructors = new SelectList(_context.Instructor, "ID", "FullName", Department.InstructorID);

            await Task.CompletedTask;

            return Page();
        }

        private async Task SetDbErrorMessageAsync(Department dbValues, Department clientValues, SchoolContext context)
        {
            if (dbValues.Name != clientValues.Name)
                ModelState.AddModelError("Department.Name", $"Current value: {dbValues.Name}");

            if (dbValues.Budget != clientValues.Budget)
                ModelState.AddModelError("Department.Budget", $"Current value: {dbValues.Budget}");

            if (dbValues.StartDate != clientValues.StartDate)
                ModelState.AddModelError("Department.StartDate", $"Current value: {dbValues.StartDate}");

            if (dbValues.InstructorID != clientValues.InstructorID)
            {
                var instructor = await _context
                    .Instructor.FindAsync(dbValues.InstructorID);
                ModelState.AddModelError("Department.InstructorID", $"Current value: {instructor?.FullName}");
            }

            ModelState.AddModelError(string.Empty, 
                "The record edition was unsuccessfull since someone else modified it just before you did. " +
                "If you need to edit please click on save again.");
        }

    }
}
