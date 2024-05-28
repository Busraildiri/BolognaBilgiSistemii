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
        public string FacultyMemberName { get; set; }
        public List<string> WeeklyContents { get; set; } = new List<string>(new string[14]); // 14 haftalık içerik
        public string SourceBooks { get; set; } // Kaynak Kitaplar
        public string Prerequisites { get; set; } // Ön Koşullar
    }
}
