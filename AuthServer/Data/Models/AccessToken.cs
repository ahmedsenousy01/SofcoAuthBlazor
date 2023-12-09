namespace AuthServer.Data.Models;

public class AccessToken
{
    public int AccessTokenId { get; set; }
    public string? AccessTokenString { get; set; }
    public List<string> Scopes { get; set; } = [];
    public DateTime Expiration { get; set; } = DateTime.Now.AddMinutes(15);
    public bool Revoked { get; set; } = false;
    public string ClientId { get; set; }
    public string UserId { get; set; }
}