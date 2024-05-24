using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BolognaBilgiSistemi.Models
{
    public class Department
    {
        public int? DepartmentId { get; set; }
        [Required]
        public string? Name { get; set; }
        public int? AdministratorId { get; set; }
        public Administrator? Administrator { get; set; }
        public ICollection<Course>? Courses { get; set; }
        public ICollection<FacultyMember>? FacultyMembers { get; set; }
    }
}
