using System.Linq.Expressions;
using NMasters.Silverlight.Net.Http.Linq.ExpressionBindings;

namespace NMasters.Silverlight.Net.Http.Linq.OData
{
    public class ODataExpressionBindingStrategy : QueryStringExpressionBindingStrategy
    {
        protected override string GetQueryString(Expression expression)
        {
            var odataVisitor = new ODataVisitor();
            return odataVisitor.Translate(expression);
        }
    }
}