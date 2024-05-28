using BolognaBilgiSistemi.Data;
using BolognaBilgiSistemi.Models;
using BolognaBilgiSistemi.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BolognaBilgiSistemi.Controllers
{
    [Authorize]
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
                                      .ThenInclude(c => c.Department)
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
                    DepartmentName = ca.Course.Department.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(int courseId)
        {
            var course = await _context.Courses
                                   .Include(c => c.Department)
                                   .Include(c => c.CourseAssignments)
                                   .ThenInclude(ca => ca.FacultyMember)
                                   .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new CourseViewModel
            {
                CourseId = course.CourseId,
                Name = course.Name,
                DepartmentName = course.Department.Name,
                FacultyMemberName = course.CourseAssignments.FirstOrDefault()?.FacultyMember.FirstName + " " + course.CourseAssignments.FirstOrDefault()?.FacultyMember.LastName,
                SourceBooks = course.SourceBooks,
                Prerequisites = course.Prerequisites,
                WeeklyContents = string.IsNullOrEmpty(course.Content) ? new List<string>(new string[14]) : course.Content.Split(new[] { ";" }, StringSplitOptions.None).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditCourse(CourseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == viewModel.CourseId);

                if (course == null)
                {
                    return NotFound();
                }

                course.SourceBooks = viewModel.SourceBooks;
                course.Prerequisites = viewModel.Prerequisites;
                course.Content = string.Join(";", viewModel.WeeklyContents);

                _context.Update(course);
                // Loglama için dosya yolu
                string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("CourseId: " + viewModel.CourseId);
                    writer.WriteLine("SourceBooks: " + viewModel.SourceBooks);
                    writer.WriteLine("Prerequisites: " + viewModel.Prerequisites);
                    writer.WriteLine("Content: " + viewModel.WeeklyContents);
                    writer.WriteLine("ModelState: " + ModelState.IsValid);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("ViewCourses", new { facultyId = HttpContext.Session.GetInt32("FacultyId") });
            }

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> CourseDetails(int courseId)
        {
            var course = await _context.Courses
                                       .Include(c => c.Department)
                                       .Include(c => c.CourseAssignments)
                                       .ThenInclude(ca => ca.FacultyMember)
                                       .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new CourseViewModel
            {
                CourseId = course.CourseId,
                Name = course.Name,
                DepartmentName = course.Department.Name,
                FacultyMemberName = course.CourseAssignments.FirstOrDefault()?.FacultyMember.FirstName + " " + course.CourseAssignments.FirstOrDefault()?.FacultyMember.LastName,
                SourceBooks = course.SourceBooks,
                Prerequisites = course.Prerequisites,
                WeeklyContents = string.IsNullOrEmpty(course.Content) ? new List<string>(new string[14]) : course.Content.Split(new[] { ";" }, StringSplitOptions.None).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Faculty")]
        public async Task<IActionResult> SaveCourseDetails(CourseViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var existingCourse = await _context.Courses
                                                   .Include(c => c.Department)
                                                   .Include(c => c.CourseAssignments)
                                                   .FirstOrDefaultAsync(c => c.CourseId == viewModel.CourseId);

                if (existingCourse == null)
                {
                    return NotFound();
                }

                existingCourse.Content = string.Join(";", viewModel.WeeklyContents);
                existingCourse.SourceBooks = viewModel.SourceBooks;
                existingCourse.Prerequisites = viewModel.Prerequisites;

                _context.Update(existingCourse);

                // Loglama için dosya yolu
                string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");

                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine("CourseId: " + viewModel.CourseId);
                    writer.WriteLine("SourceBooks: " + viewModel.SourceBooks);
                    writer.WriteLine("Prerequisites: " + viewModel.Prerequisites);
                    writer.WriteLine("Content: " + viewModel.WeeklyContents);
                    writer.WriteLine("ModelState: " + ModelState.IsValid);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("CourseDetails", new { courseId = viewModel.CourseId });
            }

            return View(viewModel);
        }
    }
}
