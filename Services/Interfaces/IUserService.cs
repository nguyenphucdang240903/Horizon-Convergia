using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserBasicDTO>> GetAllUsersAsync(int pageIndex, int pageSize);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> RegisterNewUserAsync(RegisterUserDTO dto);
        Task<User?> GetUserByResetTokenAsync(string token);
        Task UpdateResetPasswordTokenAsync(User user);
        Task UpdatePasswordAsync(User user);
        Task<IEnumerable<UserBasicDTO>> SearchUsersAsync(string keyword, int pageIndex, int pageSize);
        Task<int> CountSearchUsersAsync(string keyword);
        Task<int> CountAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        User GetUserByUserName(string userName);
        Task<User?> GetUserByEmail(string email);
        Task UpdateUserAsync(UpdateUserDTO user);
        Task<bool> DeleteUserAsync(string id);
        Task ChangeStatusAsync(string id, UserStatus status);
        Task ChangeRoleAsync(string id, UserRole role);
        Task ChangePasswordAsync(string id, string newPassword);
        Task<User> GetUserByVerificationTokenAsync(string token);
        Task UpdateUserVerificationAsync(User user);
        Task<User> AdminCreateUserAsync(RegisterUserDTO dto);
    }

}
