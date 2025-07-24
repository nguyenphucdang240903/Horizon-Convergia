using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> RegisterNewUserAsync(RegisterUserDTO dto);
        Task<User?> GetUserByResetTokenAsync(string token);
        Task UpdateResetPasswordTokenAsync(User user);
        Task UpdatePasswordAsync(User user);
        Task<IEnumerable<UserBasicDTO>> SearchUsersAsync(string? keyword, UserRole? role, UserStatus? status, int pageIndex, int pageSize, string sortBy, string sortOrder);
        Task<int> CountSearchUsersAsync(string? keyword, UserRole? role, UserStatus? status);

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
        Task<User> AdminCreateUserAsync(CreateUserByAdminDTO dto);
    }

}
