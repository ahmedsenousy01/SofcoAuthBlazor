using Microsoft.EntityFrameworkCore;
using AuthServer.Data.Models;

namespace AuthServer.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<AuthorizationCode> AuthorizationCodes { get; set; }
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<Client>()
            .HasKey(c => c.ClientId);

        modelBuilder.Entity<AuthorizationCode>()
            .HasKey(ac => ac.AuthorizationCodeId);

        modelBuilder.Entity<AccessToken>()
            .HasKey(at => at.AccessTokenId);

        modelBuilder.Entity<RefreshToken>()
            .HasKey(rt => rt.RefreshTokenId);

        modelBuilder.Entity<Client>()
            .HasMany(c => c.AuthorizationCodes)
            .WithOne()
            .HasForeignKey(ac => ac.ClientId);

        modelBuilder.Entity<Client>()
            .HasMany(c => c.AccessTokens)
            .WithOne()
            .HasForeignKey(at => at.ClientId);

        modelBuilder.Entity<Client>()
            .HasMany(c => c.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.ClientId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Clients)
            .WithOne()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>()
            .HasMany(u => u.AuthorizationCodes)
            .WithOne()
            .HasForeignKey(ac => ac.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.AccessTokens)
            .WithOne()
            .HasForeignKey(at => at.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId);

        modelBuilder.Entity<AuthorizationCode>()
            .HasMany(ac => ac.RefreshTokens) // An AuthorizationCode can have multiple RefreshTokens
            .WithOne()
            .HasForeignKey(rt => rt.AuthorizationCodeId)
            .IsRequired() // Assuming this relationship is required for a RefreshToken
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                FirstName = "John",
                LastName = "Doe",
                UserId = "adminuserid",
                UserName = "admin",
                Email = "admin@sofco.org",
                PasswordHash = "admin"
            }
        );

        modelBuilder.Entity<Client>().HasData(
            new Client
            {
                ClientId = "sofcopayclientid",
                RedirectURIs = ["http://localhost:3000"],
                Scopes = ["openid"],
                UserId = "adminuserid"
            },
            new Client
            {
                ClientId = "sofcosmsclientid",
                RedirectURIs = ["http://localhost:3001"],
                Scopes = ["openid"],
                UserId = "adminuserid"
            }
        );

        modelBuilder.Entity<AuthorizationCode>().HasData(
            new AuthorizationCode
            {
                AuthorizationCodeId = 1,
                UserId = "adminuserid",
                ClientId = "sofcopayclientid",
                CodeChallenge = "authcode1challenge"
            },
            new AuthorizationCode
            {
                AuthorizationCodeId = 2,
                UserId = "adminuserid",
                ClientId = "sofcosmsclientid",
                CodeChallenge = "authcode2challenge"
            }
        );

        modelBuilder.Entity<AccessToken>().HasData(
            new AccessToken
            {
                AccessTokenId = 1,
                UserId = "adminuserid",
                ClientId = "sofcopayclientid",
            },
            new AccessToken
            {
                AccessTokenId = 2,
                UserId = "adminuserid",
                ClientId = "sofcosmsclientid",
            }
        );

        modelBuilder.Entity<RefreshToken>().HasData(
            new RefreshToken
            {
                RefreshTokenId = 1,
                UserId = "adminuserid",
                ClientId = "sofcopayclientid",
                AuthorizationCodeId = 1
            },
            new RefreshToken
            {
                RefreshTokenId = 2,
                UserId = "adminuserid",
                ClientId = "sofcosmsclientid",
                AuthorizationCodeId = 2
            }
        );
    }
}


