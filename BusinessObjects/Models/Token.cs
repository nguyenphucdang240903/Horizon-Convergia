using System.ComponentModel.DataAnnotations;

public partial class Token
{
    [Key]
    public long Id { get; set; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public DateTime? ExpiredTime { get; set; }

    public int Status { get; set; }
    public long UserId { get; set; }
}
