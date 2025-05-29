using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Security;
using DataAccessObjects;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using System.Text.RegularExpressions;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

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
            {
                throw new Exception("Email đã được sử dụng.");
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = PasswordHasher.HashPassword(dto.Password),
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Gender = dto.Gender,
                Dob = dto.Dob,
                Role = UserRole.Buyer,
                Status = UserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsVerified = true,
                IsDeleted = false
            };
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();
            return user;
        }


        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _unitOfWork.Users.GetByEmailAsync(email);
        }



        public async Task<User?> GetUserByIdAsync(long id) =>
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

        public async Task<IEnumerable<User>> SearchUsersAsync(string keyword) =>
            await _unitOfWork.Users.SearchAsync(keyword);

        public async Task UpdateUserAsync(UpdateUserDTO user)
        {
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

        public async Task<bool> DeleteUserAsync(long id)
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


        public async Task ChangeStatusAsync(long id, UserStatus status)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is not null)
            {
                user.Status = status;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task ChangeRoleAsync(long id, UserRole role)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is not null)
            {
                user.Role = role;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task ChangePasswordAsync(long id, string newPassword)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user is not null)
            {
                user.Password = newPassword; // Hashing should be applied in real scenarios
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveAsync();
            }
        }
    }

}
