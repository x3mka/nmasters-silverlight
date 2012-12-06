using System.Net;

namespace NMasters.Silverlight.Net
{
    internal static class HttpStatusDescription
    {
        private static readonly string[][] httpStatusDescriptions;

        static HttpStatusDescription()
        {
            string[][] strArray = new string[6][];
            strArray[1] = new string[] { "Continue", "Switching Protocols", "Processing" };
            strArray[2] = new string[] { "OK", "Created", "Accepted", "Non-Authoritative Information", "No Content", "Reset Content", "Partial Content", "Multi-Status" };
            string[] strArray4 = new string[8];
            strArray4[0] = "Multiple Choices";
            strArray4[1] = "Moved Permanently";
            strArray4[2] = "Found";
            strArray4[3] = "See Other";
            strArray4[4] = "Not Modified";
            strArray4[5] = "Use Proxy";
            strArray4[7] = "Temporary Redirect";
            strArray[3] = strArray4;
            string[] strArray5 = new string[0x1a];
            strArray5[0] = "Bad Request";
            strArray5[1] = "Unauthorized";
            strArray5[2] = "Payment Required";
            strArray5[3] = "Forbidden";
            strArray5[4] = "Not Found";
            strArray5[5] = "Method Not Allowed";
            strArray5[6] = "Not Acceptable";
            strArray5[7] = "Proxy Authentication Required";
            strArray5[8] = "Request Timeout";
            strArray5[9] = "Conflict";
            strArray5[10] = "Gone";
            strArray5[11] = "Length Required";
            strArray5[12] = "Precondition Failed";
            strArray5[13] = "Request Entity Too Large";
            strArray5[14] = "Request-Uri Too Long";
            strArray5[15] = "Unsupported Media Type";
            strArray5[0x10] = "Requested Range Not Satisfiable";
            strArray5[0x11] = "Expectation Failed";
            strArray5[0x16] = "Unprocessable Entity";
            strArray5[0x17] = "Locked";
            strArray5[0x18] = "Failed Dependency";
            strArray[4] = strArray5;
            string[] strArray6 = new string[8];
            strArray6[0] = "Internal Server Error";
            strArray6[1] = "Not Implemented";
            strArray6[2] = "Bad Gateway";
            strArray6[3] = "Service Unavailable";
            strArray6[4] = "Gateway Timeout";
            strArray6[5] = "Http Version Not Supported";
            strArray6[7] = "Insufficient Storage";
            strArray[5] = strArray6;
            httpStatusDescriptions = strArray;
        }

        internal static string Get(int code)
        {
            if ((code >= 100) && (code < 600))
            {
                int index = code / 100;
                int num2 = code % 100;
                if (num2 < httpStatusDescriptions[index].Length)
                {
                    return httpStatusDescriptions[index][num2];
                }
            }
            return null;
        }

        internal static string Get(HttpStatusCode code)
        {
            return Get((int) code);
        }
    }
}

