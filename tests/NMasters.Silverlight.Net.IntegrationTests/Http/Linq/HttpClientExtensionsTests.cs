using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMasters.Silverlight.Net.Http;
using NMasters.Silverlight.Net.Http.Linq;
using NMasters.Silverlight.TestsCommon.Models;

namespace NMasters.Silverlight.Net.IntegrationTests.Http.Linq
{
    [TestClass]
    public class HttpClientExtensionsTests
    {
        private static readonly Uri BaseApiUriAddress = new Uri("http://localhost:1259/api/");

        [TestMethod]
        public void SupportsODataInLinq()
        {
            var client = new HttpClient() { BaseAddress = BaseApiUriAddress };

            //var persons = client.CreateQuery<Person>().Where(p => p.FirstName == "John").OrderByDescending(p => p.Id).Skip(1).Take(1).ToList();            
        }
    }
}