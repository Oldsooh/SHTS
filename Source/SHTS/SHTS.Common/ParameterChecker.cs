using System;
using System.Collections.Generic;
using System.Linq;

namespace Witbird.SHTS.Common
{
    public static class ParameterChecker
    {
        /// <summary>
        /// Checks object
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Check(object parameter, string parameterName)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Checks object
        /// </summary>
        /// <param name="parameter"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Check(object parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Checks string
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void Check(string parameter, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Determines whether the specified instance has item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns><c>true</c> if the specified instance has item; otherwise, <c>false</c>.</returns>
        public static bool HasItem<T>(this IEnumerable<T> instance)
        {
            return instance != null && instance.Count() > 0;
        }

        /// <summary>
        /// Determines whether the specified object is null or not.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>True if the obj is null. Otherwise false.</returns>
        public static bool IsNotNull(this object obj)
        {
            return obj != null;
        }
    }
}
