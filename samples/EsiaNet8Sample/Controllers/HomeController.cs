using System.Security.Claims;
using System.Text.Json;
using AISGorod.AspNetCore.Authentication.Esia;
using EsiaNet8Sample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EsiaNet8Sample.Controllers;

/// <summary>
/// Главный контроллер.
/// </summary>
/// <param name="esiaRestService">Сервис выполнения запросов к сервисам ЕСИА на базе подхода REST.</param>
/// <param name="esiaEnvironment">Настройка среды ЕСИА.</param>
public class HomeController(
    IEsiaRestService esiaRestService,
    IEsiaEnvironment esiaEnvironment) : Controller
{
    /// <summary>
    /// Настройки сериализации.
    /// </summary>
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };
    
    /// <summary>
    /// Главная страница.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        // This is what [Authorize] calls
        var authenticationProperties = (await HttpContext.AuthenticateAsync()).Properties;
        if (authenticationProperties != null)
        {
            ViewBag.UserProps = authenticationProperties.Items;
        }

        ViewBag.EsiaEnvironment = esiaEnvironment;

        return View();
    }

    /// <summary>
    /// Авторизация.
    /// </summary>
    /// <param name="scopes">Сферы.</param>
    /// <param name="orgSelect">Выбрана организация.</param>
    public IActionResult LogIn(string scopes, bool orgSelect)
    {
        var callbackUrl = Url.Action(orgSelect ? nameof(OrganizationSelect) : nameof(Index));
        return Challenge(new OpenIdConnectChallengeProperties
        {
            RedirectUri = callbackUrl,
            Scope = string.IsNullOrEmpty(scopes) ? ["fullname", "snils", "email", "mobile", "usr_org" ] : scopes.Split(' ')
        }, "Esia");
    }

    /// <summary>
    /// Выход из аккаунта. 
    /// </summary>
    [Authorize]
    public IActionResult LogOut()
    {
        return SignOut("Cookies", "Esia");
    }

    /// <summary>
    /// Обновление.
    /// </summary>
    [Authorize]
    public async Task<IActionResult> Refresh()
    {
        await esiaRestService.RefreshTokensAsync();
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Тест API.
    /// </summary>
    [Authorize]
    public IActionResult Api()
    {
        var oId = User.Claims.First(i => i.Type == "sub").Value;

        ViewBag.Url = $"/rs/prns/{oId}/ctts?embed=(elements)";
        return View();
    }
    
    /// <summary>
    /// Тест API
    /// </summary>
    /// <param name="url">Путь до веб-сервиса относительно корня сайта.</param>
    /// <param name="method">Http метод.</param>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Api(string url, string method)
    {
        string result;
        // Включение отступов
        try
        {
            var httpMethod = method switch
            {
                "get" => HttpMethod.Get,
                "post" => HttpMethod.Post,
                "put" => HttpMethod.Put,
                "delete" => HttpMethod.Delete,
                _ => default
            };

            var resultJson = await esiaRestService.CallAsync(url, httpMethod);
            result = JsonSerializer.Serialize(resultJson, JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            result = ex.Message + "\r\n\r\n" + ex.StackTrace;
        }

        return Content(result);
    }

    /// <summary>
    /// Выбор организации.
    /// </summary>
    /// <param name="id">Идентификатор организации.</param>
    [Authorize]
    public async Task<IActionResult> OrganizationSelect(int? id)
    {
        var oId = User.Claims.FirstOrDefault(i => i.Type == "sub")?.Value;
        var model = new OrganizationSelectViewModel();

        var organizations = await esiaRestService.CallAsync($"/rs/prns/{oId}/roles", HttpMethod.Get);
        var jsonString = organizations.GetProperty("elements").GetRawText();
        model.PersonRoles = JsonSerializer.Deserialize<List<EsiaPersonRoles>>(jsonString);

        if (id == null)
        {
            // selector render logic
            return View(model);
        }
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