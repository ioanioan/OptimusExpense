using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptimusExpense.Data;


using Microsoft.AspNetCore.Http;
//using JavaScriptEngineSwitcher.V8;
//using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using React.AspNet;
using JavaScriptEngineSwitcher.V8;
using JavaScriptEngineSwitcher.Core;
using OptimusExpense.Data.Abstract;
using OptimusExpense.Data.Repositories;
using System;
using Microsoft.Extensions.Logging;
using OptimusExpense.Model.Models;
using OptimusExpense.Infrastucture.Exception;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using OptimusExpense.Infrastucture.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;

using System.Web;
using CookieAuthenticationOptions = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationOptions;
using System.Threading.Tasks;
using OptimusExpense.Timmer;

namespace OptimusExpense
{
    public class Startup
    {
        IWebHostEnvironment _env;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<OptimusExpenseContext>(options =>
               options.UseSqlServer(
                   Configuration.GetConnectionString("DefaultConnection")));



            #region Rep
            services.AddScoped<IUserActionRepository, UserActionRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IExpenseProjectRepository, ExpenseProjectRepository>();
            services.AddScoped<IExpenseNatureRepository, ExpenseNatureRepository>();
            services.AddScoped<IDictionaryDetailRepository, DictionaryDetailRepository>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IPartnerPointRepository, PartnerPointRepository>();
            services.AddScoped<IDocumentRepository, DocumentRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IExpenseReportRepository, ExpenseReportRepository>();
            services.AddScoped<IDocumentDetailRepository, DocumentDetailRepository>();
            services.AddScoped<IAspnetUsersRepository, AspnetUsersRepository>();
            services.AddScoped<IDocumentStateRepository, DocumentStateRepository>();
            services.AddScoped<IPropertyEntityValueRepository, PropertyEntityValueRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IExpenseAdvanceRepository, ExpenseAdvanceRepository>();
            services.AddScoped<ISerialNumberRepository, SerialNumberRepository>();
            services.AddScoped<IExpenseCostCenterRepository, ExpenseCostCenterRepository>();
            services.AddScoped<IEF_RaportareRepository, EF_RaportareRepository>();
            services.AddScoped<Ipck_OrderViewRepository, pck_OrderViewRepository>();
            services.AddScoped<Ipck_OrderLogRepository, pck_OrderLogRepository>();

            #endregion


            services.AddTransient<IEmailSender, EmailSender>(i =>
            new EmailSender(
                Configuration["EmailSender:Host"],
                Configuration.GetValue<int>("EmailSender:Port"),
                Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                Configuration["EmailSender:UserName"],
                Configuration["EmailSender:Password"], _env
            )
        );


            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<AspnetUsers>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            var url = Configuration["Url"];
            services.AddIdentityServer(o =>
            {
                o.IssuerUri = url;
                o.UserInteraction.LoginUrl = "/Identity/Account/Login";
                o.UserInteraction.LogoutUrl = "/Identity/Account/Logout";
            })
                .AddApiAuthorization<AspnetUsers, ApplicationDbContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerJwt().AddJwtBearer(o =>
                {

                    // Configuration.Bind("JwtBearer", o);
                    o.Authority = url;
                    o.Audience = "OptimusExpense";

                    //o.ApiName = "api2"; //api name
                    // o.Audience = "8d708afe-2966-40b7-918c-a39551625958";

                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        // ...
                        ValidateLifetime = true,
                        LifetimeValidator = (DateTime? notBefore, DateTime? expires, SecurityToken securityToken,
                                             TokenValidationParameters validationParameters) =>
                        {
                            return notBefore <= DateTime.UtcNow &&
                                   expires >= DateTime.UtcNow;
                        }, 
                        ClockSkew=TimeSpan.FromMinutes(100)
                    };
                });




            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {

                options.TokenLifespan = TimeSpan.FromDays(1002);
                //  options.
            });



            services.AddControllersWithViews();
            services.AddRazorPages();
            //  services.AddReact();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // JsEngineSwitcher.Current.DefaultEngineName = V8JsEngine.EngineName;
            //    JsEngineSwitcher.Current.EngineFactories.AddV8();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins(url,
                                            url);
                    });
            });

            services.AddControllers(options =>
    options.Filters.Add(new HttpResponseExceptionFilter()));

            services.AddScoped<ITimmerEFactura, TimmerEFactura>();
            //services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, ITimmerEFactura timmerEFactura)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseDeveloperExceptionPage();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            loggerFactory.AddFile("Logs/mylog-{Date}.txt");

            /*     app.UseReact(config =>
                 {

                 });*/




            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseCors();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            timmerEFactura.Start();

            //app.UseMvc(routes =>
            // {
            //     routes.MapRoute(
            //         name: "default",
            //         template: "{controller=Account}/{action=Login}/{id?}");
            // });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            // app.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
