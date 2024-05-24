using BolognaBilgiSistemi.Data;
using BolognaBilgiSistemi.Models;
using BolognaBilgiSistemi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BolognaBilgiSistemi.Controllers
{
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ViewCourses(int facultyId)
        {
            var faculty = await _context.FacultyMembers
                                  .Include(f => f.CourseAssignments)
                                  .ThenInclude(ca => ca.Course)
                                  .ThenInclude(c => c.Department) // Bölüm bilgisi ekleniyor
                                  .FirstOrDefaultAsync(f => f.Id == facultyId);

            if (faculty == null)
            {
                return NotFound();
            }

            var model = new FacultyCoursesViewModel
            {
                FacultyName = faculty.FirstName + " " + faculty.LastName,
                Courses = faculty.CourseAssignments.Select(ca => new CourseViewModel
                {
                    CourseId = ca.Course.CourseId,
                    Name = ca.Course.Name,
                    DepartmentName = ca.Course.Department.Name // DepartmentName bilgisi ekleniyor
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(int courseId)
        {
            var course = await _context.Courses
                                 .Include(c => c.CourseAssignments)
                                 .ThenInclude(ca => ca.FacultyMember)
                                 .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> EditCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                return RedirectToAction("ViewCourses", new { facultyId = HttpContext.Session.GetInt32("FacultyId") });
            }

            return View(course);
        }
    }
}
