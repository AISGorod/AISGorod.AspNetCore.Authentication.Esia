using System.Security.Claims;
using System.Text.Json;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using AISGorod.AspNetCore.Authentication.Esia.EsiaServices;
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
public class HomeController(IEsiaRestService esiaRestService, IEsiaEnvironment esiaEnvironment) : Controller
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
        var authProperties = (await HttpContext.AuthenticateAsync()).Properties;
        ViewBag.UserProps = authProperties?.Items;
        ViewBag.EsiaEnvironment = esiaEnvironment;

        return View();
    }

    /// <summary>
    /// Авторизация.
    /// </summary>
    public IActionResult LogIn(string scopes, bool orgSelect)
    {
        var redirectUri = Url.Action(orgSelect ? nameof(OrganizationSelect) : nameof(Index));
        var scopeList = string.IsNullOrEmpty(scopes) ? ["fullname", "snils", "email", "mobile", "usr_org"] : scopes.Split(' ');

        return Challenge(new OpenIdConnectChallengeProperties
        {
            RedirectUri = redirectUri,
            Scope = scopeList
        }, "Esia");
    }

    /// <summary>
    /// Выход из аккаунта.
    /// </summary>
    [Authorize]
    public IActionResult LogOut() => SignOut("Cookies", "Esia");

    /// <summary>
    /// Обновление токенов.
    /// </summary>
    [Authorize]
    public async Task<IActionResult> Refresh()
    {
        await esiaRestService.RefreshTokensAsync();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Тест API.
    /// </summary>
    [Authorize]
    public IActionResult Api()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        ViewBag.Url = $"/rs/prns/{userId}/ctts?embed=(elements)";
        return View();
    }

    /// <summary>
    /// Вызов API.
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Api(string url, string method)
    {
        try
        {
            var httpMethod = ParseHttpMethod(method);
            var resultJson = await esiaRestService.CallAsync(url, httpMethod);
            var result = JsonSerializer.Serialize(resultJson, JsonSerializerOptions);

            return Content(result);
        }
        catch (Exception ex)
        {
            return Content($"{ex.Message}\r\n\r\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Выбор организации.
    /// </summary>
    [Authorize]
    public async Task<IActionResult> OrganizationSelect(int? id)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        if (userId != null)
        {
            var model = new OrganizationSelectViewModel
            {
                PersonRoles = await FetchPersonRolesAsync(userId)
            };

            if (id == null)
            {
                return View(model);
            }

            await UpdateUserClaimsAsync(model.PersonRoles.FirstOrDefault(r => r.Id == id));
        }

        return RedirectToAction(nameof(Index));
    }

    private static HttpMethod ParseHttpMethod(string method) =>
        method.ToLower() switch
        {
            "get" => HttpMethod.Get,
            "post" => HttpMethod.Post,
            "put" => HttpMethod.Put,
            "delete" => HttpMethod.Delete,
            _ => HttpMethod.Get
        };

    private async Task<List<EsiaPersonRoles>> FetchPersonRolesAsync(string userId)
    {
        var response = await esiaRestService.CallAsync($"/rs/prns/{userId}/roles", HttpMethod.Get);
        var rolesJson = response.GetProperty("elements").GetRawText();
        return JsonSerializer.Deserialize<List<EsiaPersonRoles>>(rolesJson) ?? new List<EsiaPersonRoles>();
    }

    private async Task UpdateUserClaimsAsync(EsiaPersonRoles? organization)
    {
        if (organization == null)
        {
            return;
        }

        var userInfo = await HttpContext.AuthenticateAsync("Esia");
        var identity = userInfo.Principal?.Identity as ClaimsIdentity;

        if (identity == null)
        {
            return;
        }

        // Удаляем старые claims организации
        var existingOrgClaims = identity.Claims.Where(c => c.Type.StartsWith("urn:esia:org")).ToArray();
        foreach (var claim in existingOrgClaims)
        {
            identity.RemoveClaim(claim);
        }

        // Добавляем новые claims
        AddClaimIfNotNull(identity, "urn:esia:org:oid", organization.Id.ToString());
        AddClaimIfNotNull(identity, "urn:esia:org:fullName", organization.FullName);
        AddClaimIfNotNull(identity, "urn:esia:org:shortName", organization.ShortName);
        AddClaimIfNotNull(identity, "urn:esia:org:ogrn", organization.Ogrn);

        if (userInfo.Principal != null)
        {
            await HttpContext.SignInAsync(userInfo.Principal, userInfo.Properties);
        }
    }

    private static void AddClaimIfNotNull(ClaimsIdentity identity, string type, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            identity.AddClaim(new Claim(type, value));
        }
    }
}