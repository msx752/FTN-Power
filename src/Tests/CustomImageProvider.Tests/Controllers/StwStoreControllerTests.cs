using FTNPower.Image.Api;
using FTNPower.Image.Api.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FTNPower.Image.Api.Tests.Controllers
{
    public class StwStoreControllerTests : BaseControllerTests
    {
        public StwStoreControllerTests(WebApplicationFactory<Startup> factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("/api/StwStore")]
        public async Task Post_Draw_STWStore(string url)
        {
            string strJson = File.ReadAllText("MockData\\StwStoreRequest2.json");

            // Act
            var response = await client.PostAsync($"{url}?aKey={aKey}", new StringContent(strJson, Encoding.UTF8, "application/json"));
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("image/png", response.Content.Headers.ContentType.ToString());
            var b = System.Drawing.Image.FromStream(await response.Content.ReadAsStreamAsync());
            b.Save(@"C:\Users\msali\Documents\gitRepos\FTN-Power\src\Websites\FTNPower.Image.Api\bin\Debug\netcoreapp3.1\stwStore3.png");
        }
    }
}