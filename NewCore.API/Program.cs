using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace NewCore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("http://localhost:5000");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseDefaultServiceProvider(options =>
                        options.ValidateScopes = false);
                });
    }
}
