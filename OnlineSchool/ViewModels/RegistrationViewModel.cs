using System.ComponentModel.DataAnnotations;

namespace OnlineSchool.ViewModels
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(100)]
        public string Fullname { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(60)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(64)]
        public string Password { get; set; }
    }
}
