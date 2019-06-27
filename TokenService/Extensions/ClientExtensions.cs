using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenService.Extensions
{
    public static class ClientExtensions
    {
        public static void AddSecret(this IdentityServer4.Models.Client client, IdentityServer4.Models.Secret secret)
        {
            client.ClientSecrets.Add(secret);
        }
    }
}
