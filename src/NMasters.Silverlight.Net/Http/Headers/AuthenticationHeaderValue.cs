using System;
using NMasters.Silverlight.Net.Http.Internal;

namespace NMasters.Silverlight.Net.Http.Headers
{
    /// <summary>Represents authentication information in Authorization, ProxyAuthorization, WWW-Authenticate, and Proxy-Authenticate header values.</summary>
    public class AuthenticationHeaderValue //: ICloneable
    {
        private string parameter;
        private string scheme;

        private AuthenticationHeaderValue()
        {
        }

        private AuthenticationHeaderValue(AuthenticationHeaderValue source)
        {
            this.scheme = source.scheme;
            this.parameter = source.parameter;
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> class.</summary>
        /// <param name="scheme">The scheme to use for authorization.</param>
        public AuthenticationHeaderValue(string scheme) : this(scheme, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> class.</summary>
        /// <param name="scheme">The scheme to use for authorization.</param>
        /// <param name="parameter">The credentials containing the authentication information of the user agent for the resource being requested.</param>
        public AuthenticationHeaderValue(string scheme, string parameter)
        {
            HeaderUtilities.CheckValidToken(scheme, "scheme");
            this.scheme = scheme;
            this.parameter = parameter;
        }

        /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if the specified <see cref="T:System.Object" /> is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            AuthenticationHeaderValue value2 = obj as AuthenticationHeaderValue;
            if (value2 == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.parameter) && string.IsNullOrEmpty(value2.parameter))
            {
                return (string.Compare(this.scheme, value2.scheme, StringComparison.OrdinalIgnoreCase) == 0);
            }
            return ((string.Compare(this.scheme, value2.scheme, StringComparison.OrdinalIgnoreCase) == 0) && (string.CompareOrdinal(this.parameter, value2.parameter) == 0));
        }

        internal static int GetAuthenticationLength(string input, int startIndex, out object parsedValue)
        {
            parsedValue = null;
            if (string.IsNullOrEmpty(input) || (startIndex >= input.Length))
            {
                return 0;
            }
            int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
            if (tokenLength == 0)
            {
                return 0;
            }
            AuthenticationHeaderValue value2 = new AuthenticationHeaderValue {
                scheme = input.Substring(startIndex, tokenLength)
            };
            int num2 = startIndex + tokenLength;
            int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, num2);
            num2 += whitespaceLength;
            if ((num2 == input.Length) || (input[num2] == ','))
            {
                parsedValue = value2;
                return (num2 - startIndex);
            }
            if (whitespaceLength == 0)
            {
                return 0;
            }
            int num4 = num2;
            int parameterEndIndex = num2;
            if (!TrySkipFirstBlob(input, ref num2, ref parameterEndIndex))
            {
                return 0;
            }
            if ((num2 < input.Length) && !TryGetParametersEndIndex(input, ref num2, ref parameterEndIndex))
            {
                return 0;
            }
            value2.parameter = input.Substring(num4, (parameterEndIndex - num4) + 1);
            parsedValue = value2;
            return (num2 - startIndex);
        }

        /// <summary>Serves as a hash function for an  <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.Int32" />.A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int hashCode = this.scheme.ToLowerInvariant().GetHashCode();
            if (!string.IsNullOrEmpty(this.parameter))
            {
                hashCode ^= this.parameter.GetHashCode();
            }
            return hashCode;
        }

        /// <summary>Converts a string to an <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" />.An <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</returns>
        /// <param name="input">A string that represents authentication header value information.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="input" /> is a null reference.</exception>
        /// <exception cref="T:System.FormatException">
        /// <paramref name="input" /> is not valid authentication header value information.</exception>
        public static AuthenticationHeaderValue Parse(string input)
        {
            int index = 0;
            return (AuthenticationHeaderValue) GenericHeaderParser.SingleValueAuthenticationParser.ParseValue(input, null, ref index);
        }

        /// <summary>Creates a new object that is a copy of the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> instance.</summary>
        /// <returns>Returns <see cref="T:System.Object" />.A copy of the current instance.</returns>
        //object ICloneable.Clone()
        //{
        //    return new AuthenticationHeaderValue(this);
        //}

        /// <summary>Returns a string that represents the current <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> object.</summary>
        /// <returns>Returns <see cref="T:System.String" />.A string that represents the current object.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.parameter))
            {
                return this.scheme;
            }
            return (this.scheme + " " + this.parameter);
        }

        private static bool TryGetParametersEndIndex(string input, ref int parseEndIndex, ref int parameterEndIndex)
        {
            int startIndex = parseEndIndex;
            do
            {
                startIndex++;
                bool separatorFound = false;
                startIndex = HeaderUtilities.GetNextNonEmptyOrWhitespaceIndex(input, startIndex, true, out separatorFound);
                if (startIndex == input.Length)
                {
                    return true;
                }
                int tokenLength = HttpRuleParser.GetTokenLength(input, startIndex);
                if (tokenLength == 0)
                {
                    return false;
                }
                startIndex += tokenLength;
                startIndex += HttpRuleParser.GetWhitespaceLength(input, startIndex);
                if ((startIndex == input.Length) || (input[startIndex] != '='))
                {
                    return true;
                }
                startIndex++;
                startIndex += HttpRuleParser.GetWhitespaceLength(input, startIndex);
                int valueLength = NameValueHeaderValue.GetValueLength(input, startIndex);
                if (valueLength == 0)
                {
                    return false;
                }
                startIndex += valueLength;
                parameterEndIndex = startIndex - 1;
                startIndex += HttpRuleParser.GetWhitespaceLength(input, startIndex);
                parseEndIndex = startIndex;
            }
            while ((startIndex < input.Length) && (input[startIndex] == ','));
            return true;
        }

        /// <summary>Determines whether a string is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> information.</summary>
        /// <returns>Returns <see cref="T:System.Boolean" />.true if <paramref name="input" /> is valid <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> information; otherwise, false.</returns>
        /// <param name="input">The string to validate.</param>
        /// <param name="parsedValue">The <see cref="T:NMasters.Silverlight.Net.Http.Headers.AuthenticationHeaderValue" /> version of the string.</param>
        public static bool TryParse(string input, out AuthenticationHeaderValue parsedValue)
        {
            object obj2;
            int index = 0;
            parsedValue = null;
            if (GenericHeaderParser.SingleValueAuthenticationParser.TryParseValue(input, null, ref index, out obj2))
            {
                parsedValue = (AuthenticationHeaderValue) obj2;
                return true;
            }
            return false;
        }

        private static bool TrySkipFirstBlob(string input, ref int current, ref int parameterEndIndex)
        {
            while ((current < input.Length) && (input[current] != ','))
            {
                if (input[current] == '"')
                {
                    int length = 0;
                    if (HttpRuleParser.GetQuotedStringLength(input, current, out length) != HttpParseResult.Parsed)
                    {
                        return false;
                    }
                    current += length;
                    parameterEndIndex = current - 1;
                }
                else
                {
                    int whitespaceLength = HttpRuleParser.GetWhitespaceLength(input, current);
                    if (whitespaceLength == 0)
                    {
                        parameterEndIndex = current;
                        current++;
                    }
                    else
                    {
                        current += whitespaceLength;
                    }
                }
            }
            return true;
        }

        /// <summary>Gets the credentials containing the authentication information of the user agent for the resource being requested.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The credentials containing the authentication information.</returns>
        public string Parameter
        {
            get
            {
                return this.parameter;
            }
        }

        /// <summary>Gets the scheme to use for authorization.</summary>
        /// <returns>Returns <see cref="T:System.String" />.The scheme to use for authorization.</returns>
        public string Scheme
        {
            get
            {
                return this.scheme;
            }
        }
    }
}

