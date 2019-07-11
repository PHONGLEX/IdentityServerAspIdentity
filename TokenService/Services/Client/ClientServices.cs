using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Data;

namespace TokenService.Services.Client
{
    public class ClientServices : IClientServices
    {
        private IConfigurationDbContext _configurationDbContext;
        private ApplicationDbContext _applicationDbContext;
        private IAdminConfigurationDbContext _confContext;

        public ClientServices(IConfigurationDbContext configurationDbContext, ApplicationDbContext applicationDbContext, IAdminConfigurationDbContext confContext)
        {
            _configurationDbContext = configurationDbContext;
            _applicationDbContext = applicationDbContext;
            _confContext = confContext;
        }

        public async Task<int> Create(IdentityServer4.Models.Client client)
        {
            await _configurationDbContext.Clients.AddAsync(client.ToEntity());
            return await _configurationDbContext.SaveChangesAsync();
        }

        public async Task<int> Delete(string id)
        {
            var client = await _configurationDbContext.Clients.SingleOrDefaultAsync(c => c.ClientId.Equals(id));
            _configurationDbContext.Clients.Remove(client);
            return _configurationDbContext.SaveChanges();
        }

        public async Task<IEnumerable<IdentityServer4.Models.Client>> GetAll()
        {
            var clients = new List<IdentityServer4.Models.Client>();
            foreach (var item in _confContext.Clients)
            {
                var client = await _confContext.Clients.Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets).Where(x => x.Id == item.Id).SingleOrDefaultAsync();
                clients.Add(client.ToModel());
            }

            return clients;
        }
    }
}