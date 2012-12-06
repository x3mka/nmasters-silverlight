namespace NMasters.Silverlight.Net.Http.Configuration
{
    /// <summary>Indicates if <see cref="T:NMasters.Silverlight.Net.Http.HttpClient" /> operations should be considered completed either as soon as a response is available, or after reading the entire response message including the content. </summary>
    public enum HttpCompletionOption
    {
        ResponseContentRead,
        ResponseHeadersRead
    }
}

