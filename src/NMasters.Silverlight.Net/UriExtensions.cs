using System;

namespace NMasters.Silverlight.Net
{
    public static class UriExtensions
    {
        public static bool IsHexEncoding(this Uri uri, string pattern, int index)
        {
            if ((pattern.Length - index) < 3)
            {
                return false;
            }
            // SL fixes
            return ((pattern[index] == '%'));// && (UriHelper.EscapedAscii(pattern[index + 1], pattern[index + 2]) != 0xffff));
        }


    }
}