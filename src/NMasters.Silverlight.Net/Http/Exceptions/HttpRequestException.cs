using System;

namespace NMasters.Silverlight.Net.Http.Exceptions
{
    /// <summary>A base class for exceptions thrown by the <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> and <see cref="T:NMasters.Silverlight.Net.Http.HttpMessageHandler" /> classes.</summary>
    //[Serializable]
    public class HttpRequestException : Exception
    {
        //private static readonly EventHandler<SafeSerializationEventArgs> handleSerialization = new EventHandler<SafeSerializationEventArgs>(HttpRequestException.HandleSerialization);

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Exceptions.HttpRequestException" /> class.</summary>
        public HttpRequestException() : this(null, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Exceptions.HttpRequestException" /> class with a specific message that describes the current exception.</summary>
        /// <param name="message">A message that describes the current exception.</param>
        public HttpRequestException(string message) : this(message, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Exceptions.HttpRequestException" /> class with a specific message that describes the current exception and an inner exception.</summary>
        /// <param name="message">A message that describes the current exception.</param>
        /// <param name="inner">The inner exception.</param>
        public HttpRequestException(string message, Exception inner)
            : base(message, inner)
        {
            //base.add_SerializeObjectState(handleSerialization);
        }

        //private static void HandleSerialization(object exception, SafeSerializationEventArgs eventArgs)
        //{
        //    eventArgs.AddSerializedState(new EmptyState());
        //}

        //[Serializable]
        //private class EmptyState : ISafeSerializationData
        //{
        //    public void CompleteDeserialization(object deserialized)
        //    {
        //        ((HttpRequestException) deserialized).add_SerializeObjectState(HttpRequestException.handleSerialization);
        //    }
        //}
    }
}

