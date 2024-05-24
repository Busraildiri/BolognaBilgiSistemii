using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BolognaBilgiSistemi.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required]
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string Description { get; set; } // Yeni özellik eklendi
        public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
    }
}
