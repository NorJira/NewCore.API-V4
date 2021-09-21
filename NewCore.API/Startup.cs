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
               options.SuppressAsyncSuffixInActionNames = false
            ).AddJsonOptions(options =>
               options.JsonSerializerOptions.WriteIndented = true
            );

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

                // Add Data Layer
                services.AddDbContext<NewCoreDataContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("TestDBConnection"));
                });

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
