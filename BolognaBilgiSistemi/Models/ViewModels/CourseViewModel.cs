using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BolognaBilgiSistemi.Models.ViewModels
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string DepartmentName { get; set; } // Bölüm ismi
        public int SelectedFacultyMemberId { get; set; }
    }
}
