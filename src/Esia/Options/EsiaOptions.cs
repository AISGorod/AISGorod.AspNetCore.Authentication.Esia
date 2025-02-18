﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using AISGorod.AspNetCore.Authentication.Esia.EsiaEnvironment;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AISGorod.AspNetCore.Authentication.Esia.Options;

/// <summary>
/// Параметры подключения к ЕСИА.
/// </summary>
public class EsiaOptions : RemoteAuthenticationOptions, IEsiaOptions
{
    /// <summary>
    /// Подпись настроена.
    /// </summary>
    private bool signerConfigured;

    /// <summary>
    /// Фабрика подписей.
    /// </summary>
    internal Func<IServiceProvider, IEsiaSigner>? SignerFactory { get; private set; }
    
    /// <summary>
    /// Конструктор по умолчанию.
    /// </summary>
    public EsiaOptions()
    {
        // Устанавливаем стандартную фабрику SignerFactory
        SignerFactory = sp => sp.GetRequiredService<IEsiaSigner>();
    }

    /// <inheritdoc />
    public EsiaEnvironmentType? Environment { get; set; }

    /// <inheritdoc />
    public new HttpClient? Backchannel { get; set; }

    /// <inheritdoc />
    public IEsiaEnvironment? EnvironmentInstance { get; set; }

    /// <inheritdoc />
    public string Mnemonic { get; set; } = string.Empty;

    /// <inheritdoc />
    public ICollection<string> Scope { get; set; } = [];

    /// <inheritdoc />
    public string? SignOutScheme { get; set; }

    /// <inheritdoc />
    public bool GetPrnsContactInformationOnSignIn { get; set; }

    /// <inheritdoc />
    public bool GetPrnsAddressesOnSignIn { get; set; }

    /// <inheritdoc />
    public bool GetPrnsDocumentsOnSignIn { get; set; }

    /// <inheritdoc />
    public TokenHandler TokenHandler { get; set; } = new JsonWebTokenHandler();

    /// <inheritdoc />
    public Action<HttpClient>? RestApiHttpClientHandler { get; set; }

    /// <inheritdoc />
    public new PathString CallbackPath { get; set; } = "/signin-esia";

    /// <inheritdoc />
    public PathString SignedOutCallbackPath { get; set; } = "/signout-esia";

    /// <inheritdoc />
    public void UseSigner(Func<IServiceProvider, IEsiaSigner> factory)
    {
        if (signerConfigured)
        {
            throw new InvalidOperationException("Можно настроить только один вариант подписи. Множественные варианты подписи не допускаются.");
        }

        SignerFactory = factory;
        signerConfigured = true;
    }
    
    /// <inheritdoc />
    public void UseSigner<TSigner>(IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TSigner : class, IEsiaSigner
    {
        if (signerConfigured)
        {
            throw new InvalidOperationException("Можно настроить только один вариант подписи. Множественные варианты подписи не допускаются.");
        }

        services.Add(new ServiceDescriptor(typeof(IEsiaSigner), typeof(TSigner), lifetime));

        // Устанавливаем фабрику для использования зарегистрированного сервиса
        SignerFactory = sp => sp.GetRequiredService<IEsiaSigner>();
        signerConfigured = true;
    }
}