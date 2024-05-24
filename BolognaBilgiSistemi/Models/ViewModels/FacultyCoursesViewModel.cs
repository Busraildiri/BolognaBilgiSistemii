using System.Collections.Generic;

namespace BolognaBilgiSistemi.Models.ViewModels
{
    public class FacultyCoursesViewModel
    {
        public string FacultyName { get; set; }
        public List<CourseViewModel> Courses { get; set; }
    }
}
