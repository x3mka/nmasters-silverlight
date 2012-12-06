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
        private static readonly Uri BaseApiUriAddress = new Uri("http://localhost:1259/api/");

        [TestMethod]
        public void CanGetStringContent()
        {
            var client = new HttpClient() { BaseAddress = BaseApiUriAddress };
            var response = client.GetAsync("persons").Result;

            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);            
        }
    }
}