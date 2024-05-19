//namespace BolognaBilgiSistemi.Models
//{
//    public class LoginViewModel
//    {
//        public string UserType { get; set; }
//        public string TCNumber { get; set; }
//        public string Password { get; set; }
//    }

//}

using System.ComponentModel.DataAnnotations;

namespace BolognaBilgiSistemi.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Bu alan zorunludur.")]
        public string TCNumber { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string UserType { get; set; }
    }
}


//using System.ComponentModel.DataAnnotations;

//namespace BolognaBilgiSistemi.Models
//{
//    public class LoginViewModel
//    {
//        [Required]
//        public string TCNumber { get; set; }

//        [Required]
//        [DataType(DataType.Password)]
//        public string Password { get; set; }
//    }
//}
