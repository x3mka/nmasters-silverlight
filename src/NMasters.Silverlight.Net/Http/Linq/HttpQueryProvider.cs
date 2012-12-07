using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NMasters.Silverlight.Net.Http.Formatting;

namespace NMasters.Silverlight.Net.Http.Linq
{
    public class HttpQueryProvider : HttpQueryProviderBase
    {
        private readonly HttpQueryContext _context;

        public HttpQueryProvider(HttpQueryContext context)
		{
			_context = context;
		}

        //public override string GetQueryText(Expression expression)
        //{
        //    //return _context.Configuration.Grammar.GetQueryText(expression);
        //    return null;
        //}

		public override object Execute(Expression expression)
		{
            _context.ExpressionBindingStrategy.Bind(expression, _context.Request);
            Debug.WriteLine(_context.Request.RequestUri.ToString());

            var result = _context.HttpClient.SendAsync(_context.Request).Result;

            var collectionTyle = typeof(IEnumerable<>).MakeGenericType(_context.EntityType);

            var entities = result.Content.ReadAsAsync(collectionTyle).Result;
        
            return entities;
		}
    }
}