using AISGorod.AspNetCore.Authentication.Esia;
using AISGorod.AspNetCore.Authentication.Esia.BouncyCastle;
using EsiaNet8Sample;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Строчка для того, чтобы корректно указывались User.Claims
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies";
        options.DefaultChallengeScheme = "Esia";
    })
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "EsiaNet8Sample.Cookie";
    })
    .AddEsia<CustomEsiaEvents>("Esia", options =>
    {
        //options.Environment = EsiaEnvironmentType.Test;
        options.EnvironmentInstance = new CustomEsiaEnvironment();
        options.Mnemonic = "TESTSYS";
        options.Scope = ["fullname", "snils", "email", "mobile", "usr_org"];
        options.SaveTokens = true;
#pragma warning disable CS0618 // Type or member is obsolete
        options.TokenHandler = new CustomSecurityTokenValidator();
#pragma warning restore CS0618 // Type or member is obsolete
        
        options.CallBackPath = new PathString("/signin-esia");
        options.SignedOutCallbackPath = new PathString("/signout-esia");
        
        options.UseBouncyCastle(bouncyCastleOptions =>
        {
            bouncyCastleOptions.KeyFilePath = "/home/username/esia.key";
            bouncyCastleOptions.CertFilePath = "/home/username/esia.pem";
        });
    });

builder.Services.AddMvc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();