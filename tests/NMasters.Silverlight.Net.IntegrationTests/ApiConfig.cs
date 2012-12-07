using System;

namespace NMasters.Silverlight.Net.IntegrationTests
{
    public class ApiConfig
    {
        public static Uri ApiBaseAddress
        {
            get { return new Uri("http://localhost:1259/api/"); }
        }
    }
}