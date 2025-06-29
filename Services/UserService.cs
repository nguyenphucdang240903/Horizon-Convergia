using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Security;
using DataAccessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> RegisterNewUserAsync(RegisterUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || !char.IsUpper(dto.Name[0]))
                throw new Exception("Tên phải bắt đầu bằng chữ in hoa.");

            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(dto.Email, emailPattern))
                throw new Exception("Email không đúng định dạng.");

            var passwordPattern = @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            if (!Regex.IsMatch(dto.Password, passwordPattern))
                throw new Exception("Mật khẩu phải có ít nhất 1 ký tự in hoa, 1 số, 1 ký tự đặc biệt và ít nhất 8 ký tự.");

            var phonePattern = @"^0\d{9,10}$";
            if (!Regex.IsMatch(dto.PhoneNumber, phonePattern))
                throw new Exception("Số điện thoại phải từ 10 đến 11 số và bắt đầu bằng số 0.");

            if (dto.Dob.HasValue && dto.Dob.Value.Date > DateTime.Today)
                throw new Exception("Ngày sinh không được lớn hơn ngày hiện tại.");

            var existingUser = await _unitOfWork.Users
                .Query()
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Email đã được sử dụng.");

            if (dto.Role == UserRole.Seller)
            {
                if (string.IsNullOrWhiteSpace(dto.ShopName) ||
                    string.IsNullOrWhiteSpace(dto.ShopDescription) ||
                    string.IsNullOrWhiteSpace(dto.BusinessType))
                {
                    throw new Exception("Người dùng Seller phải nhập đầy đủ thông tin cửa hàng.");
                }
            }

            var requestScheme = _httpContextAccessor.HttpContext?.Request.Scheme;
            var requestHost = _httpContextAccessor.HttpContext?.Request.Host.Value;
            var verificationToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = dto.Name,
                Email = dto.Email,
                Password = PasswordHasher.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Gender = dto.Gender,
                Dob = dto.Dob,
                Role = dto.Role,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsVerified = false,
                VerificationToken = verificationToken,
                VerificationTokenExpires = DateTime.UtcNow.AddHours(24),
                IsDeleted = false,

                ShopName = dto.Role == UserRole.Seller ? dto.ShopName : null,
                shopDescription = dto.Role == UserRole.Seller ? dto.ShopDescription : null,
                BusinessType = dto.Role == UserRole.Seller ? dto.BusinessType : null
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            var encodedToken = WebUtility.UrlEncode(verificationToken);
            //var verifyLink = $"https://www.horizonconvergia.click/verify-email?token={encodedToken}";
            var verifyLink = $"{requestScheme}://{requestHost}/api/auth/verify-email?token={encodedToken}";
            var emailService = new EmailService();
            await emailService.SendVerificationEmailAsync(user.Email, verifyLink);

            return user;
        }

        public async Task<IEnumerable<UserBasicDTO>> GetAllUsersAsync(int pageIndex, int pageSize)
        {
            var users = await _unitOfWork.Users.Query()
                .Where(u => !u.IsDeleted)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return users.Select(u => new UserBasicDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                Status = u.Status,
                Role = u.Role
            });
        }


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }



        public async Task<User?> GetUserByIdAsync(string id) =>
            await _unitOfWork.Users.GetByIdAsync(id);
        public User GetUserByUserName(string userName)
        {
            var user = _unitOfWork.Users.Get(u => u.Name == userName);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<User> GetUserByEmail(string email) => await _unitOfWork.Users.GetByIdAsync(email);


        public async Task<IEnumerable<UserBasicDTO>> SearchUsersAsync(string keyword, int pageIndex, int pageSize)
        {
            var users = await _unitOfWork.Users.SearchAsync(keyword, pageIndex, pageSize);

            return users.Select(u => new UserBasicDTO
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                Status = u.Status,
                Role = u.Role
            });
        }

        public async Task<int> CountSearchUsersAsync(string keyword)
        {
            return await _unitOfWork.Users.CountSearchAsync(keyword);
        }

        public async Task<int> CountAllUsersAsync()
        {
            return await _unitOfWork.Users.Query().CountAsync(u => !u.IsDeleted);
        }
        public async Task UpdateUserAsync(UpdateUserDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.Name) || !char.IsUpper(user.Name[0]))
                throw new Exception("Tên phải bắt đầu bằng chữ in hoa.");
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(user.Email, emailPattern))
                throw new Exception("Email không đúng định dạng.");
            var phonePattern = @"^0\d{9,10}$";
            if (!Regex.IsMatch(user.PhoneNumber, phonePattern))
                throw new Exception("Số điện thoại phải từ 10 đến 11 số và bắt đầu bằng số 0.");
            if (user.Dob.HasValue && user.Dob.Value.Date > DateTime.Today)
                throw new Exception("Ngày sinh không được lớn hơn ngày hiện tại.");
            var existingUser = await _unitOfWork.Users.GetByIdAsync(user.Id);
            existingUser.Address = user.Address;
            existingUser.Email = user.Email;
            existingUser.Name = user.Name;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.AvatarUrl = user.AvatarUrl;
            existingUser.Dob = user.Dob;
            _unitOfWork.Users.Update(existingUser);
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is not null)
            {
                user.IsDeleted = true;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }
            return false;
        }


        public async Task ChangeStatusAsync(string id, UserStatus status)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is not null)
            {
                user.Status = status;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task ChangeRoleAsync(string id, UserRole role)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is not null)
            {
                user.Role = role;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }
        }
        public async Task ChangePasswordAsync(string id, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is not null)
            {
                user.Password = PasswordHasher.HashPassword(newPassword); 
                await UpdatePasswordAsync(user);
            }
        }
        public async Task<User?> GetUserByVerificationTokenAsync(string token)
        {
            return await _unitOfWork.Users
                .Query()
                .FirstOrDefaultAsync(u => u.VerificationToken == token);
        }
        public async Task<User?> GetUserByResetTokenAsync(string token)
        {
            return await _unitOfWork.Users
                .Query()
                .FirstOrDefaultAsync(u => u.ResetPasswordToken == token);
        }

        public async Task UpdateResetPasswordTokenAsync(User user)
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdatePasswordAsync(User user)
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateUserVerificationAsync(User user)
        {
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();
        }
    }

}
