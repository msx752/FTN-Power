using FTNPower.Image.Api;
using Global;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xunit;

namespace FTNPower.Image.Api.Tests
{
    public class BaseControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        internal readonly string aKey = "";
        internal readonly HttpClient client = null;
        internal readonly WebApplicationFactory<Startup> _factory;
        public BaseControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            client = _factory.CreateClient();
            aKey = _factory.Services.ImageServiceConfigs().AKey;
        }

    }
}
