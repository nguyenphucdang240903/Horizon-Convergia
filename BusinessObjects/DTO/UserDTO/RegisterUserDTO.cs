using BusinessObjects.Enums;

namespace BusinessObjects.DTO.UserDTO
{
    public class RegisterUserDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public DateTime? Dob { get; set; }
        public UserRole Role { get; set; }

    }
    public class CreateUserByAdminDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public DateTime? Dob { get; set; }
        public UserRole Role { get; set; }
        public string? ShopName { get; set; }
        public string? ShopDescription { get; set; }
        public string? BusinessType { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountHolder { get; set; }
    }

}
