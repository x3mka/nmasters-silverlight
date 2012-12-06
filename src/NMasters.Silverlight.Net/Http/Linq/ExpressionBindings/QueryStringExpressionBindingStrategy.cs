using System;
using System.Linq.Expressions;

namespace NMasters.Silverlight.Net.Http.Linq.ExpressionBindings
{
    public abstract class QueryStringExpressionBindingStrategy : IExpressionBindingStrategy
    {
        public void Bind(Expression expression, HttpRequestMessage request)
        {
            var queryString = GetQueryString(expression);            

            request.RequestUri = new Uri(request.RequestUri +
                ((request.RequestUri.ToString().IndexOf("?", StringComparison.InvariantCulture) >= 0) ? "&" : "?")
                + queryString, request.RequestUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
        }

        protected abstract string GetQueryString(Expression expression);
    }
}