// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NMasters.Silverlight.Net.Http.Formatting
{
    // Default Contract resolver for JsonMediaTypeFormatter
    // Uses the IRequiredMemberSelector to choose required members
    internal class JsonContractResolver : DefaultContractResolver
    {
        private readonly MediaTypeFormatter _formatter;

        public JsonContractResolver(MediaTypeFormatter formatter)
        {
            _formatter = formatter;            
        }

        // Determines whether a member is required or not and sets the appropriate JsonProperty settings
        private void ConfigureProperty(MemberInfo member, JsonProperty property)
        {
            if (_formatter.RequiredMemberSelector != null && _formatter.RequiredMemberSelector.IsRequiredMember(member))
            {
                property.Required = Required.AllowNull;
                property.DefaultValueHandling = DefaultValueHandling.Include;
                property.NullValueHandling = NullValueHandling.Include;
            }
            else
            {
                property.Required = Required.Default;
            }
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            ConfigureProperty(member, property);
            return property;
        }
    }
}
