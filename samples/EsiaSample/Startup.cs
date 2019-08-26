using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AISGorod.AspNetCore.Authentication.Esia;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
                    options.Environment = EsiaEnvironmentType.Test;
                    options.Mnemonic = "TESTSYS";
                    options.Certificate = () => new X509Certificate2(System.IO.File.ReadAllBytes(@"c\esia.pfx"), "");
                    options.Scope = new[] { "fullname", "snils", "email", "mobile" };
                });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}
