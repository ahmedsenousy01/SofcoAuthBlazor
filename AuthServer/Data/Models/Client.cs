namespace AuthServer.Data.Models;

public class Client
{
    public string ClientId { get; set; } = Guid.NewGuid().ToString();
    public string ClientSecret { get; set; } = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
    public string Name { get; set; }
    public List<string> RedirectURIs { get; set; } = [];
    public List<string> Scopes { get; set; } = [];
    public string UserId { get; set; }
    public virtual List<AuthorizationCode> AuthorizationCodes { get; set; } = [];
    public virtual List<AccessToken> AccessTokens { get; set; } = [];
    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
