using System;
using System.Text;
using JWTIdentityClassLib.Data;
using JWTIdentityClassLib.Entities;
//using JWTClassLib.Helpers;
using JWTIdentityClassLib.Middleware;
using JWTIdentityClassLib.Services;
using JWTIdentityClassLib.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NewCore.Data.Context;
using NewCore.Services.CustomerServices;
using NewCore.Services.Interfaces;
using NewCore.Services.PolCvgServices;
using NewCore.Services.PolicyServices;

namespace NewCore.API
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
            // configure strongly typed settings object
            services.Configure<MailServiceSettings>(Configuration.GetSection("MailServiceSettings"));
            services.Configure<JWTSettings>(Configuration.GetSection("JWTSettings"));

            // Add Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add SQL Server services
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("IdentityDatabase"));
            });
            // Add NewCore dbcontext
            services.AddDbContext<NewCoreDataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("TestDBConnection"));
            });


            // Add Authentication
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration["JWTSettings:Issuer"],
                    ValidAudience = Configuration["JWTSettings:Audience"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });

            // configure DI for Identity & JWT application & email //services
            services.AddScoped<IUserService, UserService>()
                .AddScoped<IEmailService, EmailService>();

            // Dependency Injection - Services
            services.AddScoped<ICustomerServices, CustomerServices>()
                .AddScoped<IPolicyServices, PolicyServices>()
                .AddScoped<IPolCvgServices, PolCvgServices>();

            // services.AddControllers();
            services.AddControllers(options =>
               options.SuppressAsyncSuffixInActionNames = true
            ).AddJsonOptions(options =>
               options.JsonSerializerOptions.WriteIndented = true
            );

            // Add HTTP Client
            services.AddHttpClient();

            // Add CORS
            services.AddCors(options => {
                options.AddDefaultPolicy(
                    builder => builder.AllowAnyOrigin().AllowAnyMethod()
                    // with specify origin "http:.... must not end with '/'
                    // builder => builder.WithOrigins(
                    //     "http://localhost:5001",
                    //     "http://localhost:5100"
                    //     )
                    //     .WithMethods("GET","PUT","POST","DELETE")
                );
                // 
                // options.AddPolicy("mypolicy",
                //     builder => builder.WithOrigins("http://localhost:5001")                    
                // );

            });

            // Add health check
            //services.AddHealthChecks();
                
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // NOR - middleware CORS must be after userouting() and before useauthorization()
            //app.UseCors();  // use default
            app.UseCors(x => x
                .SetIsOriginAllowed(origin => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            //app.UseCors(x =>x
            //    .AllowAnyOrigin()
            //    .WithMethods("httppost")
            //    .AllowAnyHeader()
            //);
            //app.UseCors("mypolicy");  // use mypolicy

            app.UseAuthorization();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();
            // global error handler
            //app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
