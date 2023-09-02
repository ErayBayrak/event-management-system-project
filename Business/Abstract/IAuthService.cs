using Entities.Concrete;
using Entities.DTOs.Company;
using Entities.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAuthService
    {
        User Register(UserForRegisterDto userForRegisterDto);
        User Login(UserForLoginDto userForLoginDto);
        Company CompanyLogin(CompanyForLoginDto companyForLoginDto);
        bool UserExists(string email);
        bool CompanyExists(string email);
        void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        void ChangePassword(int userId, string oldPassword, string newPassword);
        Company CompanyRegister(CompanyForRegisterDto companyForRegisterDto);    

    }
}
