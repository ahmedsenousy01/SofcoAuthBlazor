﻿using Microsoft.AspNetCore.Authentication.Cookies;
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
using AuthServer.Data.Models;
using System.Text.RegularExpressions;

namespace AuthServer.Api
{
    public static class Router
    {
        public static void MapOAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/client", async (HttpContext ctx, ClientRepository clientRepo) =>
            {
                var clientName = ctx.Request.Form["ClientName"].ToString();
                var redirectURIs = ctx.Request.Form["RedirectURIs"].ToString();
                var scopes = ctx.Request.Form["Scopes"].ToString();
                string error;

                // Parse Redirect URIs and Scopes from inputs
                List<string> r = redirectURIs.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(
                    uri => uri.Trim()).ToList();
                List<string> s = scopes.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(scope =>
                scope.Trim()).ToList();

                // TODO: validation

                var u = ctx.User;
                var userId = u.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)!.Value;

                var createdSuccessfully = await clientRepo.CreateClientAsync(new Client
                {
                    Name = clientName,
                    RedirectURIs = r,
                    Scopes = s
                }, userId);

                if (createdSuccessfully)
                {
                    return Results.Redirect($"/client?result=1");
                }

                return Results.Redirect($"/client?error={Uri.EscapeDataString("")}");

            });

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

                string error = "Username or password incorrect";
                return Results.Redirect($"/account/login?error={Uri.EscapeDataString(error)}&ReturnUrl={ReturnUrl}");
            }).RequireCors("privateRoute");

            app.MapPost("/register", async (HttpContext ctx, UserRepository userRepo) =>
            {
                var firstName = ctx.Request.Form["FirstName"].ToString();
                var lastName = ctx.Request.Form["LastName"].ToString();
                var email = ctx.Request.Form["Email"].ToString();
                var password = ctx.Request.Form["Password"].ToString();
                var confirmPassword = ctx.Request.Form["ConfirmPassword"].ToString();
                List<string> errors = [];

                // validate user (name, email, pass)
                string namePattern = "^[A-Za-z]{3,50}$";
                bool isFirstNameValid = Regex.IsMatch(firstName, namePattern);
                bool isLastNameValid = Regex.IsMatch(lastName, namePattern);

                string emailPattern = "^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Z|a-z]{2,}$";
                bool isEmailValid = Regex.IsMatch(email, emailPattern);

                string passwordPattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^\\da-zA-Z]).{8,}$";
                bool isPasswordValid = Regex.IsMatch(password, passwordPattern);
                bool confirmPasswordMatch = password == confirmPassword;

                bool clientIsValid = true;
                if (!isFirstNameValid)
                {
                    errors.Add("Invalid first name");
                    clientIsValid = false;
                }
                if (!isLastNameValid)
                {
                    errors.Add("Invalid last name");
                    clientIsValid = false;

                }
                if (!isEmailValid)
                {
                    errors.Add("Invalid email");
                    clientIsValid = false;
                }
                if (!isPasswordValid)
                {
                    errors.Add("Invalid password: password must be at least 8 characters long and should have 1 lowercase letter, 1 uppercase letter, 1 number and 1 symbol");
                    clientIsValid = false;
                }
                if (!confirmPasswordMatch)
                {
                    errors.Add("Passwords must match");
                    clientIsValid = false;
                }

                var u = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = email,
                    Email = email,
                    PasswordHash = password
                };

                var createdSuccessfully = false;
                if (clientIsValid)
                    createdSuccessfully = await userRepo.CreateUserAsync(u);

                if (createdSuccessfully)
                {
                    return Results.Redirect($"/account/register?result=1");
                }

                return Results.Redirect($"/account/register?errors={Uri.EscapeDataString(JsonSerializer.Serialize(errors))}");
            });

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

                    return Results.Redirect(redirectUrl, true);
                }
                else if (responseType == "implicit")
                {
                    var authCodeString = await authCodeRepo.GenerateAuthCodeAsync(currentClient.ClientId, currentUser.UserId, codeChallenge, codeChallengeMethod);
                    var authCode = currentClient.AuthorizationCodes.FirstOrDefault(c => c.AuthCodeString == authCodeString);
                    var user = await userRepo.GetUserByIdAsync(currentClient.UserId);
                    var identityToken = tokenService.GenerateIdentityToken(user);
                    var tokenPair = await tokenService.GenerateAccessRefreshTokenPair(authCode!);
                    authCode.Used = true;

                    var redirectUrl =
                    $"{redirectUri}?" +
                    $"identity_token={identityToken}&" +
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
