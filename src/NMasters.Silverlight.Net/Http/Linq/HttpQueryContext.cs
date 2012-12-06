using System;
using NMasters.Silverlight.Net.Http.Linq.ExpressionBindings;
using NMasters.Silverlight.Net.Http.Linq.OData;

namespace NMasters.Silverlight.Net.Http.Linq
{
    public class HttpQueryContext
    {
        public HttpQueryContext()
        {
            ExpressionBindingStrategy = new ODataExpressionBindingStrategy();
        }

        public Type EntityType { get; set; }
        public HttpRequestMessage Request { get; set; }
        public HttpClient HttpClient { get; set; }
        public IExpressionBindingStrategy ExpressionBindingStrategy { get; set; }
    }
}