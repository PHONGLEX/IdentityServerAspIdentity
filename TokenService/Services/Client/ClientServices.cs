using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenService.Services.Client
{
    public class ClientServices : IClientServices
    {
        private IConfigurationDbContext _configurationDbContext;

        public ClientServices(IConfigurationDbContext configurationDbContext)
        {
            _configurationDbContext = configurationDbContext;
        }

        public async Task<int> Create(IdentityServer4.Models.Client client)
        {
            await _configurationDbContext.Clients.AddAsync(client.ToEntity());
            return _configurationDbContext.SaveChanges();
        }

        public async Task<int> Delete(string id)
        {
            var client = await _configurationDbContext.Clients.SingleOrDefaultAsync(c => c.ClientId.Equals(id));
            _configurationDbContext.Clients.Remove(client);
            return _configurationDbContext.SaveChanges();
        }

        IEnumerable<IdentityServer4.Models.Client> IClientServices.GetAll()
        {
            var clients = new List<IdentityServer4.Models.Client>();

            foreach (var item in _configurationDbContext.Clients)
            {
                clients.Add(item.ToModel());
            }

            return clients;
        }
    }
}