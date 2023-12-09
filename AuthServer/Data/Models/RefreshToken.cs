namespace AuthServer.Data.Models;

public class RefreshToken
{
    public int RefreshTokenId { get; set; }
    public string RefreshTokenString { get; set; } = Guid.NewGuid().ToString();
    public DateTime Expiration { get; set; }
    public bool Revoked { get; set; } = false;
    public bool Used { get; set; } = false;
    public string UserId { get; set; }
    public string ClientId { get; set; }
    public int AuthorizationCodeId { get; set; }
}
