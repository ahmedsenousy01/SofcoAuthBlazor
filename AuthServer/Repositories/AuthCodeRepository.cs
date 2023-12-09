using AuthServer.Data;
using AuthServer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Repositories
{
    public class AuthCodeRepository(ApplicationDbContext context, ILogger<ClientRepository> logger)
    {
        private readonly ApplicationDbContext db = context;
        private readonly ILogger<ClientRepository> logger = logger;

        public async Task<AuthorizationCode?> GetAuthCodeByIdAsync(int id)
        {
            return await db.AuthorizationCodes.FindAsync(id);
        }

        public async Task<List<AuthorizationCode>> GetUserAuthCodesAsync(string userId)
        {
            return await db.AuthorizationCodes.Where(ac => ac.UserId == userId).ToListAsync();
        }

        public async Task<List<AuthorizationCode>> GetClientAuthCodesAsync(string clientId)
        {
            return await db.AuthorizationCodes.Where(ac => ac.ClientId == clientId).ToListAsync();
        }

        public async Task<List<AuthorizationCode>> GetAuthCodesAsync()
        {
            return await db.AuthorizationCodes.ToListAsync();
        }

        public async Task<string?> GenerateAuthCodeAsync(string clientId, string userId, string codeChallenge, string codeChallengeMethod)
        {
            try
            {
                var authCode = new AuthorizationCode()
                {
                    ClientId = clientId,
                    UserId = userId,
                    CodeChallenge = codeChallenge,
                    CodeChallengeMethod = codeChallengeMethod
                };
                await db.AuthorizationCodes.AddAsync(authCode);
                await db.SaveChangesAsync();
                return authCode.AuthCodeString;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return null;
            }
        }
    }
}
