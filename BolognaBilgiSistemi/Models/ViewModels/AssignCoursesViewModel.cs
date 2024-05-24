using BolognaBilgiSistemi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BolognaBilgiSistemi.Models.ViewModels
{
    public class AssignCoursesViewModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        public int FacultyMemberId { get; set; }

        public int DepartmentId { get; set; }

        public List<CourseViewModel> Courses { get; set; }

        public List<SelectListItem> FacultyMembers { get; set; }
        public string DepartmentName { get; set; } // Bölüm adı
    }
}
