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
        [TestMethod]
        public void SupportsODataInLinq()
        {
            var client = new HttpClient() { BaseAddress = ApiConfig.ApiBaseAddress };

            var persons = client.CreateQuery<Person>().Where(p => p.FirstName == "John").OrderBy(p => p.Id).Skip(1).Take(1).ToList();            

            Assert.IsNotNull(persons);
            Assert.IsTrue(persons.Count == 1);
            Assert.IsTrue(persons[0].Id == 2);
        }
    }
}