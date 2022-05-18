using Microsoft.AspNetCore.Builder;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Slice.Models;
using Slice.Repositories;

namespace Slice
{
    public class Startup
    {
        readonly string _POLICY_NAME = "_AllowAll";
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddAuthorization();

            services.AddCors(opt =>                
                opt.AddPolicy(_POLICY_NAME,
                builder =>
                {
                    builder.AllowAnyOrigin()
                      .SetIsOriginAllowedToAllowWildcardSubdomains()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                })
            ); // Adding CORS Secvices
            services.AddSingleton<SliceRepository>(); // adding repository
            services.AddSingleton( // adding formats
            new List<Format>()
            {
                new Format("A0", 841, 1189),
                new Format("A1", 594, 841),
                new Format("A2", 420, 594),
                new Format("A3", 297, 420),
                new Format("A4", 210, 297),
                new Format("A5", 148, 210),
                new Format("A6", 105, 148),
            }
            );
            //JwtAuth
            //services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
       
        }
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();

            // Allowing CORS
            app.UseCors(_POLICY_NAME);

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors(_POLICY_NAME);
            });
        }

    }
}
