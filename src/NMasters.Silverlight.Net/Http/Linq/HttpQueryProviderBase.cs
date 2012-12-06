using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NMasters.Silverlight.Net.Http.Linq
{
    public abstract class HttpQueryProviderBase : IQueryProvider
    {        
        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new HttpQuery<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(HttpQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        T IQueryProvider.Execute<T>(Expression expression)
        {
            return (T)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return Execute(expression);
        }

        //public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);
    }

}