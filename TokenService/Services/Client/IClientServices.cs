using System.Collections.Generic;
using System.Threading.Tasks;

namespace TokenService.Services.Client
{
    public interface IClientServices
    {
        Task<IEnumerable<IdentityServer4.Models.Client>> GetAll();
        Task<int> Create(IdentityServer4.Models.Client client);
        Task<int> Edit(IdentityServer4.EntityFramework.Entities.Client client);
        Task<int> Delete(string id);
        Task<IdentityServer4.EntityFramework.Entities.Client> GetClientByClientId(string clientId);
    }
}