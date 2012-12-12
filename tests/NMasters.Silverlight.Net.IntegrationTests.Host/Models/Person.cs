using System.Collections.Concurrent;

namespace NMasters.Silverlight.Net.IntegrationTests.Host.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        private void Test()
        {
            var dic = new ConcurrentDictionary<int, int>();            
        }
    }
}