using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BolognaBilgiSistemi.Data;
using BolognaBilgiSistemi.Models;
using BolognaBilgiSistemi.Models.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<IActionResult> AssignCourses(int departmentId)
        {
            var department = await _context.Departments
                                           .Include(d => d.Courses)
                                           .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

            if (department == null)
            {
                return NotFound();
            }

            var facultyMembers = await _context.FacultyMembers.ToListAsync();

            var model = new AssignCoursesViewModel
            {
                DepartmentId = department.DepartmentId ?? 0,
                Courses = department.Courses.Select(c => new CourseViewModel
                {
                    CourseId = c.CourseId,
                    Name = c.Name
                }).ToList(),
                FacultyMembers = facultyMembers.Select(fm => new SelectListItem
                {
                    Value = fm.Id.ToString(),
                    Text = fm.FirstName + " " + fm.LastName
                }).ToList(),
                DepartmentName = department.Name
            };

            return View(model);
        }
    }
}
