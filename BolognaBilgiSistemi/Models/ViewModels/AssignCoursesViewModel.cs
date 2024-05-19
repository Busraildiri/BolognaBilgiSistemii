using BolognaBilgiSistemi.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BolognaBilgiSistemi.Models
{
    public class AssignCoursesViewModel
    {
        public Department Department { get; set; }
        public List<CourseViewModel> Courses { get; set; }
        public SelectList FacultyMembers { get; set; }
    }
}
