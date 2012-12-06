using System;
using System.Linq;
using NMasters.Silverlight.Net.Http.Linq.UriBuilders;

namespace NMasters.Silverlight.Net.Http.Linq
{
    public static class HttpClientExtensions
    {
        public static IQueryable<T> CreateQuery<T>(this HttpClient client)
        {
            return client.CreateQuery<T>(new DefaultEntityUriBuilder());                
        }

        public static IQueryable<T> CreateQuery<T>(this HttpClient client, IEntityUriBuilder uriBuilder)
        {
            var uri = uriBuilder.BuildUri(typeof(T));
            return client.CreateQuery<T>(uri);    
        }

        public static IQueryable<T> CreateQuery<T>(this HttpClient client, string uri)
        {
            // todo: make checks for relative/absolute uris
            return client.CreateQuery<T>(new Uri(uri));    
        }

        public static IQueryable<T> CreateQuery<T>(this HttpClient client, Uri uri)
        {
            return (IQueryable<T>)client.CreateQuery(typeof(T), uri);
        }

        public static IQueryable CreateQuery(this HttpClient client, Type entityType, Uri uri)
        {
            //todo: prepare request acc. to httpclient settings (expose via HttpClient itselt?)   
            uri = new Uri(client.BaseAddress + uri.ToString());

            var request = new HttpRequestMessage(HttpMethod.Get, uri);            

            var httpProvider = new HttpQueryProvider(
                new HttpQueryContext()
                {
                    EntityType = entityType,
                    HttpClient = client,                    
                    Request = request
                }
            );

            return (IQueryable)Activator.CreateInstance(typeof(HttpQuery<>).MakeGenericType(entityType),
                new object[] { httpProvider });
        }

        
    }
}