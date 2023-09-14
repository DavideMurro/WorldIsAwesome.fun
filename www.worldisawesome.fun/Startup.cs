using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Globalization;
using www.worldisawesome.fun.Services;
using www.worldisawesome.fun.DBContext;
using Microsoft.AspNetCore.StaticFiles;

namespace www.worldisawesome.fun
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;


            // NReco FFMpeg
            NReco.VideoConverter.License.SetLicenseKey(
                    "Video_Converter_Bin_Examples_Pack_250390870865",
                    "hO9poEdO2qFYfrBrVOcn39yQmLRT69QDC67jvnY/rseDn5AIkVMpeNuUxvtAXmfgthtVErUluc5Lnn9JwK7ekog1lGQuiC57ythvuvhwJ2BVh2ILFbgQqdksAZfTmQ99DUpvlp9yAHcYCCT7v7qZ2rUnkmwWLS1zmW83aAiI+gA="
                );
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("es"),
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                // Formatting numbers, dates, etc.
                options.SupportedCultures = supportedCultures;
                // Localized UI strings.
                options.SupportedUICultures = supportedCultures;
            });

            services.AddDbContext<WorldIsAwesomeContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("WorldIsAwesomeContext")));
            //services.AddDistributedMemoryCache();

            // add authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();

            // add controllers
            //services.AddControllers();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseDeveloperExceptionPage();
            //app.UseDatabaseErrorPage();
            app.UseHsts();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/Api"), subApp =>
            {
                subApp.UseMiddleware(typeof(ErrorHandlingMiddleware));
            });
            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/Api"), subApp =>
            {
                subApp.UseExceptionHandler("/Error");
            });

            app.UseRequestLocalization();

            //app.UseHttpsRedirection();
            //app.UseDefaultFiles();
            //app.UseStaticFiles();
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Remove(".webmanifest");
            provider.Mappings.Remove(".css");
            provider.Mappings.Remove(".js");
            provider.Mappings.Remove(".svg");
            provider.Mappings.Add(".webmanifest", "application/manifest+json; charset=utf-8");
            provider.Mappings.Add(".css", "text/css; charset=utf-8");
            provider.Mappings.Add(".js", "application/javascript; charset=utf-8");
            provider.Mappings.Add(".svg", "image/svg+xml; charset=utf-8");
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=31536000,immutable";
                },
                ContentTypeProvider = provider
            });

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
