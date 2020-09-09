using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Controllers;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoreCodeCamp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>();
            services.AddScoped<ICampRepository, CampRepository>();
            services.AddAutoMapper();
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true; // Gdy nie ma parametru w urlu bnedzie bral  domyslna wersje api
                opt.DefaultApiVersion = new ApiVersion(1, 1); //Domyslna versja kazdego api to 1.1
                opt.ReportApiVersions = true; // Dodaje Api version number do headera http
                opt.ApiVersionReader = ApiVersionReader.Combine( //Laczy kilka readerow moze byc dowolny z nich
                          new HeaderApiVersionReader("X-Version"), //Defaultowo bez podania szuka versji w query stringu - ta wersja szuka w headerze po twojej nazwie (tutaj)X-version
                                                                   //header values jest bardziej skomplikowane - latwiej dla innych jest query string(np.excell,js and hobyści)
                          new QueryStringApiVersionReader("ver", "version")); //query z "ver" i "version beda obslugiwane
            });
            services.AddMvc(opt => opt.EnableEndpointRouting = false) //trzeba dodac to w net 2.2(moze tez nastepnych) zeby api versioning dzialalo
              .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
