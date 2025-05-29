using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(User user);
        Task<User?> GetUserByIdAsync(long id);
        Task<IEnumerable<User>> SearchUsersAsync(string keyword);
        Task UpdateUserAsync(UpdateUserDTO user);
        Task DeleteUserAsync(long id);
        Task ChangeStatusAsync(long id, UserStatus status);
        Task ChangeRoleAsync(long id, UserRole role);
        Task ChangePasswordAsync(long id, string newPassword);
    }

}
