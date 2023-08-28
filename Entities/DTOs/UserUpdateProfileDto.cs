using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs
{
    public class UserUpdateProfileDto
    {
        public string Email { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Şifre en az 8 karakter olmalı ve hem harf hem rakam içermelidir.")]
        public string NewPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OldPassword { get; set; }
    }
}
