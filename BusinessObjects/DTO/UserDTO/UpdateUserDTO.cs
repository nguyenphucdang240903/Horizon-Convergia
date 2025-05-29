namespace BusinessObjects.DTO.UserDTO
{
    public class UpdateUserDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AvatarUrl { get; set; }

        public DateTime? Dob { get; set; }
    }
}
