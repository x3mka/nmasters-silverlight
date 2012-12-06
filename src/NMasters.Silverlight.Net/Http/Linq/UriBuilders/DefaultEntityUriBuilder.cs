using System;
using System.Linq;
using NMasters.Silverlight.Net.Http.Linq.Collections;

namespace NMasters.Silverlight.Net.Http.Linq.UriBuilders
{
    public class DefaultEntityUriBuilder : IEntityUriBuilder
    {
        public DefaultEntityUriBuilder()
        {
            IsPluralized = true;
        }

        public bool IsPluralized { get; set; }

        private readonly ConcurrentDictionary<Type, EntityUriAttribute> _entityUriAttributesCache
            = new ConcurrentDictionary<Type, EntityUriAttribute>();

        public Uri BuildUri(Type type)
        {
            var attributeUri = GetAttributeUri(type);
            if (attributeUri != null)
                return new Uri(GetAttributeUri(type), UriKind.Relative);

            string uri = type.Name;
            if (IsPluralized)
            {
                // todo: use some good pluralizer
                //if (uri.EndsWith("y"))
                //    uri = uri.Substring(0, uri.Length - 1) + "ies";
                //else 
                uri += "s";
            }
                
            return new Uri(uri, UriKind.Relative);

        }

        private string GetAttributeUri(Type type)
        {
            EntityUriAttribute attribute = null;
            if (!_entityUriAttributesCache.TryGetValue(type, out attribute))
            {
                attribute = type.GetCustomAttributes(true).OfType<EntityUriAttribute>().FirstOrDefault();
                _entityUriAttributesCache.TryAddValue(type, attribute);
            }
            return attribute == null ? null : attribute.RelativeUri;
        }

    }
}