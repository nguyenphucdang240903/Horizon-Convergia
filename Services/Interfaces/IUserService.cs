using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> RegisterNewUserAsync(RegisterUserDTO dto);


        Task<User?> GetUserByIdAsync(long id);
        User GetUserByUserName(string userName);
        Task<IEnumerable<User>> SearchUsersAsync(string keyword);
        Task UpdateUserAsync(UpdateUserDTO user);
        Task<bool> DeleteUserAsync(long id);
        Task ChangeStatusAsync(long id, UserStatus status);
        Task ChangeRoleAsync(long id, UserRole role);
        Task ChangePasswordAsync(long id, string newPassword);
        Task<User> GetUserByVerificationTokenAsync(string token);
        Task UpdateUserVerificationAsync(User user);

    }

}
