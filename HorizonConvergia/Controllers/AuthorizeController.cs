using BusinessObjects.DTO.ResultDTO;
using BusinessObjects.DTO.TokenDTO;
using BusinessObjects.Models;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthorizeController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        #region GenerateToken
        /// <summary>
        /// Which will generating token accessible for user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [NonAction]
        public TokenDTO GenerateToken(User user, String? RT)
        {
            List<Claim> claims = new List<Claim>()
    {
        new Claim("UserId", user.Id.ToString()),
        new Claim("UserName", user.Name),
        new Claim("Email", user.Email),
        new Claim("Role", user.Role.ToString())
    };

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU="));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",
                audience: "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            if (RT != null)
            {
                return new TokenDTO()
                {
                    AccessToken = accessToken,
                    RefreshToken = RT,
                    ExpiredAt = _tokenService.GetRefreshTokenByUserID(user.Id).ExpiredTime
                };
            }
            return new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = GenerateRefreshToken(user),
                ExpiredAt = _tokenService.GetRefreshTokenByUserID(user.Id).ExpiredTime
            };
        }
        #endregion

        #region GenerateRefreshToken
        // Hàm tạo refresh token
        [NonAction]
        public string GenerateRefreshToken(User user)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var refreshTokenEntity = new Token
                {
                    Id = user.Id,
                    AccessToken = new Random().Next().ToString(),
                    RefreshToken = refreshtoken.ToString(),
                    ExpiredTime = DateTime.Now.AddDays(7),
                    Status = 1
                };

                _tokenService.GenerateRefreshToken(refreshTokenEntity);
                return refreshtoken;
            }
        }
        #endregion

        #region Login
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string name, string password)
        {
            var user = _userService.GetUserByUserName(name);
            if (user != null && user.IsVerified == true)
            {
                // Hash the input password with SHA256
                var hashedInputPasswordString = PasswordHasher.HashPassword(password);

                if (hashedInputPasswordString == user.Password)
                {
                    // Convert userId to string using .ToString()
                    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name)
    };
                    // Compare the hashed input password with the stored hashed password
                    _tokenService.ResetRefreshToken();
                    var token = GenerateToken(user, null);
                    return Ok(token);
                }
            }
            return BadRequest(new ResultDTO
            {
                IsSuccess = false,
                Message = "Status Code:401 Unauthorized",
                Data = null
            });
        }
        #endregion
    }
}
