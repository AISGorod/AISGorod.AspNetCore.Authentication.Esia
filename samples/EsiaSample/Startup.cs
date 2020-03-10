using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EsiaSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddHttpContextAccessor();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "Esia";
                })
                .AddCookie("Cookies", options =>
                {
                    options.Cookie.Name = "EsiaSample.Cookie";
                })
                .AddEsia("Esia", options =>
                {
                    //options.Environment = EsiaEnvironmentType.Test;
                    options.EnvironmentInstance = new CustomEsiaEnvironment();
                    options.Mnemonic = "TESTSYS";
                    options.Scope = new[] { "fullname", "snils", "email", "mobile", "usr_org" };
                    options.SaveTokens = true;
                });
            services.AddSingleton<IEsiaSigner, OpensslEsiaSigner>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
