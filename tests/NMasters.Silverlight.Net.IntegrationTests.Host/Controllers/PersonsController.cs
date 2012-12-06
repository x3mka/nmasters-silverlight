using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using NMasters.Silverlight.Net.IntegrationTests.Host.Models;

namespace NMasters.Silverlight.Net.IntegrationTests.Host.Controllers
{
    public class PersonsController : ApiController
    {
        // GET api/persons
        public IEnumerable<Person> Get()
        {
            return new []
                       {
                           new Person() { Id = 1, FirstName = "John", LastName = "Smith"},
                           new Person() { Id = 2, FirstName = "John", LastName = "Doe"},
                           new Person() { Id = 3, FirstName = "Ameli", LastName = "Rait"},
                           new Person() { Id = 4, FirstName = "Andrew", LastName = "Lamer"},
                           new Person() { Id = 5, FirstName = "Sandy", LastName = "Miller"},
                       };
        }     
    }
}
