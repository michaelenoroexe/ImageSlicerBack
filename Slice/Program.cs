using Microsoft.AspNetCore;
using Slice;
using System.Web;

namespace API
{
    class Slice
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            
            // Using startup to configure application parameters
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
        }
    }
}
