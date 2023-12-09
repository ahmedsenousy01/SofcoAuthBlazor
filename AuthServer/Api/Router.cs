using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using AuthServer.Repositories;
using System.Security.Claims;
using System.Web;
using AuthServer.Data.Dto;
using System.Text.Json;
using AuthServer.Services;
using AuthServer.Utils;
using AuthServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Extensions;

namespace AuthServer.Api
{
    public static class Router
    {
        public static void MapOAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (HttpContext ctx, UserRepository userRepo) =>
            {
                var email = ctx.Request.Form["email"].ToString();
                var password = ctx.Request.Form["password"].ToString();

                // Manually get the return url from the query string if it has query parameters of its own
                var url = ctx.Request.GetDisplayUrl();
                int returnUrlIndex = url.IndexOf("ReturnUrl=");
                string ReturnUrl = url.Substring(returnUrlIndex + "ReturnUrl=".Length);

                var user = await userRepo.GetUserByEmailAsync(email);
                if (user != null && password == user.PasswordHash)
                {
                    var u = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
                        {
                            new(ClaimTypes.NameIdentifier, user.UserId),
                            new(ClaimTypes.Name, user.Email)
                        }, CookieAuthenticationDefaults.AuthenticationScheme));
                    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, u);
                    return Results.Redirect(ReturnUrl ?? "/");
                }
                return Results.Json(new
                {
                    status = false,
                    message = "Username or password incorrect"
                });
            }).RequireCors("privateRoute");

            app.MapGet("/authorize",
            async (
                HttpContext ctx,
                UserRepository userRepo,
                ClientRepository clientRepo,
                AuthCodeRepository authCodeRepo,
                TokenService tokenService,
                [FromQuery] string? responseType,
                [FromQuery] string? clientId,
                [FromQuery] string? codeChallenge,
                [FromQuery] string? codeChallengeMethod,
                [FromQuery] string? redirectUri,
                [FromQuery] string? scope,
                [FromQuery] string? state
            ) =>
            {
                var u = ctx.User;
                var currentUser = await userRepo.GetUserByIdAsync(u.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value);
                var currentClient = await clientRepo.GetClientByIdAsync(clientId!);

                if (string.IsNullOrEmpty(clientId) ||
                    string.IsNullOrEmpty(codeChallenge) ||
                    string.IsNullOrEmpty(codeChallengeMethod) ||
                    string.IsNullOrEmpty(redirectUri) ||
                    string.IsNullOrEmpty(scope) ||
                    string.IsNullOrEmpty(state) ||
                    string.IsNullOrEmpty(responseType) ||
                    currentClient is null ||
                    currentUser is null
                    )
                {
                    return Results.Redirect(@$"{redirectUri}?error=invalid_request&error_message={JsonSerializer.Serialize(new
                    {
                        clientId,
                        codeChallenge,
                        codeChallengeMethod,
                        redirectUri,
                        scope,
                        state,
                        responseType,
                        currentClient,
                        currentUser
                    })}");
                }

                if (responseType == "code")
                {
                    // TODO: handle edge cases (validation)

                    var authCode = await authCodeRepo.GenerateAuthCodeAsync(currentClient.ClientId, currentUser.UserId, codeChallenge, codeChallengeMethod);
                    var redirectUrl =
                    $"{redirectUri}?" +
                    $"code={authCode}&" +
                    $"state={state}&" +
                    $"iss={HttpUtility.UrlEncode(Constants.ServerBaseUrl)}";

                    return Results.Redirect(redirectUrl);
                }
                else if (responseType == "implicit")
                {
                    var authCodeString = await authCodeRepo.GenerateAuthCodeAsync(currentClient.ClientId, currentUser.UserId, codeChallenge, codeChallengeMethod);
                    var authCode = currentClient.AuthorizationCodes.FirstOrDefault(c => c.AuthCodeString == authCodeString);
                    var tokenPair = await tokenService.GenerateAccessRefreshTokenPair(authCode!);
                    authCode.Used = true;

                    var redirectUrl =
                    $"{redirectUri}?" +
                    $"access_token={tokenPair.Item1.AccessTokenString}&" +
                    $"access_token={tokenPair.Item1.AccessTokenString}&" +
                    $"refresh_token={tokenPair.Item2.RefreshTokenString}&" +
                    $"state={state}&" +
                    $"iss={HttpUtility.UrlEncode(Constants.ServerBaseUrl)}";

                    return Results.Redirect(redirectUrl);
                }
                return Results.Redirect(@$"{redirectUri}?error=invalid_request&error_message={JsonSerializer.Serialize(new
                {
                    clientId,
                    codeChallenge,
                    codeChallengeMethod,
                    redirectUri,
                    scope,
                    state,
                    responseType,
                    currentClient,
                    currentUser
                })}");
            }).RequireAuthorization();

            app.MapPost("/token",
            async (
                HttpContext ctx,
                ApplicationDbContext db,
                UserRepository userRepo,
                ClientRepository clientRepo,
                TokenService tokenService,
                [FromBody] TokenRequestDto body
            ) =>
            {
                if (body.GrantType == "authorization_code")
                {
                    var client = await clientRepo.GetClientByIdAsync(body.ClientId!);
                    if (client is null)
                        return Results.Json(new
                        {
                            status = false,
                            message = "client not found"
                        });

                    var authCode = client.AuthorizationCodes.FirstOrDefault(c => c.AuthCodeString == body.Code);
                    if (authCode is null || body.Code != authCode.AuthCodeString)
                        return Results.Json(new
                        {
                            status = false,
                            message = "Invalid auth code"
                        });

                    if (authCode.Used)
                    {
                        authCode.RefreshTokens.ForEach(rt => rt.Revoked = true);
                        return Results.Json(new
                        {
                            status = false,
                            message = "Invalid auth code"
                        });
                    }

                    if (!string.IsNullOrEmpty(body.CodeVerifier))
                    {
                        if (authCode.CodeChallengeMethod == "Plain")
                        {
                            if (body.CodeVerifier != authCode.CodeChallenge)
                                return Results.Json(new
                                {
                                    status = false,
                                    message = "Invalid code challenge, code error"
                                });
                        }
                        else if (authCode.CodeChallengeMethod == "S256")
                        {
                            // TODO: fix hashing error (can't verify hash correctly)
                            if (!Hasher.VerifySha256Hash(body.CodeVerifier, authCode.CodeChallenge))
                                return Results.Json(new
                                {
                                    status = false,
                                    message = "Invalid code challenge, hashing error"
                                });
                        }
                        else return Results.Json(new
                        {
                            status = false,
                            message = "PKCE validation failed"
                        });
                    }
                    else return Results.Json(new
                    {
                        status = false,
                        message = "PKCE validation failed"
                    });

                    var tokenPair = await tokenService.GenerateAccessRefreshTokenPair(authCode);
                    authCode.Used = true;

                    await db.AccessTokens.AddAsync(tokenPair.Item1);
                    await db.RefreshTokens.AddAsync(tokenPair.Item2);
                    await db.SaveChangesAsync();

                    return Results.Json(new
                    {
                        status = true,
                        accessToken = tokenPair.Item1.AccessTokenString,
                        refreshToken = tokenPair.Item2.RefreshTokenString,
                        identityToken = tokenService.GenerateIdentityToken(db.Users.Find(authCode.UserId)!)
                    });
                }
                else if (body.GrantType == "refresh_token")
                {
                    var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(rt => rt.RefreshTokenString == body.RefreshToken);
                    var authCode = await db.AuthorizationCodes.FirstOrDefaultAsync(ac => ac.AuthorizationCodeId == refreshToken!.AuthorizationCodeId);
                    if (refreshToken!.Used)
                    {
                        authCode!.RefreshTokens.ForEach(rt => rt.Revoked = true);
                        await db.SaveChangesAsync();
                        return Results.Json(new
                        {
                            status = false,
                            message = "Access denied"
                        });
                    }
                    var tokenPair = await tokenService.GenerateAccessRefreshTokenPair(authCode!);
                    refreshToken.Used = true;

                    await db.AccessTokens.AddAsync(tokenPair.Item1);
                    await db.RefreshTokens.AddAsync(tokenPair.Item2);
                    await db.SaveChangesAsync();

                    return Results.Json(new
                    {
                        status = true,
                        accessToken = tokenPair.Item1.AccessTokenString,
                        refreshToken = tokenPair.Item2.RefreshTokenString,
                    });
                }
                else return Results.Json(new
                {
                    status = false,
                    message = "Grant Type Error"
                });
            });
        }
    }
}
