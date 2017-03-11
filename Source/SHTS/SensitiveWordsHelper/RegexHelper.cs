using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WitBird.Common
{
    public static class RegexHelper
    {
        #region Regex patterns

        /// <summary>
        /// Filters phone number
        /// </summary>
        //const string PhoneNumberPattern = @"(?is)(\d{3,4}\-)?\d{7,}";
        private const string PhoneNumberPattern = @"^(((13[0-9]{1})|(15[0-9]{1})|(18[0-9]{1}))+\d{8})$";

        /// <summary>
        /// Filters http address
        /// </summary>
        private const string HttpAddressPattern = @"(http|https|ftp|rtsp|mms)://[a-z0-9]+(\.[a-z0-9]+)+(\?\S+)?";
        
        /// <summary>
        /// Filters email address with 3 words
        /// </summary>
        private const string EmailAddressPattern = @"^[a-z0-9]+([._\\-]*[a-z0-9])*@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){1,63}[a-z0-9]+$";

        private const string UserIdentifiedCardNoFor15DightsPattern = @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$";

        private const string UserIdentifiedCardNoFor18DightsPattern = @"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$";

        #endregion


        #region Public methods

        public static bool IsValidPhoneNumber(string sourcePhoneNumber)
        {
            if (string.IsNullOrWhiteSpace(sourcePhoneNumber))
            {
                return false;
            }

            return Regex.IsMatch(sourcePhoneNumber, PhoneNumberPattern);
        }

        public static bool IsValidEmailAddress(string sourceEmailAddr)
        {
            if (string.IsNullOrWhiteSpace(sourceEmailAddr))
            {
                return false;
            }

            return Regex.IsMatch(sourceEmailAddr, EmailAddressPattern);
        }

        public static bool IsValidHttpAddress(string sourceHttpAddr)
        {
            if (string.IsNullOrWhiteSpace(sourceHttpAddr))
            {
                return false;
            }

            return Regex.IsMatch(sourceHttpAddr, HttpAddressPattern);
        }

        public static bool IsValidUserIdentifiedCardNo(string sourceUserIdentifedCardNo)
        {
            if (string.IsNullOrWhiteSpace(sourceUserIdentifedCardNo))
            {
                return false;
            }

            return Regex.IsMatch(sourceUserIdentifedCardNo, UserIdentifiedCardNoFor18DightsPattern) ||
                Regex.IsMatch(sourceUserIdentifedCardNo, UserIdentifiedCardNoFor15DightsPattern);
        }

        #endregion
    }
}
