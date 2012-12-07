using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using NMasters.Silverlight.Net.IntegrationTests.Host.Models;

namespace NMasters.Silverlight.Net.IntegrationTests.Host.Controllers
{
    public class PersonsController : ApiController
    {
        private static readonly List<Person> Persons = new List<Person>()
                                                   {
                           new Person() { Id = 1, FirstName = "John", LastName = "Smith"},
                           new Person() { Id = 2, FirstName = "John", LastName = "Doe"},
                           new Person() { Id = 3, FirstName = "Ameli", LastName = "Rait"},
                           new Person() { Id = 4, FirstName = "Andrew", LastName = "Lamer"},
                           new Person() { Id = 5, FirstName = "Sandy", LastName = "Miller"},
                                                   }; 

        // GET api/persons
        [Queryable]
        public IQueryable<Person> Get()
        {
            return Persons.AsQueryable();
        }

        // GET api/persons/{id}
        public Person GetById(int id)
        {
            return Persons.Find(p => p.Id == id);
        } 
    }
}
