using System;
using System.Threading.Tasks;
using Demo.Authentication.Authentication;
using Demo.Authentication.Data;
using Demo.Authentication.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Demo.Authentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            // Добавляем аутентификацию
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                    options.SlidingExpiration = true;
                    options.Events.OnSignedIn = context =>
                    {
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToAccessDenied = context => {
                        context.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    };
                    options.Events.OnRedirectToLogin = context => {
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    };
                    //options.SessionStore = new CustomTicketStore();
                })
                .AddScheme<AuthSchemeOptions, AuthSchemeHandler>("MyCustomScheme", options =>
                {
                });
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthorization(opts => {
                /*
                opts.AddPolicy("AspManager", policy => {
                    policy.RequireRole("Admin");
                    policy.RequireClaim("Coding-Skill", "ASP.NET Core MVC");
                });
                */
            });
            
            services.AddSingleton<Database>();
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
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            // Добавляем аутентификацию и авторизацию в обработку запросов
            app.UseAuthentication(); //Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationHandler
            app.UseCustomMiddleware();
            app.UseAuthorization();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                });

            app.UseSpa(
                spa =>
                {
                    // To learn more about options for serving an Angular SPA from ASP.NET Core,
                    // see https://go.microsoft.com/fwlink/?linkid=864501

                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
                });
        }
    }
}
