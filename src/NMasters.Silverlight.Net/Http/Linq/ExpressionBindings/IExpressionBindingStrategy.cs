using System.Linq.Expressions;

namespace NMasters.Silverlight.Net.Http.Linq.ExpressionBindings
{
    public interface IExpressionBindingStrategy
    {
        void Bind(Expression expression, HttpRequestMessage message);        
    }
}