using System;

namespace NMasters.Silverlight.Net.Http.Linq.UriBuilders
{
    public interface IEntityUriBuilder
    {
        Uri BuildUri(Type type);        
    }
}