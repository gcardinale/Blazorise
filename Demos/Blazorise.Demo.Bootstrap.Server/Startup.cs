#region Using directives
using Blazorise.Demo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
#endregion

namespace Blazorise.Demo.Bootstrap.Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            services
                .AddMvc()
                .AddNewtonsoftJson();

            services.AddHttpClient();
            services.AddScoped<EmployeeData>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
        {
            if ( env.IsDevelopment() )
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }

            app.UseStaticFiles();
            app.UseBlazorFrameworkFiles();

            app.UseRouting();

            app.UseEndpoints( endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToFile( "index.html" );
            } );
        }
    }
}