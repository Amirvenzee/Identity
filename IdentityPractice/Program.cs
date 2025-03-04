using IdentityPractice.Context;
using IdentityPractice.Models;
using IdentityPractice.Repositories;
using IdentityPractice.Security;
using IdentityPractice.Srvices;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PersianTranslation.Identity;

namespace IdentityPractice
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<TestDbContext>(x =>
            {
                x.UseSqlServer(builder.Configuration.GetConnectionString("man"));
            });


            builder.Services.AddTransient<ISmsService, SmsService>();
            builder.Services.AddScoped<IMessageSender, MessageSender>();

            builder.Services.AddAuthentication().AddGoogle(option =>
            {
                option.ClientId = "490824423175-jh4bkgdgrmqio5ptp2rt6d1o6bahhs9k.apps.googleusercontent.com";
                option.ClientSecret = "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";
            });

            builder.Services.Configure<KavenegarInfo>(builder.Configuration.GetSection("KavenegarInfo"));


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option => 
            {

                option.Password.RequiredLength = 4;
                option.Password.RequiredUniqueChars = 0;
                option.Password.RequireNonAlphanumeric = false;
                option.User.RequireUniqueEmail = true;
                option.User.AllowedUserNameCharacters =
                   "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-";
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);

                option.Lockout.MaxFailedAccessAttempts = 5;


            }).AddEntityFrameworkStores<TestDbContext>().AddDefaultTokenProviders().AddErrorDescriber<PersianIdentityErrorDescriber>();


            builder.Services.ConfigureApplicationCookie(option =>
            {
              
                option.LoginPath = "/Login/Login";

               
            });

            builder.Services.AddAuthorization(option =>
            {
                option.AddPolicy("OnlyAdmin", context => context.RequireRole("Admin"));

            });



            //Set TimeSpan For SecurityStamp
            builder.Services.Configure<SecurityStampValidatorOptions>(option =>
            {
                option.ValidationInterval = TimeSpan.FromSeconds(5);
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
