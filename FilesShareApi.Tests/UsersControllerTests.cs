using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System.Net;
using FilesShareApi;
using System.Text.Json;
using FilesShareApi.Tests;

namespace FileShareApi.Tests
{
    public class UsersControllerTests
    {
        private readonly HttpClient client;

        public UsersControllerTests()
        {
            client = ServerCreator.GetHTTPClient();
        }

        [Fact]
        public async Task CreateTestUser()
        {
            var testUser = new UserCreateDto
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
        public async Task TestLogout()
        {
            var logoutResponse = await client.GetAsync("/users/logout");

            logoutResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
        }

        [Fact]
        public async Task TestLogin()
        {
            var LoginUser = new UserLoginDto() 
            { 
                Email = "user1@example.com", 
                Password = "String123" 
            };

            var requestContent = new StringContent(JsonConvert.SerializeObject(LoginUser), Encoding.UTF8, "application/json");
            var logoutResponse = await client.PostAsync("/users/login", requestContent);

            logoutResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteTestUser()
        {
            var getUserResponse = await client.GetAsync("/users/1?name=testUser");
            getUserResponse.EnsureSuccessStatusCode();
            var content = await getUserResponse.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            var id = json.RootElement.GetProperty("id");      

            var deleteResponse = await client.DeleteAsync($"/users/1?id={ id }");

            deleteResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }
    }
}
