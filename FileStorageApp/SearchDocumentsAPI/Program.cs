using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace SearchDocumentsAPI
{
#pragma warning disable CS1591

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}

#pragma warning restore CS1591