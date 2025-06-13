namespace Services.Interfaces
{
    public interface ITokenService
    {
        void GenerateRefreshToken(Token token);
        Token GetRefreshToken(string refreshToken);
        Token GetRefreshTokenByUserID(string id);
        void RemoveAllRefreshToken();
        void ResetRefreshToken();
        void UpdateRefreshToken(Token refreshToken);
    }
}
