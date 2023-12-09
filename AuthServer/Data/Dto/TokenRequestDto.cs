namespace AuthServer.Data.Dto;

public class TokenRequestDto
{
    public string GrantType { get; set; }
    public string? ClientId { get; set; }
    public string? CodeVerifier { get; set; }
    public string? Code { get; set; }
    public string? RefreshToken { get; set; }
}
