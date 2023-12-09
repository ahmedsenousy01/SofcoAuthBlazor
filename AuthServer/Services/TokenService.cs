using System.IdentityModel.Tokens.Jwt; 
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using AuthServer.Data.Models;
using System.Text.Json;
using AuthServer.Data;

namespace AuthServer.Services;

public class TokenService(ApplicationDbContext db)
{
    private readonly ApplicationDbContext db = db;
    private static string GenerateJwtToken(string secretKey, Claim[] claims)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public async Task<Tuple<AccessToken, RefreshToken>> GenerateAccessRefreshTokenPair(AuthorizationCode authCode)
    {
        var clientSecret = (await db.Clients.FindAsync(authCode.ClientId))!.ClientSecret;
        var accessToken = new AccessToken()
        {
            ClientId = authCode.ClientId,
            UserId = authCode.UserId,
            AccessTokenString = GenerateJwtToken(clientSecret, [
            new Claim("iss", Constants.ServerBaseUrl),
            new Claim("sub", authCode.UserId),
            new Claim("aud", authCode.ClientId),
            new Claim("scope", JsonSerializer.Serialize("Scopes")),
            new Claim("iat", DateTime.Now.Ticks.ToString()),
            new Claim("exp", DateTime.Now.AddMinutes(15).Ticks.ToString())])
        };
        var refreshToken = new RefreshToken()
        {
            ClientId = authCode.ClientId,
            UserId = authCode.UserId,
            AuthorizationCodeId = authCode.AuthorizationCodeId
        };
        return new Tuple<AccessToken, RefreshToken>(accessToken, refreshToken);
    }

    public string GenerateIdentityToken(User u)
    {
        return GenerateJwtToken(Constants.SymmetricEncryptionKey, [
            new Claim("iss", Constants.ServerBaseUrl),
            new Claim("sub", u.UserId),
            new Claim("email", u.Email),
            new Claim("GivenName", u.FirstName),
            new Claim("FamilyName", u.LastName),
            new Claim("iat", DateTime.Now.Ticks.ToString()),
            new Claim("exp", DateTime.Now.AddMinutes(15).Ticks.ToString())
        ]);
    }
}
