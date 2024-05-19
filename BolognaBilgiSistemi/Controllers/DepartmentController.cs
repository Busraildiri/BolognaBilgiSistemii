using BolognaBilgiSistemi.Data;
using BolognaBilgiSistemi.Models;
using BolognaBilgiSistemi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BolognaBilgiSistemi.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Department/AssignCourses
        public async Task<IActionResult> AssignCourses(int departmentId)
        {
            if (departmentId <= 0)
            {
                return BadRequest("Geçersiz departman ID'si.");
            }

            var department = await _context.Departments
                .Include(d => d.Courses)
                .ThenInclude(c => c.CourseAssignments)
                .ThenInclude(ca => ca.FacultyMember)
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

            if (department == null)
            {
                return NotFound($"Departman ID'si {departmentId} ile eşleşen bir kayıt bulunamadı.");
            }

            var facultyMembers = await _context.FacultyMembers
                .Select(f => new
                {
                    f.Id,
                    FullName = f.FirstName + " " + f.LastName
                })
                .ToListAsync();

            var coursesViewModel = department.Courses.Select(c => new CourseViewModel
            {
                CourseId = c.CourseId,
                Name = c.Name,
                SelectedFacultyMemberId = c.CourseAssignments.FirstOrDefault()?.FacultyMemberId
            }).ToList();

            var viewModel = new AssignCoursesViewModel
            {
                Department = department,
                Courses = coursesViewModel,
                FacultyMembers = new SelectList(facultyMembers, "Id", "FullName")
            };

            return View(viewModel);
        }

        // POST: Department/AssignCourses
        [HttpPost]
        public async Task<IActionResult> AssignCourses(AssignCoursesViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var course in model.Courses)
                {
                    var assignment = await _context.CourseAssignments
                        .FirstOrDefaultAsync(ca => ca.CourseId == course.CourseId);

                    if (assignment != null)
                    {
                        assignment.FacultyMemberId = course.SelectedFacultyMemberId ?? 0;
                        _context.Update(assignment);
                    }
                    else if (course.SelectedFacultyMemberId != 0)
                    {
                        _context.CourseAssignments.Add(new CourseAssignment
                        {
                            CourseId = course.CourseId,
                            FacultyMemberId = course.SelectedFacultyMemberId ?? 0
                        });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Redirect to a confirmation page or index
            }

            // Model invalid ise, faculty members listesini yeniden yükleyin ve view'a geri gönderin
            model.FacultyMembers = new SelectList(await _context.FacultyMembers
                .Select(f => new
                {
                    f.Id,
                    FullName = f.FirstName + " " + f.LastName
                })
                .ToListAsync(), "Id", "FullName");

            return View(model);
        }
    }
}
