using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using AISGorod.AspNetCore.Authentication.Esia.Options;

namespace AISGorod.AspNetCore.Authentication.Esia;

/// <summary>
/// Класс, реализующий обработчики событий от поставщика данных.
/// </summary>
public class EsiaEvents(IEsiaOptions esiaOptions, IEsiaEnvironment esiaEnvironment, IEsiaSigner esiaSigner) : OpenIdConnectEvents
{
    /// <summary>
    /// Добавление дополнительных параметров в запрос получения авторизационного кода
    /// </summary>
    protected virtual Task AddAdditionalParametersForReceivingAccessCode(IDictionary<string, string> parameters) => Task.CompletedTask;

    /// <summary>
    /// Событие перенаправления к поставщику данных.
    /// </summary>
    public override Task RedirectToIdentityProvider(RedirectContext context)
    {
        // prepare data
        var now = DateTimeOffset.Now;
        var pm = context.ProtocolMessage;

        // add additional fields for redirect
        pm.ResponseType = OpenIdConnectResponseType.Code;
        pm.Parameters.Add("access_type", "offline");
        pm.Parameters.Add("timestamp", now.ToString("yyyy.MM.dd HH:mm:ss") + " " + now.ToString("zzz").Replace(":", ""));
        pm.Parameters.Add("client_certificate_hash", esiaSigner.GetCertificateFingerprint());
        pm.State = Guid.NewGuid().ToString();

        // get data for sign
        var scope = pm.Parameters["scope"];
        var timestamp = pm.Parameters["timestamp"];
        var clientId = pm.ClientId;
        var state = pm.State;
        var redirectUri = pm.RedirectUri;

        pm.ClientSecret = esiaSigner.Sign($"{clientId}{scope}{timestamp}{state}{redirectUri}");

        return AddAdditionalParametersForReceivingAccessCode(pm.Parameters);
    }

    /// <summary>
    /// Событие получения маркера доступа и(или) маркера идентификации.
    /// </summary>
    public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        // prepare data
        var now = DateTimeOffset.Now;
        var pm = context.TokenEndpointRequest;

        // add additional fields for redirect
        if (pm == null)
        {
            return Task.CompletedTask;
        }

        pm.ClientId = context.Options.ClientId;
        pm.Parameters.Add("scope", string.Join(" ", (context.Properties as OpenIdConnectChallengeProperties)?.Scope ?? context.Options.Scope));
        pm.Parameters.Add("timestamp", now.ToString("yyyy.MM.dd HH:mm:ss") + " " + now.ToString("zzz").Replace(":", ""));
        pm.Parameters.Add("client_certificate_hash", esiaSigner.GetCertificateFingerprint());
        pm.State = Guid.NewGuid().ToString();

        // get data for sign
        var scope = pm.Parameters["scope"];
        var timestamp = pm.Parameters["timestamp"];
        var clientId = pm.ClientId;
        var state = pm.State;
        var redirectUri = pm.RedirectUri;
        var code = pm.Code;

        // set clientSecret
        if (clientId != null)
        {
            pm.ClientSecret = esiaSigner.Sign($"{clientId}{scope}{timestamp}{state}{redirectUri}{code}");
        }

        // ok
        return Task.CompletedTask;
    }

    /// <summary>
    /// Событие получения маркера доступа.
    /// </summary>
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        // We cannot use default UserInfoEndpoint because there are {oId} in uri, BLYATTTT!

        ArgumentNullException.ThrowIfNull(context.TokenEndpointResponse, nameof(context.TokenEndpointResponse));

        var userOid = context.SecurityToken.Subject;
        ArgumentNullException.ThrowIfNull(userOid, nameof(userOid));

        var httpClient = context.Options.Backchannel;
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));

        var claimsIdentity = (ClaimsIdentity?)context.Principal?.Identity;
        ArgumentNullException.ThrowIfNull(claimsIdentity, nameof(claimsIdentity));

        // Get and fill main info.
        var prnsResponse = await CallRestEndpointAsync(httpClient, userOid, context.TokenEndpointResponse, HttpMethod.Get);
        var prnsClaimAction = new MapAllClaimsAction();
        prnsClaimAction.Run(prnsResponse, claimsIdentity, "esia_prns");

        // Get and fill contact information.
        if (esiaOptions.GetPrnsContactInformationOnSignIn)
        {
            var contactsResponse = await CallRestEndpointAsync(httpClient, userOid, context.TokenEndpointResponse, HttpMethod.Get, "/ctts?embed=(elements)");
            var contactsClaimsAction = new JsonKeyClaimAction(EsiaDefaults.PrnsCttsClaimType, ClaimValueTypes.String, "elements");
            contactsClaimsAction.Run(contactsResponse, claimsIdentity, "esia_prns_ctts");
        }

        // Get and fill addresses.
        if (esiaOptions.GetPrnsAddressesOnSignIn)
        {
            var addressesResponse = await CallRestEndpointAsync(httpClient, userOid, context.TokenEndpointResponse, HttpMethod.Get, "/addrs?embed=(elements)");
            var addressesClaimsAction = new JsonKeyClaimAction(EsiaDefaults.PrnsAddrsClaimType, ClaimValueTypes.String, "elements");
            addressesClaimsAction.Run(addressesResponse, claimsIdentity, "esia_prns_addrs");
        }

        // Get and fill documents.
        if (esiaOptions.GetPrnsDocumentsOnSignIn)
        {
            var documentsResponse = await CallRestEndpointAsync(httpClient, userOid, context.TokenEndpointResponse, HttpMethod.Get, "/docs?embed=(elements)");
            var documentsClaimsAction = new JsonKeyClaimAction(EsiaDefaults.PrnsDocsClaimType, ClaimValueTypes.String, "elements");
            documentsClaimsAction.Run(documentsResponse, claimsIdentity, "esia_prns_docs");
        }

        // Fill scopes.
        context.Properties?.SetString(EsiaDefaults.EnablesScopesPropertiesKey, string.Join(" ", (context.Properties as OpenIdConnectChallengeProperties)?.Scope ?? context.Options.Scope));
    }

    /// <summary>
    /// Событие перенаправления к поставщику данных для логаута.
    /// </summary>
    public override Task RedirectToIdentityProviderForSignOut(RedirectContext context)
    {
        var pm = context.ProtocolMessage;
        pm.ClientId = context.Options.ClientId;
        pm.PostLogoutRedirectUri = null;
        pm.Parameters.Add("redirect_url", context.Properties.RedirectUri); // THERE ARE DEFAULT redirect_uri param, blyat!!!
        return Task.CompletedTask;
    }

    private async Task<JsonElement> CallRestEndpointAsync(
        HttpClient httpClient,
        string userOid,
        OpenIdConnectMessage tokenEndpointResponse,
        HttpMethod method,
        string? suffix = default)
    {
        try
        {
            var httpRequest = new HttpRequestMessage(method, esiaEnvironment.RestPersonsEndpoint + userOid + suffix);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue(tokenEndpointResponse.TokenType, tokenEndpointResponse.AccessToken);
            var restResponse = await httpClient.SendAsync(httpRequest);
            restResponse.EnsureSuccessStatusCode();
            using var doc = JsonDocument.Parse(await restResponse.Content.ReadAsStringAsync());
            return doc.RootElement.Clone();
        }
        catch (Exception ex)
        {
            throw new EsiaRestRequestException($"Ошибка запроса к адресу {esiaEnvironment.RestPersonsEndpoint + userOid + suffix} для получения данных о пользователе", ex);
        }
    }
}