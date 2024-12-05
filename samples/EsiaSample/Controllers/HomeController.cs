using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using EsiaSample.Models;

namespace EsiaSample.Controllers;

public class HomeController : Controller
{
    /// <summary>
    /// Настройки сериализации.
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    
    private readonly IEsiaRestService esiaRestService;
    private readonly IEsiaEnvironment esiaEnvironment;

    public HomeController(
        IEsiaRestService esiaRestService,
        IEsiaEnvironment esiaEnvironment)
    {
        this.esiaRestService = esiaRestService;
        this.esiaEnvironment = esiaEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        // This is what [Authorize] calls
        var userResult = await HttpContext.AuthenticateAsync();
        var props = userResult.Properties;
        ViewBag.UserProps = props?.Items;

        ViewBag.EsiaEnvironment = esiaEnvironment;

        return View();
    }

    public IActionResult LogIn(string scopes, bool orgSelect)
    {
        var callbackUrl = Url.Action(orgSelect ? "OrganizationSelect" : "Index", "Home");
        
        return Challenge(new OpenIdConnectChallengeProperties()
        {
            RedirectUri = callbackUrl,
            Scope = string.IsNullOrEmpty(scopes) ? null : scopes.Split(' ')
        }, "Esia");
    }

    [Authorize]
    public IActionResult LogOut()
    {
        return SignOut("Cookies", "Esia");
    }

    [Authorize]
    public async Task<IActionResult> Refresh()
    {
        await esiaRestService.RefreshTokensAsync();
        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult Api()
    {
        var oId = User.Claims.First(i => i.Type == "sub").Value;

        ViewBag.Url = $"/rs/prns/{oId}/ctts?embed=(elements)";
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Api(string url, string method)
    {
        var result = default(string);
        try
        {
            HttpMethod httpMethod = default(HttpMethod);
            switch (method)
            {
                case "get":
                    httpMethod = HttpMethod.Get;
                    break;
                case "post":
                    httpMethod = HttpMethod.Post;
                    break;
                case "put":
                    httpMethod = HttpMethod.Put;
                    break;
                case "delete":
                    httpMethod = HttpMethod.Delete;
                    break;
            }

            var resultJson = await esiaRestService.CallAsync(url, httpMethod);
            result = JsonSerializer.Serialize(resultJson, JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            result = ex.Message + "\r\n\r\n" + ex.StackTrace;
        }

        return Content(result);
    }

    [Authorize]
    public async Task<IActionResult> OrganizationSelect(int? id)
    {
        var oId = User.Claims.First(i => i.Type == "sub").Value;
        var model = new Models.OrganizationSelectViewModel();

        var organizations = await esiaRestService.CallAsync($"/rs/prns/{oId}/roles", HttpMethod.Get);
        var jsonString = organizations.GetProperty("elements").GetRawText();
        model.PersonRoles = JsonSerializer.Deserialize<List<EsiaPersonRoles>>(jsonString);

        if (id == null)
        {
            // selector render logic
            return View(model);
        }
        else
        {
            // org choice login

            var currentOrganization = model.PersonRoles.First(i => i.oid == id);
            var userInfo = await HttpContext.AuthenticateAsync("Esia");
            var identity = userInfo.Principal.Identity as ClaimsIdentity;

            var orgClaims = identity.Claims.Where(i => i.Type.StartsWith("urn:esia:org")).ToArray();
            foreach (var orgClaim in orgClaims)
            {
                identity.RemoveClaim(orgClaim);
            }

            identity.AddClaim(new Claim("urn:esia:org:oid", currentOrganization.oid.ToString()));
            identity.AddClaim(new Claim("urn:esia:org:fullName", currentOrganization.fullName));
            identity.AddClaim(new Claim("urn:esia:org:shortName", currentOrganization.shortName));
            identity.AddClaim(new Claim("urn:esia:org:ogrn", currentOrganization.ogrn));

            await HttpContext.SignInAsync(userInfo.Principal, userInfo.Properties);

            return RedirectToAction(nameof(Index));
        }
    }
}