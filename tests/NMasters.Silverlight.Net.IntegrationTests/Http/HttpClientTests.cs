using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMasters.Silverlight.Net.Http;
using NMasters.Silverlight.Net.Http.Linq;
using NMasters.Silverlight.TestsCommon.Models;

namespace NMasters.Silverlight.Net.IntegrationTests.Http
{
    [TestClass]
    public class HttpClientTests
    {        
        [TestMethod]
        public void CanGetStringContent()
        {
            var client = new HttpClient() { BaseAddress = ApiConfig.ApiBaseAddress };
            var response = client.GetAsync("persons").Result;            

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);

            var content = response.Content.ReadAsStringAsync().Result;

            Assert.IsNotNull(content);
            Assert.IsTrue(content.Length > 0);
        }
    }
}