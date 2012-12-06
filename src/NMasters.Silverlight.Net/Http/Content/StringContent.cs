using System;
using System.Text;
using NMasters.Silverlight.Net.Http.Headers;

namespace NMasters.Silverlight.Net.Http.Content
{
    /// <summary>Provides HTTP content based on a string.</summary>
    public class StringContent : ByteArrayContent
    {
        private const string defaultMediaType = "text/plain";

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.StringContent" /> class.</summary>
        /// <param name="content">The content used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.StringContent" />.</param>
        public StringContent(string content) : this(content, null, null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.StringContent" /> class.</summary>
        /// <param name="content">The content used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.StringContent" />.</param>
        /// <param name="encoding">The encoding to use for the content.</param>
        public StringContent(string content, Encoding encoding) : this(content, encoding, null)
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.StringContent" /> class.</summary>
        /// <param name="content">The content used to initialize the <see cref="T:NMasters.Silverlight.Net.Http.Content.StringContent" />.</param>
        /// <param name="encoding">The encoding to use for the content.</param>
        /// <param name="mediaType">The media type to use for the content.</param>
        public StringContent(string content, Encoding encoding, string mediaType) : base(GetContentByteArray(content, encoding))
        {
            MediaTypeHeaderValue value2 = new MediaTypeHeaderValue((mediaType == null) ? "text/plain" : mediaType) {
                CharSet = (encoding == null) ? HttpContent.DefaultStringEncoding.WebName : encoding.WebName
            };
            base.Headers.ContentType = value2;
        }

        private static byte[] GetContentByteArray(string content, Encoding encoding)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (encoding == null)
            {
                encoding = HttpContent.DefaultStringEncoding;
            }
            return encoding.GetBytes(content);
        }
    }
}

