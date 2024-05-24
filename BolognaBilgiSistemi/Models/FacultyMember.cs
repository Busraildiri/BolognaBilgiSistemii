using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BolognaBilgiSistemi.Models
{
    public class FacultyMember
    {
        private ICollection<CourseAssignment>? courseAssignments = new List<CourseAssignment>();

        public int Id { get; set; }
        [Required]
        public string TCNumber { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        public ICollection<CourseAssignment>? CourseAssignments { get => courseAssignments; set => courseAssignments = value; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}
