using System.ComponentModel.DataAnnotations;

namespace CathedralLibraryInfrastructure.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Ім'я")]
        public string First_name { get; set; }

        [Required]
        [Display(Name = "Прізвище")]
        public string Last_name { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Рік народження")]
        public int Year { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        public string PasswordConfirm { get; set; }
    }
}
