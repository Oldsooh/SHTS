using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WitBird.Common
{
    /// <summary>
    /// Provides a set of method which to filter specified sensitive words, such as phone number, tel number, 
    /// email address, illegal characters.
    /// </summary>
    public static class FilterHelper
    {
        #region Regex patterns

        /// <summary>
        /// Filters phone number
        /// </summary>
        const string PhoneNumberPattern = @"(?is)(\d{3,4}\-)?\d{7,}";
        /// <summary>
        /// Filters http address
        /// </summary>
        const string HttpAddressPattern = @"(http|https|ftp|rtsp|mms)://[a-z0-9]+(\.[a-z0-9]+)+(\?\S+)?";
        /// <summary>
        /// Filters email address with 3 words
        /// </summary>
        const string EmailAddressPattern = @"\w{3}@\w*\.";

        #endregion

        #region Private fields

        private const string DefaultReplacement = "***";
        private static TimeSpan TimeoutSpan = new TimeSpan(0, 0, 10); // 10 seconds.

        #endregion

        #region Public properties

        #endregion 
        
        #region Private methods

        private static string FilterString(this string source, string pattern, string replacement)
        {
            return Regex.Replace(source, pattern, replacement, RegexOptions.IgnoreCase, TimeoutSpan);
        }
        
        #endregion

        #region Public methods

        /// <summary>
        /// Filters all phone number with default replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>Returns the filtered string with default replacement.</returns>
        public static string FilterPhoneNumber(string source)
        {
            return Filter(FilterLevel.PhoneNumber, source, DefaultReplacement);
        }

        /// <summary>
        /// Filters all phone number with specified replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>Returns the filtered string with specified replacement.</returns>
        public static string FilterPhoneNumber(string source, string replacement)
        {
            return Filter(FilterLevel.PhoneNumber, source, replacement);
        }

        /// <summary>
        /// Filters all email address with default replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>Returns the filtered string with default replacement.</returns>
        public static string FilterEmail(string source)
        {
            return Filter(FilterLevel.Email, source, DefaultReplacement);
        }

        /// <summary>
        /// Filters all email address with specified replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>Returns the filtered string with specified replacement.</returns>
        public static string FilterEmail(string source,  string replacement)
        {
            return Filter(FilterLevel.Email, source, replacement);
        }

        /// <summary>
        /// Filters all url address with default replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <returns>Returns the filtered string with default replacement.</returns>
        public static string FilterUrl(string source)
        {
            return Filter(FilterLevel.PhoneNumber, source, DefaultReplacement);
        }

        /// <summary>
        /// Filters all url address with specified replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>Returns the filtered string with specified replacement.</returns>
        public static string FilterUrl(string source, string replacement)
        {
            return Filter(FilterLevel.PhoneNumber, source, replacement);
        }

        /// <summary>
        /// Filters all sensitive words which has matched with speficed filter condition and replaces to default replacement.
        /// </summary>
        /// <param name="level">Filter condtion.</param>
        /// <param name="source">The source string.</param>
        /// <returns>Returns the filtered string.</returns>
        public static string Filter(FilterLevel level, string source)
        {
            return Filter(level, source, DefaultReplacement);
        }

        /// <summary>
        /// Filters all sensitive words which has matched with speficed filter condition and replaces to specified replacement.
        /// </summary>
        /// <param name="level">Filter condtion.</param>
        /// <param name="source">The source string.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>Returns the filtered string.</returns>
        public static string Filter(FilterLevel level, string source, string replacement)
        {
            var result = string.Empty;

            if (string.IsNullOrWhiteSpace(source))
            {
                return result;
            }

            try
            {
                switch (level)
                {
                    case FilterLevel.All:
                        result = source.FilterString(PhoneNumberPattern, replacement)
                            .FilterString(EmailAddressPattern, replacement)
                            .FilterString(HttpAddressPattern, replacement);
                        break;
                    case FilterLevel.Email:
                        result = source.FilterString(EmailAddressPattern, replacement);
                        break;
                    case FilterLevel.PhoneAndEmail:
                        result = source.FilterString(PhoneNumberPattern, replacement)
                            .FilterString(EmailAddressPattern, replacement);
                        break;
                    case FilterLevel.PhoneAndUrl:
                        replacement = source.FilterString(PhoneNumberPattern, replacement)
                            .FilterString(HttpAddressPattern, replacement);
                        break;
                    case FilterLevel.PhoneNumber:
                        result = source.FilterString(PhoneNumberPattern, replacement);
                        break;
                    case FilterLevel.Url:
                        result = source.FilterString(HttpAddressPattern, replacement);
                        break;
                    case FilterLevel.UrlAndEmail:
                        result = source.FilterString(HttpAddressPattern, replacement)
                            .FilterString(EmailAddressPattern, replacement);
                        break;
                    default:
                        result = source;
                        break;
                }
            }
            catch
            {
                result = source;
            }

            return result;
        }

        /// <summary>
        /// Replaces all sensitive words to default replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="sensitiveWord">The sensitive word.</param>
        /// <returns>Returns the filtered string.</returns>
        public static string Filter(string source, string sensitiveWord)
        {
            return Filter(source, sensitiveWord, DefaultReplacement);
        }

        /// <summary>
        /// Replaces all sensitive words to specified replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="sensitiveWord">The sensitive word.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>Returns the filtered string.</returns>
        public static string Filter(string source, string sensitiveWord, string replacement)
        {
            return Filter(source, new List<string> { sensitiveWord }, replacement);
        }

        /// <summary>
        /// Replaces all sensitive words to specified replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="sensitiveWords">The sensitive word list.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>Returns the filtered string.</returns>
        public static string Filter(string source, List<string> sensitiveWords)
        {
            return Filter(source, sensitiveWords, DefaultReplacement);
        }

        /// <summary>
        /// Replaces all sensitive words to specified replacement.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="sensitiveWords">The sensitive word list.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>Returns the filtered string.</returns>
        public static string Filter(string source, List<string> sensitiveWords, string replacement)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(source))
            {
                return result;
            }

            if (sensitiveWords == null || sensitiveWords.Count == 0)
            {
                return source;
            }

            result = source;
            foreach (var item in sensitiveWords)
            {
                result = result.Replace(item, replacement);
            }

            return result;
        }

        #endregion
    }

    /// <summary>
    /// Enums all of filter conditions, such as phone numer, url and email address.
    /// </summary>
    public enum FilterLevel
    {
        /// <summary>
        /// Filters all phone numbers.
        /// </summary>
        PhoneNumber,
        /// <summary>
        /// Filters all http url addresses.
        /// </summary>
        Url,
        /// <summary>
        /// Filters all email addresses.
        /// </summary>
        Email,
        /// <summary>
        /// Filters all phone numbers and url addresses.
        /// </summary>
        PhoneAndUrl,
        /// <summary>
        /// Filters all phone numbers and email addresses.
        /// </summary>
        PhoneAndEmail,
        /// <summary>
        /// Filters all url addresses and email addresses.
        /// </summary>
        UrlAndEmail,
        /// <summary>
        /// Filter all phone numbers, url addresses and email addresses.
        /// </summary>
        All
    }
}
