using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using AuthServer.Repositories;
using AuthServer.Components;
using AuthServer.Data;
using AuthServer.Api;
using AuthServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/account/login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options
        .UseLazyLoadingProxies()
        .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ClientRepository>();
builder.Services.AddScoped<AuthCodeRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "privateRoute", builder =>
    {
        builder.WithOrigins("http://localhost:5102");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.MapGet("/info", (ApplicationDbContext db) =>
    {
        return Results.Json(new
        {
            users = db.Users.ToList(),
            clients = db.Clients.ToList(),
            authcodes = db.AuthorizationCodes.ToList(),
            accesstokens = db.AccessTokens.ToList(),
            refreshtokens = db.RefreshTokens.ToList()
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseCors();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapOAuthEndpoints();

app.Run();
