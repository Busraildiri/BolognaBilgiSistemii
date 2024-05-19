using BolognaBilgiSistemi.Data;
using BolognaBilgiSistemi.Models;
using BolognaBilgiSistemi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

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
                                        .FirstOrDefault(a => a.TCNumber == model.TCNumber && a.Password == model.Password);
                    if (admin != null)
                    {
                        // İdareci için giriş başarılı, session'a kullanıcı adını kaydet ve idareci düzenleme sayfasına yönlendir
                        HttpContext.Session.SetString("UserName", admin.FirstName + " " + admin.LastName);
                        return RedirectToAction("AssignCourses", "Department", new { departmentId = admin.DepartmentId }); // Adminin bölümüne göre yönlendir
                    }
                }
                else if (model.UserType == "Faculty")
                {
                    var faculty = _context.FacultyMembers
                                          .FirstOrDefault(f => f.TCNumber == model.TCNumber && f.Password == model.Password);
                    if (faculty != null)
                    {
                        // Öğretim elemanı için giriş başarılı, session'a kullanıcı adını kaydet ve öğretim elemanı düzenleme sayfasına yönlendir
                        HttpContext.Session.SetString("UserName", faculty.FirstName + " " + faculty.LastName);
                        return RedirectToAction("EditFaculty", "User");
                    }
                }

                // Giriş başarısız, JavaScript alert ile hata mesajı göster
                return Content("<script language='javascript' type='text/javascript'>alert('Kimlik numarasi veya sifre hatali.'); window.location.href = '/User/Login?userType=' + model.UserType;</script>", "text/html");
            }

            return View(model);
        }

        public IActionResult EditAdmin()
        {
            // İdareciler için düzenleme sayfası
            return View();
        }

        public IActionResult EditFaculty()
        {
            // Öğretim elemanları için düzenleme sayfası
            return View();
        }
    }
}
