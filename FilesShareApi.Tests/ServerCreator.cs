using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;

namespace FilesShareApi.Tests
{
    public static class ServerCreator
    {
        private readonly static TestServer testServer = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
        private readonly static HttpClient httpClient = testServer.CreateClient();
        public static HttpClient GetHTTPClient()
        {
            return httpClient;
        }


    }
}
