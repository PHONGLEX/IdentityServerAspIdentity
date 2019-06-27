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
        private IClientStore _clientStore;
        private IClientServices _clientServices;

        public ClientController(IClientStore clientStore, IClientServices clientServices)
        {
            _clientStore = clientStore;
            _clientServices = clientServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetList()
        {
            var clients = _clientServices.GetAll();

            return Json(new
            {
                data = clients.Select(c => new
                {
                    Id = c.ClientId,
                    Name = c.ClientName,
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
    }
}