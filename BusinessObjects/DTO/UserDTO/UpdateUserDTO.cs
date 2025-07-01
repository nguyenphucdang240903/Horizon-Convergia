using BusinessObjects.Enums;

namespace BusinessObjects.DTO.UserDTO
{
    public class UpdateUserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AvatarUrl { get; set; }

        public DateTime? Dob { get; set; }
    }
    public class ChangeStatusDTO
    {
        public UserStatus Status { get; set; }
    }

    public class ChangeRoleDTO
    {
        public UserRole Role { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
