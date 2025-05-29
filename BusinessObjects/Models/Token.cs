using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Models
{
    public partial class Token
    {
        [Key]
        public string UserId { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime? ExpiredTime { get; set; }

        public int Status { get; set; }
    }
}
