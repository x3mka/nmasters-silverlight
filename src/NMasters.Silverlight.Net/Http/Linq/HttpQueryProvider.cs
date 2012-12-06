using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

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

            //if (_context.Configuration.RequestSetup != null)
            //    _context.Configuration.RequestSetup(_context.Request);

            var response = _context.HttpClient.SendAsync(_context.Request).Result;

            //Task<object> task = null;
            //var formatters = _context.Configuration.CustomFormatters;
            //if (formatters == null || formatters.Count() == 0)
            //    formatters = new MediaTypeFormatterCollection();

            //if (response.StatusCode != HttpStatusCode.OK)
            //{
            //    throw new PocoHttpResponseException(response);
            //}

            //task = _context.Configuration.ResponseReader(response, formatters, _context.EntityType);

            //return task.Result;
		    return response;
		}
    }
}