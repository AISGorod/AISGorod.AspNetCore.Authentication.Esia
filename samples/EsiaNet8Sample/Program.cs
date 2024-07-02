using AISGorod.AspNetCore.Authentication.Esia;
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
        options.Mnemonic = "RIAS-HCS-ULYANOVSK";
        options.Scope = ["fullname", "snils", "email", "mobile", "usr_org" ];
        options.SaveTokens = true;
        options.SecurityTokenValidator = new CustomSecurityTokenValidator();
    });

builder.Services.AddSingleton<IEsiaSigner, OpensslEsiaSigner>();

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