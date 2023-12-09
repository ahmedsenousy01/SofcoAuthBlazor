namespace AuthServer.Data.Models;

public class User
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => FirstName + " " + LastName;
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public virtual List<Client> Clients { get; set; } = [];
    public virtual List<AuthorizationCode> AuthorizationCodes { get; set; } = [];
    public virtual List<AccessToken> AccessTokens { get; set; } = [];
    public virtual List<RefreshToken> RefreshTokens { get; set; } = [];
}