using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ACA.Data;
using ACA.Domain;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ACA.Classes.Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(o=>
            {
                o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddServerSideBlazor();
            services.AddTelerikBlazor();
            //https://forums.asp.net/t/2158728.aspx?Blazor+in+load+balanced+environment+
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            var useS3 = Configuration.GetValue<bool>("UseS3", false);
            if (useS3)
            {
                services.AddSingleton<ICsvDataFileService, S3CsvDataFileService>();
                services.Configure<S3CsvDataFileConfiguration>(Configuration.GetSection("S3CsvDataFileConfiguration"));
                services.AddSingleton<ICsvDataFileConfiguration>(resolver =>
                    resolver.GetRequiredService<IOptions<S3CsvDataFileConfiguration>>().Value);
            }
            else
            {
                services.AddSingleton<ICsvDataFileService, CsvDataFileService>();
                services.Configure<CsvDataFileConfiguration>(Configuration.GetSection("CsvDataFileConfiguration"));
                services.AddSingleton<ICsvDataFileConfiguration>(resolver =>
                    resolver.GetRequiredService<IOptions<CsvDataFileConfiguration>>().Value);
            }

            services.AddSingleton<IScoreReportService, ScoreReportService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseMvcWithDefaultRoute();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            //run validation here to identify invalid configurations
            var validatables = app.ApplicationServices.GetServices<IValidatable>();
            foreach (var validatable in validatables)
            {
                validatable.Validate();
            }
        }
    }
}
