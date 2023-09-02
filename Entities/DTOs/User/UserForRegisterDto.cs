using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTOs.User
{
    public class UserForRegisterDto
    {
        public string Email { get; set; }
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Şifre en az 8 karakter olmalı ve hem harf hem rakam içermelidir.")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string PasswordConfirm { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
