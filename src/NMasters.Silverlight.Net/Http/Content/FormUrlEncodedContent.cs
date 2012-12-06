using System;
using System.Collections.Generic;
using System.Text;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Content
{
    /// <summary>A container for name/value tuples encoded using application/x-www-form-urlencoded MIME type.</summary>
    public class FormUrlEncodedContent : ByteArrayContent
    {
        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.FormUrlEncodedContent" /> class with a specific collection of name/value pairs.</summary>
        /// <param name="nameValueCollection">A collection of name/value pairs.</param>
        public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection) : base(GetContentByteArray(nameValueCollection))
        {
            base.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        private static string Encode(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }
            return Uri.EscapeDataString(data).Replace("%20", "+");
        }

        private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException("nameValueCollection");
            }
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in nameValueCollection)
            {
                if (builder.Length > 0)
                {
                    builder.Append('&');
                }
                builder.Append(Encode(pair.Key));
                builder.Append('=');
                builder.Append(Encode(pair.Value));
            }
            return HttpRuleParser.DefaultHttpEncoding.GetBytes(builder.ToString());
        }
    }
}

