using System.Web;
using System.Web.Mvc;

namespace NMasters.Silverlight.Net.IntegrationTests.Host
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}