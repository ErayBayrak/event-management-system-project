using Business.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {

        private readonly IUserService _userService;

        public AuthManager(IUserService userService)
        {
            _userService = userService;
        }
        public User Login(UserForLoginDto request)
        {
            var userToCheck = _userService.GetByMail(request.Email);

            if (userToCheck.Email != request.Email)
            {
                throw new Exception("User not found.");
            }

            if (!VerifyPasswordHash(request.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                throw new Exception("Wrong password.");
            }
            return userToCheck;
        }

        public User Register(UserForRegisterDto request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            };

            _userService.Add(user);
            return user;
        }
        public bool UserExists(string email)
        {
            if (_userService.GetByMail(email) != null)
            {
                throw new Exception("User already exist.");
            }
            return true;

        }


        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userService.Get(x => x.Id == userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            if (!VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
            {
                throw new Exception("Wrong password.");
            }
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _userService.Update(user);

        }
    }
}
