namespace AuthServer.Data.Models;

public class AuthorizationCode
{
    public int AuthorizationCodeId { get; set; }
    public string AuthCodeString { get; set; } = Guid.NewGuid().ToString();
    public string CodeChallenge { get; set; }
    public string CodeChallengeMethod { get; set; } = "Plain";
    public DateTime Expiration { get; set; } = DateTime.Now.AddMinutes(10);
    public bool Used { get; set; } = false;
    public string ClientId { get; set; }
    public string UserId { get; set; }
    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}
