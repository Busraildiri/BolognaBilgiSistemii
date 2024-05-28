using BolognaBilgiSistemi.Data;
using BolognaBilgiSistemi.Models;
using BolognaBilgiSistemi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BolognaBilgiSistemi.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string userType)
        {
            var model = new LoginViewModel { UserType = userType };
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.UserType == "Administrator")
                {
                    var admin = _context.Administrators
                                        .Include(a => a.Department)
                                        .FirstOrDefault(a => a.TCNumber == model.TCNumber && a.Password == model.Password);
                    if (admin != null)
                    {
                        HttpContext.Session.SetString("UserName", admin.FirstName + " " + admin.LastName);
                        HttpContext.Session.SetString("DepartmentName", admin.Department.Name);
                        HttpContext.Session.SetString("UserType", "Administrator");
                        return RedirectToAction("AssignCourses", "Department", new { departmentId = admin.DepartmentId });
                    }
                }
                else if (model.UserType == "Faculty")
                {
                    var faculty = _context.FacultyMembers
                                          .Include(f => f.Department)
                                          .FirstOrDefault(f => f.TCNumber == model.TCNumber && f.Password == model.Password);
                    if (faculty != null)
                    {
                        HttpContext.Session.SetString("UserName", faculty.FirstName + " " + faculty.LastName);
                        HttpContext.Session.SetString("DepartmentName", faculty.Department.Name);
                        HttpContext.Session.SetString("UserType", "Faculty");
                        HttpContext.Session.SetInt32("FacultyId", faculty.Id); // FacultyId'yi Session'a ekliyoruz
                        return RedirectToAction("ViewCourses", "User", new { facultyId = faculty.Id });
                    }
                }

                return Content("<script language='javascript' type='text/javascript'>alert('Kimlik numarasi veya sifre hatali.'); window.location.href = '/User/Login?userType=' + model.UserType;</script>", "text/html");
            }

            return View(model);
        }

        public async Task<IActionResult> ViewCourses(int facultyId)
        {
            var faculty = await _context.FacultyMembers
                                  .Include(f => f.CourseAssignments)
                                  .ThenInclude(ca => ca.Course)
                                  .ThenInclude(c => c.Department) // Department bilgisi ekleniyor
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
                    DepartmentName = ca.Course.Department != null ? ca.Course.Department.Name : "Bölüm bilgisi yok"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Route("api/user/assigncourse")]
        public JsonResult AssignCourse([FromBody] AssignCoursesViewModel model)
        {
            // Loglama
            System.Diagnostics.Debug.WriteLine("AssignCourse called with model: " + JsonConvert.SerializeObject(model));

            // Öğretim elemanına en fazla 6 ders atanabilmesi kontrolü
            var facultyAssignmentsCount = _context.CourseAssignments
                                                  .Count(ca => ca.FacultyMemberId == model.FacultyMemberId);
            if (facultyAssignmentsCount >= 6)
            {
                return Json(new { success = false, errors = new List<string> { "Bir öğretim elemanına en fazla 6 ders atanabilir." } });
            }

            // Bir derse yalnızca bir öğretim elemanı atanabilmesi kontrolü
            var courseAssignment = _context.CourseAssignments
                                           .FirstOrDefault(ca => ca.CourseId == model.CourseId);
            if (courseAssignment != null)
            {
                return Json(new { success = false, errors = new List<string> { "Bu ders zaten bir öğretim elemanına atanmış." } });
            }

            // Ders ataması
            var assignment = new CourseAssignment
            {
                CourseId = model.CourseId,
                FacultyMemberId = model.FacultyMemberId
            };

            try
            {
                _context.CourseAssignments.Add(assignment);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error while saving assignment: " + ex.Message);
                return Json(new { success = false, errors = new List<string> { ex.Message } });
            }
        }

        [HttpPost]
        public IActionResult AssignCourseForm([FromForm] AssignCoursesViewModel model)
        {
            // Loglama için dosya yolu
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine("CourseId: " + model.CourseId);
                writer.WriteLine("FacultyMemberId: " + model.FacultyMemberId);
                writer.WriteLine("ModelState: " + ModelState.IsValid);
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    writer.WriteLine("Error: " + error.ErrorMessage);
                }
            }

            // Öğretim elemanına en fazla 6 ders atanabilmesi kontrolü
            var facultyAssignmentsCount = _context.CourseAssignments
                                                  .Count(ca => ca.FacultyMemberId == model.FacultyMemberId);
            if (facultyAssignmentsCount >= 6)
            {
                return Json(new { success = false, errors = new List<string> { "Bir öğretim elemanına en fazla 6 ders atanabilir." } });
            }

            // Bir derse yalnızca bir öğretim elemanı atanabilmesi kontrolü
            var courseAssignment = _context.CourseAssignments
                                           .FirstOrDefault(ca => ca.CourseId == model.CourseId);
            if (courseAssignment != null)
            {
                return Json(new { success = false, errors = new List<string> { "Bu ders zaten bir öğretim elemanına atanmış." } });
            }

            // Ders ataması
            if (ModelState.IsValid)
            {
                try
                {
                    var assignment = new CourseAssignment
                    {
                        CourseId = model.CourseId,
                        FacultyMemberId = model.FacultyMemberId
                    };

                    _context.CourseAssignments.Add(assignment);
                    _context.SaveChanges();

                    return RedirectToAction("AssignCourses", "Department", new { departmentId = model.DepartmentId });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errors = new List<string> { ex.Message } });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors });
            }
        }

        [HttpGet]
        public IActionResult GetAssignedCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out int facultyMemberId))
            {
                return Json(new { courses = new List<string>() });
            }

            var facultyMember = _context.FacultyMembers
                                        .Include(fm => fm.CourseAssignments)
                                        .ThenInclude(ca => ca.Course)
                                        .SingleOrDefault(fm => fm.Id == facultyMemberId);

            if (facultyMember == null)
            {
                return Json(new { courses = new List<string>() });
            }

            var courses = facultyMember.CourseAssignments
                                        .Select(ca => new { ca.Course.Name })
                                        .ToList();

            return Json(new { courses });
        }

        [HttpGet]
        public IActionResult GetCourseAssignments()
        {
            var assignments = _context.CourseAssignments
                                      .Include(ca => ca.Course)
                                      .Include(ca => ca.FacultyMember)
                                      .Include(ca => ca.FacultyMember.Department) // Bölüm bilgisi ekleniyor
                                      .Select(ca => new
                                      {
                                          CourseName = ca.Course.Name,
                                          FacultyMemberName = ca.FacultyMember.FirstName + " " + ca.FacultyMember.LastName,
                                          DepartmentName = ca.FacultyMember.Department.Name // Bölüm ismi ekleniyor
                                      })
                                      .ToList();

            System.Diagnostics.Debug.WriteLine("GetCourseAssignments result: " + JsonConvert.SerializeObject(assignments));
            if (assignments.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No assignments found.");
            }


            return Json(new { assignments });
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            return RedirectToAction("Login", "User");
        }
    }
}
