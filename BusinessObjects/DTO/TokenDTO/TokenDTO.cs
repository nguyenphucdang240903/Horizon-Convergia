namespace BusinessObjects.DTO.TokenDTO
{
    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
}
