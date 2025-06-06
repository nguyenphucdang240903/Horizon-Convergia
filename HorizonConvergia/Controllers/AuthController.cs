﻿using BusinessObjects.DTO.ResultDTO;
using BusinessObjects.DTO.TokenDTO;
using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            try
            {
                var user = await _userService.RegisterNewUserAsync(dto);

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công. Vui lòng kiểm tra email để xác minh tài khoản.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = await _userService.GetUserByVerificationTokenAsync(token);
            if (user == null || user.VerificationTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("Token không hợp lệ hoặc đã hết hạn.");
            }

            user.IsVerified = true;
            user.VerificationToken = null;
            user.VerificationTokenExpires = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userService.UpdateUserVerificationAsync(user);

            return Ok("Tài khoản đã được xác minh thành công.");
        }


        [NonAction]
        public string GenerateVerificationToken()
        {
            var tokenBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            return Convert.ToBase64String(tokenBytes);
        }



        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse), "Auth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
                return Unauthorized(new { Message = "Google authentication failed." });

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return BadRequest(new { Message = "Email not found in Google claims." });

            // Retrieve user from database
            var user = await _userService.GetUserByEmail(email);

            if (user == null)
            {
                // Optionally, register the user here if you want to auto-register Google users
                return Unauthorized(new { Message = "User not registered." });
            }

            // Generate JWT token
            var token = GenerateToken(user, null);

            // Always return JSON
            return Ok(token); // Return the token as JSON response
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
                    UserId = user.Id,
                    AccessToken = new Random().Next().ToString(),
                    RefreshToken = refreshtoken,
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
        public IActionResult Login(string email, string password)
        {
            var user = _userService.GetUserByEmailAsync(email).Result;
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
        new Claim(ClaimTypes.Name, user.Email)
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

        #region Logout
        [HttpPost]
        [Route("Logout")]
        public IActionResult Logout()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
                {
                    return BadRequest(new ResultDTO
                    {
                        IsSuccess = false,
                        Message = "Invalid token format."
                    });
                }

                token = token.Split(' ')[1];
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("c2VydmVwZXJmZWN0bHljaGVlc2VxdWlja2NvYWNoY29sbGVjdHNsb3Bld2lzZWNhbWU=")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = false
                };

                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                var userIdClaim = claimsPrincipal.FindFirst("UserId");

                if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out long userId))
                {
                    // Handle the case where the UserId claim is missing or invalid
                    return BadRequest(new ResultDTO
                    {
                        IsSuccess = false,
                        Message = "Invalid UserId."
                    });
                }

                var refreshToken = _tokenService.GetRefreshTokenByUserID(userId);
                _tokenService.UpdateRefreshToken(refreshToken);
                _tokenService.ResetRefreshToken();

                if (HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    HttpContext.Request.Headers.Remove("Authorization");
                }

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Logout successfully!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Something went wrong: " + ex.Message
                });
            }
        }
        #endregion


        #region Who Am I
        /// <summary>
        /// Check infor of user
        /// </summary>
        /// <returns>Infor of user</returns>
        [HttpGet("whoami")]
        public IActionResult WhoAmI()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // Lấy thông tin về người dùng từ claims
            var userIdClaim = User.FindFirst("UserId");
            var userNameClaim = User.FindFirst("UserName");
            var userEmailClaim = User.FindFirst("Email");
            var userRoleClaim = User.FindFirst("Role");

            // Kiểm tra xem các claim có tồn tại không
            if (userIdClaim == null || userNameClaim == null || userEmailClaim == null || userRoleClaim == null)
            {
                return Unauthorized(new { Message = "Missing user information in claims" });
            }

            try
            {
                // Chuyển đổi kiểu dữ liệu
                long userId = long.Parse(userIdClaim.Value);
                UserRole userRole = (UserRole)Enum.Parse(typeof(UserRole), userRoleClaim.Value);

                // Tạo response object
                var response = new
                {
                    UserId = userId,
                    UserName = userNameClaim.Value,
                    Email = userEmailClaim.Value,
                    Role = userRole.ToString() // Hoặc có thể trả về (int)userRole nếu muốn giá trị số
                };

                return Ok(response);
            }
            catch (FormatException)
            {
                return BadRequest(new { Message = "Invalid UserId format" });
            }
            catch (ArgumentException)
            {
                return BadRequest(new { Message = "Invalid Role value" });
            }
            catch (OverflowException)
            {
                return BadRequest(new { Message = "UserId value is too large" });
            }
        }
        #endregion
    }
}
