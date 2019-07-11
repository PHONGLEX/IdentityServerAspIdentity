using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TokenService.Extensions;
using TokenService.Services.Client;

namespace TokenService.Controllers
{
    public class ClientController : Controller
    {
        private IClientServices _clientServices;

        public ClientController(IClientServices clientServices)
        {
            _clientServices = clientServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetList()
        {
            var clients = await _clientServices.GetAll();

            return Json(new
            {
                data = clients.Where(c => c != null).Select(c => new
                {
                    Id = c.ClientId,
                    Name = c.ClientName,
                    Secrets = c.ClientSecrets,
                    RedirectUri = c.RedirectUris,
                    Scope = c.AllowedScopes,
                    c.AllowOfflineAccess,
                    c.AlwaysIncludeUserClaimsInIdToken
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(IdentityServer4.Models.Client client, List<KeyValuePair<string, string>> secrets)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var s in secrets)
                    {
                        client.AddSecret(new IdentityServer4.Models.Secret
                        {
                            Value = s.Key.Sha256(),
                            Expiration = DateTime.Parse(s.Value)
                        });
                    }

                    var scopes = client.AllowedScopes?.FirstOrDefault().Split(',');
                    if (scopes != null && scopes.Count() > 0)
                    {
                        client.AllowedScopes = new Collection<string>();
                        foreach(string s in scopes)
                        {
                            client.AllowedScopes.Add(s);
                        }
                    }
                    // the access token (and its claims) should be updated on a refresh token request
                    client.UpdateAccessTokenClaimsOnRefresh = true;

                    var result = await _clientServices.Create(client);
                    return Json(result);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var result = await _clientServices.Delete(id);
                    return Json(result);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityServer4.Models.Client client, List<KeyValuePair<string, string>> secrets)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var s in secrets)
                    {
                        client.AddSecret(new IdentityServer4.Models.Secret
                        {
                            Value = s.Key.Sha256(),
                            Expiration = DateTime.Parse(s.Value)
                        });
                    }

                    var scopes = client.AllowedScopes?.FirstOrDefault().Split(',');
                    if (scopes != null && scopes.Count() > 0)
                    {
                        client.AllowedScopes = new Collection<string>();
                        foreach (string s in scopes)
                        {
                            client.AllowedScopes.Add(s);
                        }
                    }
                    // the access token (and its claims) should be updated on a refresh token request
                    client.UpdateAccessTokenClaimsOnRefresh = true;


                    var result = await _clientServices.Edit(await ConvertModelToEntity(client));
                    return Json(result);
                }
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        #region private method
        async Task<IdentityServer4.EntityFramework.Entities.Client> ConvertModelToEntity(IdentityServer4.Models.Client client)
        {
            var _client = await _clientServices.GetClientByClientId(client.ClientId);

            _client.ClientName = client.ClientName;
            _client.AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken;
            _client.AllowOfflineAccess = client.AllowOfflineAccess;

            var RedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>();
            var ClientSecrets = new List<IdentityServer4.EntityFramework.Entities.ClientSecret>();
            var AllowedScopes = new List<IdentityServer4.EntityFramework.Entities.ClientScope>();
            foreach (var item in client.ClientSecrets)
            {
                ClientSecrets.Add(new IdentityServer4.EntityFramework.Entities.ClientSecret
                {
                    Value = item.Value,
                    Expiration = item.Expiration
                });
            }

            foreach (var scope in client.AllowedScopes)
            {
                AllowedScopes.Add(new IdentityServer4.EntityFramework.Entities.ClientScope
                {
                    Scope = scope,
                    ClientId = _client.Id
                });
            }

            foreach (var ruri in client.RedirectUris)
            {
                RedirectUris.Add(new IdentityServer4.EntityFramework.Entities.ClientRedirectUri
                {
                    RedirectUri = ruri,
                    ClientId = _client.Id
                });
            }

            _client.RedirectUris = RedirectUris;
            _client.ClientSecrets = ClientSecrets;
            _client.AllowedScopes = AllowedScopes;

            return _client;
        }
        #endregion
    }
}