using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework;
using Entities.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Business.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Text;
using static System.Net.WebRequestMethods;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IAuthService authService, IUserService userService)
        {
            _configuration = configuration;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserForRegisterDto request)
        {
            var userExists = _authService.UserExists(request.Email);
            if (!userExists)
            {
                return BadRequest("Bu maile kayıtlı kullanıcı mevcut");
            }
            var registerResult = _authService.Register(request);
            return Ok(registerResult);
        }
        [HttpPost("adminlogin")]
        public async Task<ActionResult<string>> AdminLogin(UserForLoginDto request)
        {
            
            if (request.Email == "admin@gmail.com" && request.Password == "admin")
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, "Admin admin"));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, request.Email));
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: "https://localhost:44394",
                 audience: "https://localhost:44394",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                  signingCredentials: creds
                    );

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                string userToken = tokenHandler.WriteToken(securityToken);
                return Ok(userToken);

            }
            else
            {
                return NotFound();
            }

        }


        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserForLoginDto request)
        {
            var userToLogin = _authService.Login(request);

            string token = CreateToken(userToLogin);
            return Ok(token);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        private string CreateToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: "https://localhost:44394",
                audience: "https://localhost:44394",
                         claims: claims,
                expires: DateTime.Now.AddDays(1),
                notBefore: DateTime.Now,
                signingCredentials: creds
                );



            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }
        [HttpGet]
        public bool ValidateToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            try
            {
                JwtSecurityTokenHandler handler = new();
                handler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                }, out SecurityToken validatedToken
                );
                var jwtToken = (JwtSecurityToken)validatedToken;
                var claims = jwtToken.Claims.ToList();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        [Authorize]
        [HttpPost("updateprofile")]
        public async Task<ActionResult> UpdateProfile(UserUpdateProfileDto request)
        {
            var existingUser = _userService.GetByMail(request.Email);
            if (existingUser == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                if (!_authService.VerifyPasswordHash(request.OldPassword, existingUser.PasswordHash, existingUser.PasswordSalt))
                {
                    return BadRequest("Eski şifre yanlış.");
                }

                byte[] passwordHash, passwordSalt;
                _authService.CreatePasswordHash(request.NewPassword, out passwordHash, out passwordSalt);

                existingUser.PasswordHash = passwordHash;
                existingUser.PasswordSalt = passwordSalt;
            }

            existingUser.FirstName = request.FirstName;
            existingUser.LastName = request.LastName;

            _userService.Update(existingUser);

            return Ok("Profil güncellendi.");
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<User>> GetUserProfile()
        {
            //var user = _userService.GetByMail(email); 
            //if (user == null)
            //{
            //    return NotFound("Kullanıcı bulunamadı.");
            //}
            //return Ok(user); 

            var userEmailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (userEmailClaim == null)
            {
                return BadRequest("Kullanıcı bilgisi alınamadı.");
            }
            var userEmail = userEmailClaim.Value;
            var user = _userService.GetByMail(userEmail);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            return Ok(user);
        }

    }
}
