using DotNetWebApi.Common.Utilities;
using DotNetWebApi.IocConfig.Middlewares;
using DotNetWebApi.IocConfig.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DotNetWebApi.IocConfig.Configuration
{
    public static class AppConfigRegistry
    {
        public static void AddAppConfigs(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            Assert.NotNull(app, nameof(app));
            Assert.NotNull(env, nameof(env));

            //app.IntializeDatabase();

            app.UseCustomExceptionHandler();

            if (!env.IsDevelopment())
                app.UseHsts();

            app.UseHttpsRedirection();

            app.UseAntiDos();

            app.UseSwaggerAndUi();

            app.UseRouting();

            app.UseAuthentication();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseStaticFiles();

            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);
        }


        public static void AddAppRoutes(this IApplicationBuilder app)
        {
            app.UseEndpoints(config =>
            {
                config.MapControllers();
            });
        }
    }
}
