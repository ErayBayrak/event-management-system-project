using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Business.Abstract;
using Microsoft.AspNetCore.Identity;
using System.Text;
using static System.Net.WebRequestMethods;
using Entities.DTOs.Company;
using Entities.DTOs.User;
using WebAPI.Models;
using Microsoft.Extensions.Options;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly TokenOption _tokenOption;

        public AuthController( IAuthService authService, IUserService userService, IOptions<TokenOption> options)
        {
            _authService = authService;
            _userService = userService;
            _tokenOption = options.Value;
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
        [HttpPost("companyregister")]
        public async Task<ActionResult<Company>> CompanyRegister(CompanyForRegisterDto dto)
        {
            var companyExists = _authService.CompanyExists(dto.Email);
            if (!companyExists)
            {
                return BadRequest("Bu maile kayıtlı kullanıcı mevcut");
            }
            var registerResult = _authService.CompanyRegister(dto);
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

                JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                 audience: _tokenOption.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_tokenOption.AccessTokenExpiration),
                  signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOption.Token)), SecurityAlgorithms.HmacSha512Signature)
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
        [HttpPost("companylogin")]
        public async Task<ActionResult<string>> CompanyLogin(CompanyForLoginDto request)
        {
            var companyToLogin = _authService.CompanyLogin(request);
            string token = CreateTokenForCompany(companyToLogin);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserForLoginDto request)
        {
            var userToLogin = _authService.Login(request);

            string token = CreateToken(userToLogin);
            return Ok(token);
        }
        private string CreateTokenForCompany(Company company)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,company.Email),
                new Claim(ClaimTypes.Role,"Company")
            };

            var token = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                audience: _tokenOption.Audience,
                         claims: claims,
                expires: DateTime.Now.AddDays(_tokenOption.AccessTokenExpiration),
                notBefore: DateTime.Now,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOption.Token)), SecurityAlgorithms.HmacSha512Signature)
                );



            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }

        private string CreateToken(User user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                audience: _tokenOption.Audience,
                         claims: claims,
                expires: DateTime.Now.AddDays(_tokenOption.AccessTokenExpiration),
                notBefore: DateTime.Now,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOption.Token)), SecurityAlgorithms.HmacSha512Signature)
                );



            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }
        [HttpGet]
        public bool ValidateToken(string token)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_tokenOption.Token));
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
