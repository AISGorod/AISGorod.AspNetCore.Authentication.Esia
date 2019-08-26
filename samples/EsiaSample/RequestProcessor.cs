using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EsiaSample
{
    /// <summary>
    /// Обработчик запросов пользователей.
    /// Нет смысла использовать мощь MVC+Razor для всего лишь демонстрации.
    /// </summary>
    static class RequestProcessor
    {
        public static async Task ProcessAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var identity = context.User.Identity as ClaimsIdentity;
            var isAuthenticated = (identity?.IsAuthenticated == true);

            context.Response.ContentType = "text/html;charset=UTF-8";

            if (path == "/")
            {
                await WriteHeaderAsync(context);

                await context.Response.WriteAsync($@"
<ul class='nav'>
    {NavLink("/signin", $"Войти через {EsiaDefaults.DisplayName} - все права")}
    {NavLink("/signin-2", $"Войти через {EsiaDefaults.DisplayName} - только fullname")}
</ul>
");

                if (isAuthenticated)
                {
                    await context.Response.WriteAsync("<h3>Claims</h3>");
                    foreach (var claim in identity.Claims)
                    {
                        await context.Response.WriteAsync($"<b>{claim.Type}</b>: {claim.Value}<br/>");
                    }

                    var userInfo = await context.AuthenticateAsync();
                    await context.Response.WriteAsync("<h3>Properties</h3>");
                    foreach (var prop in userInfo.Properties.Items)
                    {
                        await context.Response.WriteAsync($"<b>{prop.Key}</b>: {prop.Value}<br/>");
                    }
                }
                else
                {

                }
                await WriteFooterAsync(context);
            }
            else if (path == "/signin")
            {
                await context.ChallengeAsync("Esia", new AuthenticationProperties()
                {
                    RedirectUri = "/"
                });
            }
            else if (path == "/signin-2")
            {
                await context.ChallengeAsync("Esia", new OpenIdConnectChallengeProperties()
                {
                    RedirectUri = "/",
                    Scope = new[] { "openid", "fullname" }
                });
            }
            else if (path == "/signout")
            {
                await context.SignOutAsync("Cookies");
                await context.SignOutAsync("Esia");
            }
            else if (path == "/refresh")
            {
                var service = (IEsiaRestService)context.RequestServices.GetService(typeof(IEsiaRestService));
                await service.RefreshTokensAsync();
                context.Response.Redirect("/");
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }

        private static async Task WriteHeaderAsync(HttpContext context)
        {
            await context.Response.WriteAsync($@"<!DOCTYPE html>
<html>
<head>
    <link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' integrity='sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T' crossorigin='anonymous'>
    <script src='https://code.jquery.com/jquery-3.3.1.slim.min.js' integrity='sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo' crossorigin='anonymous'></script>
    <script src='https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js' integrity='sha384-UO2eT0CpHqdSJQ6hJty5KVphtPhzWj9WO1clHTMGa3JDZwrnQq4sF86dIHNDz0W1' crossorigin='anonymous'></script>
    <script src='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js' integrity='sha384-JjSmVgyd0p3pXB1rRibZUAYoIIy6OrQ6VrjIEaFf/nJGzIxFDsf4x0xIM+B07jRM' crossorigin='anonymous'></script>
</head>
<body>
    <nav class='navbar navbar-expand-lg navbar-light bg-light'>
        <ul class='navbar-nav mr-auto'>
            {NavLink("/", "Информация о пользователе")}
            {NavLink("/api", "Проверка отправки запросов API")}
            {NavLink("/refresh", "Обновить маркер доступа")}
            {NavLink("/signout", "Выйти из ЕСИА")}
        </ul>
    </nav>
    <div class='container'>");
        }

        private static async Task WriteFooterAsync(HttpContext context)
        {
            await context.Response.WriteAsync("</div>");
            await context.Response.WriteAsync("</body></html>");
        }

        private static string NavLink(string url, string text)
        {
            return $@"
<li class='nav-item'>
    <a class='nav-link' href='{url}'>{text}</a>
</li>";
        }
    }
}
