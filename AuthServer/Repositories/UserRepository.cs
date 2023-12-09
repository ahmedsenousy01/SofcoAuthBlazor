using AuthServer.Data;
using AuthServer.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace AuthServer.Repositories;

public class UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
{
    private readonly ApplicationDbContext db = context;
    private readonly ILogger logger = logger;

    public async Task<User?> GetUserByIdAsync(string id)
    {
        return await db.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByUserNameAsync(string username)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await db.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await db.Users.ToListAsync();
    }

    public async Task<bool> CreateUserAsync(User u)
    {
        try
        {
            await db.Users.AddAsync(u);
            await db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task<bool> UpdateUserAsync(User updatedUser)
    {
        try
        {
            var existingUser = await db.Users.FindAsync(updatedUser.UserId);
            if (existingUser != null)
            {
                existingUser.UserName = updatedUser.UserName;
                existingUser.Email = updatedUser.Email;

                await db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        try
        {
            var userToDelete = await GetUserByIdAsync(id);
            if (userToDelete != null)
            {
                db.Users.Remove(userToDelete);
                await db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return false;
        }
    }

}
