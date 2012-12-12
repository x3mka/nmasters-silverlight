using System;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMasters.Silverlight.Net.Http;
using NMasters.Silverlight.Net.Http.Exceptions;
using NMasters.Silverlight.Net.Http.Handlers;
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

        [TestMethod]
        public void CancelationTest()
        {
            
            TimeoutManager.TimeoutInMilliseconds = 0;

            var client = new HttpClient() { BaseAddress = BaseApiUriAddress };
            var response = client.GetAsync("persons");

            Thread.Sleep(2000);

            try
            {
                Assert.IsNotNull(response.Result);
            }
            catch (AggregateException exception)
            {
                Assert.AreEqual(exception.InnerExceptions.Count, 1);
                var requestException = exception.InnerException.InnerException as WebException;
                Assert.IsNotNull(requestException);
                Assert.AreEqual(requestException.Status, WebExceptionStatus.RequestCanceled);
            }

            TimeoutManager.TimeoutInMilliseconds = 100 * 1000;

        }
    }
}