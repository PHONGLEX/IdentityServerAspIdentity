using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Data;

namespace TokenService.Services.Client
{
    public class ClientServices : IClientServices
    {
        private IAdminConfigurationDbContext _confContext;

        public ClientServices(IAdminConfigurationDbContext confContext)
        {
            _confContext = confContext;
        }

        public async Task<int> Create(IdentityServer4.Models.Client client)
        {
            await _confContext.Clients.AddAsync(client.ToEntity());
            return await _confContext.SaveChangesAsync();
        }

        public async Task<int> Delete(string id)
        {
            var client = await _confContext.Clients.SingleOrDefaultAsync(c => c.ClientId.Equals(id));
            _confContext.Clients.Remove(client);
            return _confContext.SaveChanges();
        }

        public async Task<int> Edit(IdentityServer4.EntityFramework.Entities.Client client)
        {
            _confContext.Clients.Update(client);
            return await _confContext.SaveChangesAsync();           
        }

        public async Task<IEnumerable<IdentityServer4.Models.Client>> GetAll()
        {
            var clients = new List<IdentityServer4.Models.Client>();
            foreach (var item in _confContext.Clients)
            {
                var client = await _confContext.Clients.Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets).Include(x => x.RedirectUris).Where(x => x.Id == item.Id).SingleOrDefaultAsync();
                clients.Add(client.ToModel());
            }

            return clients;
        }

        public async Task<IdentityServer4.EntityFramework.Entities.Client> GetClientByClientId(string clientId)
        {
            return await _confContext.Clients.Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets).Include(x => x.RedirectUris).Where(x => x.ClientId == clientId).SingleOrDefaultAsync();
        }
    }
}