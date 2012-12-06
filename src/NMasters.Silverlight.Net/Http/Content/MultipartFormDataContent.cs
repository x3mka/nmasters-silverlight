using System;
using NMasters.Silverlight.Net.Http.Headers;
using NMasters.Silverlight.Net.Http.Helpers;
using NMasters.Silverlight.Net.Properties;

namespace NMasters.Silverlight.Net.Http.Content
{
    /// <summary>Provides a container for content encoded using multipart/form-data MIME type.</summary>
    public class MultipartFormDataContent : MultipartContent
    {
        private const string formData = "form-data";

        /// <summary>Creates a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Content.MultipartFormDataContent" /> class.</summary>
        public MultipartFormDataContent() : base("form-data")
        {
        }

        /// <summary>Creates a new instance of the <see cref="T:System.Net.Http.MultipartFormDataContent" /> class.</summary>
        /// <param name="boundary">The boundary string for the multipart form data content.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="boundary" /> was null or contains only white space characters.-or-The <paramref name="boundary" /> ends with a space character.</exception>
        /// <exception cref="T:System.OutOfRangeException">The length of the <paramref name="boundary" /> was greater than 70.</exception>
        public MultipartFormDataContent(string boundary) : base("form-data", boundary)
        {
        }

        /// <summary>Add HTTP content to a collection of <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" /> objects that get serialized to multipart/form-data MIME type.</summary>
        /// <param name="content">The HTTP content to add to the collection.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was null.</exception>
        public override void Add(HttpContent content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (content.Headers.ContentDisposition == null)
            {
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            }
            base.Add(content);
        }

        /// <summary>Add HTTP content to a collection of <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" /> objects that get serialized to multipart/form-data MIME type.</summary>
        /// <param name="content">The HTTP content to add to the collection.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="name" /> was null or contains only white space characters.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was null.</exception>
        public void Add(HttpContent content, string name)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "name");
            }
            this.AddInternal(content, name, null);
        }

        /// <summary>Add HTTP content to a collection of <see cref="T:NMasters.Silverlight.Net.Http.HttpContent" /> objects that get serialized to multipart/form-data MIME type.</summary>
        /// <param name="content">The HTTP content to add to the collection.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="fileName">The file name for the HTTP content to add to the collection.</param>
        /// <exception cref="T:System.ArgumentException">The <paramref name="name" /> was null or contains only white space characters.-or-The <paramref name="fileName" /> was null or contains only white space characters.</exception>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="content" /> was null.</exception>
        public void Add(HttpContent content, string name, string fileName)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "name");
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException(SR.net_http_argument_empty_string, "fileName");
            }
            this.AddInternal(content, name, fileName);
        }

        private void AddInternal(HttpContent content, string name, string fileName)
        {
            if (content.Headers.ContentDisposition == null)
            {
                // SL fixes
                //ContentDispositionHeaderValue value2 = new ContentDispositionHeaderValue("form-data") {
                //    Name = name,
                //    FileName = fileName,
                //    FileNameStar = fileName
                //};
                //content.Headers.ContentDisposition = value2;
            }
            base.Add(content);
        }
    }
}

