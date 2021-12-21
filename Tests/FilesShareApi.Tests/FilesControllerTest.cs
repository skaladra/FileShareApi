using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using FilesShareApi;

namespace FileShareApi.Tests
{
    public class FilesControllerTest
    {
        private readonly HttpClient client;

        public FilesControllerTest()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>());
            client = server.CreateClient();
        }

        [Fact]
        public async Task CreateTestUser()
        {
            var testUser = new UserEntity
            {
                Name = "testUser",
                Email = "testUser@nowhere.com",
                Password = "String123"
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(testUser), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/users/register", requestContent);

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Nothing()
        {

        }
    }
}

