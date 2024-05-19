using System.ComponentModel.DataAnnotations.Schema;

namespace BolognaBilgiSistemi.Models
{
    public class CourseAssignment
    {
        public int CourseAssignmentId { get; set; }

        public int CourseId { get; set; }

        public int FacultyMemberId { get; set; }

        [ForeignKey("CourseId")]
        public Course Course { get; set; }

        [ForeignKey("FacultyMemberId")]
        public FacultyMember FacultyMember { get; set; }
    }
}
