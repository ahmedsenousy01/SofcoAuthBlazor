using AuthServer.Data;
using AuthServer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Repositories
{
    public class ClientRepository(ApplicationDbContext context, ILogger<ClientRepository> logger)
    {
        private readonly ApplicationDbContext db = context;
        private readonly ILogger<ClientRepository> logger = logger;

        public async Task<Client?> GetClientByIdAsync(string id)
        {
            return await db.Clients.FindAsync(id);
        }

        public async Task<List<Client>> GetUserClientsAsync(string userId)
        {
            return await db.Clients.Where(c => c.UserId == userId).ToListAsync();
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            return await db.Clients.ToListAsync();
        }

        public async Task<bool> CreateClientAsync(Client c, string ownerId)
        {
            try
            {
                var owner = await db.Users.FindAsync(ownerId);
                if (owner != null)
                {
                    c.UserId = ownerId;
                    owner.Clients.Add(c);
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

        public async Task<bool> UpdateClientAsync(Client updatedClient, string ownerId)
        {
            try
            {
                var existingClient = await db.Clients.FindAsync(updatedClient.ClientId);

                if (existingClient != null && existingClient.UserId == ownerId)
                {
                    existingClient.RedirectURIs = updatedClient.RedirectURIs;
                    existingClient.Scopes = updatedClient.Scopes;

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

        public async Task<bool> DeleteClientAsync(string clientId, string ownerId)
        {
            try
            {
                var clientToDelete = await db.Clients.FindAsync(clientId);
                if (clientToDelete != null && clientToDelete.UserId == ownerId)
                {
                    db.Clients.Remove(clientToDelete);
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
}
