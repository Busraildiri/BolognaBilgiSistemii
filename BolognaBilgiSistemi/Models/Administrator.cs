using System.ComponentModel.DataAnnotations;

namespace BolognaBilgiSistemi.Models
{
    public class Administrator
    {
        public int Id { get; set; }
        [Required]
        public string TCNumber { get; set; }  // TC kimlik numarası
        [Required]
        public string Password { get; set; }  // Şifre
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int DepartmentId { get; set; }  // Bölüm ID'si, zorunlu olmayan
        public Department Department { get; set; }
    }
}
