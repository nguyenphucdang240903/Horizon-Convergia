using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects;
using Services.Interfaces;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task RegisterAsync(User user)
        {
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();
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
