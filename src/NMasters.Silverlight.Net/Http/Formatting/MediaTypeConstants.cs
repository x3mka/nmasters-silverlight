// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using NMasters.Silverlight.Net.Http.Headers;

namespace NMasters.Silverlight.Net.Http.Formatting
{
    /// <summary>
    /// Constants related to media types.
    /// </summary>
    internal static class MediaTypeConstants
    {
        private static readonly MediaTypeHeaderValue DefaultApplicationXmlMediaType = new MediaTypeHeaderValue("application/xml");
        private static readonly MediaTypeHeaderValue DefaultTextXmlMediaType = new MediaTypeHeaderValue("text/xml");
        private static readonly MediaTypeHeaderValue DefaultApplicationJsonMediaType = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeHeaderValue DefaultTextJsonMediaType = new MediaTypeHeaderValue("text/json");
        private static readonly MediaTypeHeaderValue DefaultApplicationOctetStreamMediaType = new MediaTypeHeaderValue("application/octet-stream");
        private static readonly MediaTypeHeaderValue DefaultApplicationFormUrlEncodedMediaType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/octet-stream</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/octet-stream</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationOctetStreamMediaType
        {
            get { return (MediaTypeHeaderValue)DefaultApplicationOctetStreamMediaType.Clone(); }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/xml</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/xml</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationXmlMediaType
        {
            get { return (MediaTypeHeaderValue)DefaultApplicationXmlMediaType.Clone(); }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/json</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/json</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationJsonMediaType
        {
            get { return (MediaTypeHeaderValue)DefaultApplicationJsonMediaType.Clone(); }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>text/xml</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>text/xml</c>.
        /// </value>
        public static MediaTypeHeaderValue TextXmlMediaType
        {
            get { return (MediaTypeHeaderValue)DefaultTextXmlMediaType.Clone(); }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>text/json</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>text/json</c>.
        /// </value>
        public static MediaTypeHeaderValue TextJsonMediaType
        {
            get { return (MediaTypeHeaderValue)DefaultTextJsonMediaType.Clone(); }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/x-www-form-urlencoded</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/x-www-form-urlencoded</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationFormUrlEncodedMediaType
        {
            get { return (MediaTypeHeaderValue)DefaultApplicationFormUrlEncodedMediaType.Clone(); }
        }
    }
}
