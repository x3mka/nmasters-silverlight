using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMasters.Silverlight.Net.Http;
using NMasters.Silverlight.Net.Http.Formatting;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.TestsCommon.Models;

namespace NMasters.Silverlight.Net.IntegrationTests.Http.Formatting
{
    [TestClass]
    public class HttpClientFormattingExtensionsTests
    {
        [TestMethod]
        public void CanGetJsonContent()
        {
            var client = new HttpClient() { BaseAddress = ApiConfig.ApiBaseAddress };
            client.DefaultRequestHeaders.Add("Accept", AcceptType.Json);

            var response = client.GetAsync("persons/1").Result;

            var person = response.Content.ReadAsAsync<Person>().Result;

            Assert.IsTrue(person.Id == 1);
            Assert.IsTrue(person.FirstName == "John");
            Assert.IsTrue(person.LastName == "Smith");            
        }

        [TestMethod]
        public void CanGetXmlContent()
        {
            var client = new HttpClient() { BaseAddress = ApiConfig.ApiBaseAddress };
            client.DefaultRequestHeaders.Add("Accept", AcceptType.Xml);

            var response = client.GetAsync("persons/1").Result;

            var person = response.Content.ReadAsAsync<Person>().Result;

            Assert.IsTrue(person.Id == 1);
            Assert.IsTrue(person.FirstName == "John");
            Assert.IsTrue(person.LastName == "Smith");
        }

        [TestMethod]
        public void CanGetJsonEnumerable()
        {
            var client = new HttpClient() { BaseAddress = ApiConfig.ApiBaseAddress };
            client.DefaultRequestHeaders.Add("Accept", AcceptType.Json);

            var response = client.GetAsync("persons").Result;

            var persons = response.Content.ReadAsAsync<IEnumerable<Person>>().Result.ToList();

            Assert.IsTrue(persons.Count == 5);            
        }

    }
}