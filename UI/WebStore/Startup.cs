﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SmartBreadcrumbs;
using WebStore.Clients.Services;
using WebStore.Controllers;
using WebStore.DAL.Context;
using WebStore.Entities.Entities.Identity;
using WebStore.Infrastructure.Implementations;
using WebStore.Infrastructure.Implementations.CookieCartService;
using WebStore.Infrastructure.Sql;
using WebStore.Infrastructure.Sql.Admin;
using WebStore.Interfaces;

namespace WebStore
{
    public class Startup
    {
        /// <summary>
        /// Добавляем свойство для доступа к конфигурации
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Добавляем новый конструктор, принимающий интерфейс IConfiguration
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Добавляем сервисы, необходимые для mvc
            services.AddMvc();

            //BreadCrumbs
            services.UseBreadcrumbs(GetType().Assembly);

            //Добавляем разрешение зависимости
            services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();
            services.AddScoped<IProductData, SqlProductData>();
            services.AddScoped<IOrdersService, SqlOrdersService>();
            services.AddScoped<IProductDataAdmin, SqlProductDataAdmin>();
            services.AddScoped<IOrdersServiceAdmin, SqlOrdersServiceAdmin>();

            // Добавляем реализацию клиента
            services.AddTransient<IValuesService, ValuesClient>();

            services.AddDbContext<WebStoreContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<WebStoreContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;


                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                //options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });

            //Настройки для корзины
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICartService, CookieCartService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Проверка среды выполнения, если dev то выкидывать ошибку в браузере
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // подключаем файлы по умолчанию
            app.UseDefaultFiles();
            // подключаем статические файлы
            app.UseStaticFiles();
            //Welcome page
            app.UseWelcomePage("/welcome");
            //Autentification
            app.UseAuthentication();
            //Маршрутизатор
            app.UseMvc(routes => 
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
        }
    }
}
