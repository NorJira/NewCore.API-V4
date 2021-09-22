using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewCore.Data;
using NewCore.Data.Context;
using NewCore.Services;
// using NewCore.Services;
using NewCore.Services.CustomerServices;
using NewCore.Services.Interfaces;
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

            // services.AddControllers();
            services.AddControllers(options =>
               options.SuppressAsyncSuffixInActionNames = true
            ).AddJsonOptions(options =>
               options.JsonSerializerOptions.WriteIndented = true
            );
            // Add Data Layer
            //services.AddDbContext<NewCoreDataContext>(options =>
            //{
            //    options.UseSqlServer(Configuration.GetConnectionString("TestDBConnection"));
            //});
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<NewCoreDataContext>(options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("TestDBConnection"));
                    })
                .AddTransient<ICustomerServices, CustomerServices>()
                .AddTransient<IPolicyServices, PolicyServices>();

            // Add Business Layer
            //services.AddTransient<ICustomerServices, CustomerServices>();
            //services.AddTransient<IPolicyServices, PolicyServices>();

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
            app.UseCors();  // use default
            //app.UseCors("mypolicy");  // use mypolicy

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
