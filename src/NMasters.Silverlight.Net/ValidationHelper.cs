using System;
using System.Globalization;

namespace NMasters.Silverlight.Net
{
    internal static class ValidationHelper
    {
        public static string[] EmptyArray = new string[0];
        internal static readonly char[] InvalidMethodChars = new char[] { ' ', '\r', '\n', '\t' };
        internal static readonly char[] InvalidParamChars = new char[] { 
            '(', ')', '<', '>', '@', ',', ';', ':', '\\', '"', '\'', '/', '[', ']', '?', '=', 
            '{', '}', ' ', '\t', '\r', '\n'
         };

        public static string ExceptionMessage(Exception exception)
        {
            if (exception == null)
            {
                return string.Empty;
            }
            if (exception.InnerException == null)
            {
                return exception.Message;
            }
            return (exception.Message + " (" + ExceptionMessage(exception.InnerException) + ")");
        }

        public static string HashString(object objectValue)
        {
            if (objectValue == null)
            {
                return "(null)";
            }
            if ((objectValue is string) && (((string) objectValue).Length == 0))
            {
                return "(string.empty)";
            }
            return objectValue.GetHashCode().ToString(NumberFormatInfo.InvariantInfo);
        }

        public static bool IsBlankString(string stringValue)
        {
            if (stringValue != null)
            {
                return (stringValue.Length == 0);
            }
            return true;
        }

        public static bool IsInvalidHttpString(string stringValue)
        {
            return (stringValue.IndexOfAny(InvalidParamChars) != -1);
        }

        public static string[] MakeEmptyArrayNull(string[] stringArray)
        {
            if ((stringArray != null) && (stringArray.Length != 0))
            {
                return stringArray;
            }
            return null;
        }

        public static string MakeStringNull(string stringValue)
        {
            if ((stringValue != null) && (stringValue.Length != 0))
            {
                return stringValue;
            }
            return null;
        }

        public static string ToString(object objectValue)
        {
            if (objectValue == null)
            {
                return "(null)";
            }
            if ((objectValue is string) && (((string) objectValue).Length == 0))
            {
                return "(string.empty)";
            }
            if (objectValue is Exception)
            {
                return ExceptionMessage(objectValue as Exception);
            }
            if (objectValue is IntPtr)
            {
                IntPtr ptr = (IntPtr) objectValue;
                // SL
                return ("0x" + ptr.ToString());
                //return ("0x" + ptr.ToString("x"));
            }
            return objectValue.ToString();
        }

        public static bool ValidateRange(int actual, int fromAllowed, int toAllowed)
        {
            return ((actual >= fromAllowed) && (actual <= toAllowed));
        }

        internal static void ValidateSegment(ArraySegment<byte> segment)
        {
            if (((segment.Offset < 0) || (segment.Count < 0)) || (segment.Count > (segment.Array.Length - segment.Offset)))
            {
                throw new ArgumentOutOfRangeException("segment");
            }
        }

        public static bool ValidateTcpPort(int port)
        {
            return ((port >= 0) && (port <= 0xffff));
        }
    }
}

