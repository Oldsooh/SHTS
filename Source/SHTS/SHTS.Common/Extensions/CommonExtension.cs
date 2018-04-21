using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Data;
using Newtonsoft.Json;

namespace Witbird.SHTS.Common.Extensions
{
    /// <summary>
    /// Class CommonExtensions.
    /// </summary>
    public static class CommonExtension
    {
        #region Extensions for all objects

        /// <summary>
        /// To the identity string.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="identity">The identity.</param>
        /// <returns>System.String.</returns>
        public static string ToIdentityString(this object anyObject, string identity)
        {
            return anyObject != null ? string.Format("[{0}:{1}]", anyObject.GetType().ToString(), !string.IsNullOrWhiteSpace(identity) ? identity : "<Unknown>") : "<null>";
        }

        /// <summary>
        /// Creates the XML node.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <returns>XElement.</returns>
        public static XElement CreateXmlNode(this object anyObject, string nodeName = null)
        {
            return nodeName.CreateXml();
        }

        /// <summary>
        /// Creates the XML.
        /// </summary>
        /// <param name="nodeName">Name of the node.</param>
        /// <returns>XElement.</returns>
        public static XElement CreateXml(this string nodeName)
        {
            return XElement.Parse(string.Format("<{0}></{0}>", nodeName.GetStringValue("Item")));
        }

        /// <summary>
        /// Creates the child node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="childNodeName">Name of the child node.</param>
        /// <param name="value">The value.</param>
        /// <returns>XElement.</returns>
        public static XElement CreateChildNode(this XElement parentNode, string childNodeName, object value = null)
        {
            if (parentNode != null && !string.IsNullOrWhiteSpace(childNodeName))
            {
                XElement child = parentNode.CreateXmlNode(childNodeName);

                if (value != null)
                {
                    child.SetValue(value);
                }
                parentNode.Add(child);

                return child;
            }

            return null;
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="defaultString">The default string.</param>
        /// <returns>System.String.</returns>
        public static string GetStringValue(this string anyObject, string defaultString = "")
        {
            return !string.IsNullOrWhiteSpace(anyObject) ? anyObject : defaultString;
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="defaultString">The default string.</param>
        /// <returns>System.String.</returns>
        public static string GetStringValue(this object anyObject, string defaultString = "")
        {
            return anyObject != null ? anyObject.ToString() : defaultString;
        }

        /// <summary>
        /// Gets object value for the specified object.
        /// If the object is null, then return "&lt;null&gt;". Otherwise, return ToString() result.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="obj">The obj.</param>
        /// <returns>System.String.</returns>
        public static string GetObjectValue(this object anyObject, object obj)
        {
            return obj == null ? "<null>" : obj.ToString();
        }

        /// <summary>
        /// Checks the null object.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="objectIdentity">The object identity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CheckNullObject(this object anyObject, object obj, string objectIdentity)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(objectIdentity);
            }
        }

        /// <summary>
        /// Checks the null object.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="objectIdentity">The object identity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CheckNullObject(this object anyObject, string objectIdentity)
        {
            CheckNullObject(anyObject, anyObject, objectIdentity);
        }

        /// <summary>
        /// Checks the empty string.
        /// </summary>
        /// <param name="anyString">Any string.</param>
        /// <param name="objectIdentity">The object identity.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CheckEmptyString(this string anyString, string objectIdentity)
        {
            if (string.IsNullOrWhiteSpace(anyString))
            {
                throw new ArgumentNullException(objectIdentity);
            }
        }

        #endregion

        #region Extension for DB operation

        /// <summary>
        /// To double.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Double.</returns>
        public static double DBToDouble(this object data, double defaultValue = 0)
        {
            double result;
            if (data == null || data == DBNull.Value || !double.TryParse(data.ToString(), out result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// DBs to float.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Single.</returns>
        public static float DBToFloat(this object data, float defaultValue = 0)
        {
            float result;
            if (data == null || data == DBNull.Value || !float.TryParse(data.ToString(), out result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// Databases the automatic nullable float.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Nullable{System.Single}.</returns>
        public static float? DBToNullableFloat(this object data, float? defaultValue = null)
        {
            float result;
            return (data == null || data == DBNull.Value || !float.TryParse(data.ToString(), out result)) ? defaultValue : result;
        }

        /// <summary>
        /// To the int32.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int32.</returns>
        public static int DBToInt32(this object data, int defaultValue = 0)
        {
            int result;
            if (data == null || data == DBNull.Value || !int.TryParse(data.ToString(), out result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// To the long.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Int64.</returns>
        public static long DBToLong(this object data, long defaultValue = 0)
        {
            long result;
            if (data == null || data == DBNull.Value || !long.TryParse(data.ToString(), out result))
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// To the nullable int32.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Nullable{System.Int32}.</returns>
        public static int? DBToNullableInt32(this object data, int? defaultValue = null)
        {
            int result;
            return (data == null || data == DBNull.Value || !int.TryParse(data.ToString(), out result)) ? defaultValue : result;
        }

        /// <summary>
        /// To the nullable int32.
        /// </summary>
        /// <param name="stringObject">The string object.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Nullable{Int32}.</returns>
        public static Nullable<Int32> ToNullableInt32(this string stringObject, Nullable<Int32> defaultValue = null)
        {
            Int32 result = 0;
            return Int32.TryParse(stringObject, out result) ? result : defaultValue;
        }

        /// <summary>
        /// DBs to date time.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>System.Nullable{DateTime}.</returns>
        public static DateTime? DBToDateTime(this object data)
        {
            DateTime? result = null;

            if (data != null && data != DBNull.Value)
            {
                try
                {
                    result = Convert.ToDateTime(data);
                }
                catch
                {
                    result = data as DateTime?;
                }
            }

            if (result != null)
            {
                result = DateTime.SpecifyKind(result.Value, DateTimeKind.Utc);
            }

            return result;
        }

        /// <summary>
        /// To the date time.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>DateTime.</returns>
        public static DateTime DBToDateTime(this object data, DateTime defaultValue)
        {
            DateTime? result = DBToDateTime(data);
            return result == null ? defaultValue : result.Value;
        }

        /// <summary>
        /// To GUID.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Nullable{Guid}.</returns>
        public static Guid? DBToGuid(this object data, Guid? defaultValue = null)
        {
            Guid result;
            return (data == null || data == DBNull.Value || !Guid.TryParse(data.ToString(), out result)) ? defaultValue : result;
        }

        /// <summary>
        /// Databases the automatic decimal.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal DBToDecimal(this object data, decimal defaultValue = 0)
        {
            decimal result;
            return (data == null || data == DBNull.Value || !decimal.TryParse(data.ToString(), out result)) ? defaultValue : result;
        }

        /// <summary>
        /// To nullable boolean
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The nullable boolean value.</returns>
        public static bool? DBToBoolean(this object data, bool? defaultValue = null)
        {
            bool result;
            string dataString = data.GetStringValue();
            int booleanInt;

            return
                int.TryParse(dataString, out booleanInt) ?
                Convert.ToBoolean(booleanInt)
                : ((data == null || data == DBNull.Value || !bool.TryParse(dataString, out result)) ? defaultValue : result);
        }

        /// <summary>
        /// DBs to boolean.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The boolean value. If failed to convert, return <c>false</c>.</returns>
        public static bool DBToBoolean(this object data)
        {
            return DBToBoolean(data, false).Value;
        }

        /// <summary>
        /// To the nullable boolean.
        /// </summary>
        /// <param name="stringObject">The data.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns>The nullable boolean value.</returns>
        public static bool? ToNullableBoolean(this string stringObject, bool? defaultValue = null)
        {
            bool result;
            int booleanInt;

            return
                int.TryParse(stringObject, out booleanInt) ?
                Convert.ToBoolean(booleanInt)
                : ((stringObject == null || !bool.TryParse(stringObject, out result)) ? defaultValue : result);
        }

        /// <summary>
        /// To string.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>System.String.</returns>
        public static string DBToString(this object data, string defaultValue = "")
        {
            return (data == null || data == DBNull.Value) ? defaultValue : data.ToString();
        }

        /// <summary>
        /// Databases to XML.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>XElement.</returns>
        public static XElement DBToXml(this object data)
        {
            var xml = data.GetStringValue();

            if (!string.IsNullOrWhiteSpace(xml))
            {
                try
                {
                    return XElement.Parse(xml);
                }
                catch (Exception ex)
                {
                    throw new Exception("DBToXml", ex);
                }
            }

            return null;
        }

        #endregion

        #region Type Convert Extensions

        /// <summary>
        /// To the int32.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns>System.Int32.</returns>
        public static int ToInt32(this Guid guid)
        {
            byte[] seed = guid.ToByteArray();
            for (int i = 0; i < 3; i++)
            {
                seed[i] ^= seed[i + 4];
                seed[i] ^= seed[i + 8];
                seed[i] ^= seed[i + 12];
            }

            return BitConverter.ToInt32(seed, 0);
        }

        /// <summary>
        /// Enums to int32.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue">The enum value.</param>
        /// <returns>System.Nullable{System.Int32}.</returns>
        public static int? EnumToInt32<T>(this Nullable<T> enumValue) where T : struct,IConvertible
        {
            int? result = null;
            if (enumValue != null)
            {
                IConvertible convertible = enumValue.Value;
                result = convertible.ToInt32(CultureInfo.InvariantCulture);
            }

            return result;
        }

        /// <summary>
        /// To boolean.
        /// </summary>
        /// <param name="stringObject">The string object.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns>The boolean result. If failed to concert, return <c>false</c>.</returns>
        public static bool ToBoolean(this string stringObject, bool defaultValue = false)
        {
            bool result = defaultValue;
            if (stringObject == "1")
            {
                result = true;
            }
            else
            {
                Boolean.TryParse(stringObject, out result);
            }

            return result;
        }

        /// <summary>
        /// Inners the string.
        /// </summary>
        /// <param name="anyString">Any string.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns>System.String.</returns>
        public static string InnerString(this string anyString, string start, string end)
        {
            string result = anyString.GetStringValue();

            start = start.GetStringValue();
            end = end.GetStringValue();

            int startIndex = start.Length == 0 ? 0 : result.IndexOf(start);
            if (startIndex < 0)
            {
                startIndex = 0;
            }

            int endIndex = end.Length == 0 ? result.Length : result.IndexOf(end, startIndex);
            if (endIndex < 0)
            {
                endIndex = result.Length;
            }

            result = result.Substring(startIndex + start.Length, endIndex - startIndex - start.Length);

            return result;
        }

        /// <summary>
        /// To the int32.
        /// </summary>
        /// <param name="stringObject">The string object.</param>
        /// <returns>Int32.</returns>
        public static Int32 ToInt32(this string stringObject)
        {
            Int32 result = 0;
            Int32.TryParse(stringObject, out result);
            return result;
        }

        /// <summary>
        /// To the double.
        /// </summary>
        /// <param name="stringObject">The string object.</param>
        /// <returns>Double.</returns>
        public static Double ToDouble(this string stringObject)
        {
            Double result = 0;
            Double.TryParse(stringObject, out result);
            return result;
        }

        /// <summary>
        /// To the decimal.
        /// </summary>
        /// <param name="stringObject">The string object.</param>
        /// <returns>System.Decimal.</returns>
        public static decimal ToDecimal(this string stringObject)
        {
            Decimal result = 0;
            Decimal.TryParse(stringObject, out result);
            return result;
        }

        /// <summary>
        /// Automatics the unique identifier.
        /// </summary>
        /// <param name="stringObject">The string object.</param>
        /// <param name="defaultGuid">The default unique identifier.</param>
        /// <returns>System.Nullable{Guid}.</returns>
        public static Guid? ToGuid(this string stringObject, Guid? defaultGuid = null)
        {
            Guid output;
            return Guid.TryParse(stringObject, out output) ?
                output
                : defaultGuid;
        }

        #endregion

        #region XElement Extensiosn

        /// <summary>
        /// Tries the get child value.
        /// </summary>
        /// <param name="anyXml">Any XML.</param>
        /// <param name="childNodeName">Name of the child node.</param>
        /// <returns>System.String.</returns>
        public static string TryGetChildValue(this XElement anyXml, string childNodeName)
        {
            XElement child = null;

            if (anyXml != null)
            {
                child = string.IsNullOrWhiteSpace(childNodeName) ? anyXml.Elements().FirstOrDefault() : anyXml.Element(childNodeName);
            }

            return child == null ? string.Empty : child.Value;
        }

        /// <summary>
        /// Finds the child elements by tag with attribute.
        /// </summary>
        /// <param name="anyXml">Any XML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>List{XElement}.</returns>
        public static List<XElement> FindChildElementsByTagWithAttribute(this XElement anyXml, string tagName, string attributeName, string attributeValue)
        {
            return FindChildElements(anyXml, tagName, attributeName, attributeValue, false);
        }

        /// <summary>
        /// Finds the child elements by tag with attribute.
        /// </summary>
        /// <param name="anyXml">Any XML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="hasAttributeName">Name of the has attribute.</param>
        /// <returns>List{XElement}.</returns>
        public static List<XElement> FindChildElementsByTagWithAttribute(this XElement anyXml, string tagName, string hasAttributeName)
        {
            return FindChildElements(anyXml, tagName, hasAttributeName, null, false);
        }

        /// <summary>
        /// Finds the child elements by attribute.
        /// </summary>
        /// <param name="anyXml">Any XML.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>List{XElement}.</returns>
        public static List<XElement> FindChildElementsByAttribute(this XElement anyXml, string attributeName, string attributeValue)
        {
            return FindChildElements(anyXml, null, attributeName, attributeValue, false);
        }

        /// <summary>
        /// Finds the first child element by attribute.
        /// </summary>
        /// <param name="anyXml">Any XML.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>XElement.</returns>
        public static XElement FindFirstChildElementByAttribute(this XElement anyXml, string attributeName, string attributeValue)
        {
            return FindChildElements(anyXml, null, attributeName, attributeValue, true).FirstOrDefault();
        }

        /// <summary>
        /// Finds the first child element by tag with attribute.
        /// </summary>
        /// <param name="anyXml">Any XML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <returns>XElement.</returns>
        public static XElement FindFirstChildElementByTagWithAttribute(this XElement anyXml, string tagName, string attributeName, string attributeValue)
        {
            return FindChildElements(anyXml, tagName, attributeName, attributeValue, true).FirstOrDefault();
        }

        /// <summary>
        /// Finds the child elements by tag with attribute.
        /// </summary>
        /// <param name="anyXml">Any XML.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="attributeValue">The attribute value.</param>
        /// <param name="findOnlyOne">if set to <c>true</c> [find only one].</param>
        /// <returns>List{XElement}.</returns>
        private static List<XElement> FindChildElements(this XElement anyXml, string tagName, string attributeName, string attributeValue, bool findOnlyOne = false)
        {
            List<XElement> result = new List<XElement>();

            if (anyXml != null)
            {
                var children = string.IsNullOrWhiteSpace(tagName) ? anyXml.Elements() : anyXml.Elements(tagName);

                foreach (var one in children)
                {
                    if (string.IsNullOrWhiteSpace(attributeName) ||
                                            (one.Attribute(attributeName) != null &&
                                                (string.IsNullOrWhiteSpace(attributeValue) || attributeValue == one.Attribute(attributeName).Value)))
                    {
                        result.Add(one);

                        if (findOnlyOne)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified element has elements; otherwise, <c>false</c>.</returns>
        public static bool HasElements(this XElement element, string name = null)
        {
            return element != null
                && (!string.IsNullOrEmpty(name) ? element.Elements(name).Count() > 0 : element.Elements().Count() > 0);
        }

        /// <summary>
        /// Gets the attribute value.
        /// </summary>
        /// <param name="xElement">The x element.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>System.String.</returns>
        public static string GetAttributeValue(this XElement xElement, string attribute)
        {
            string result = string.Empty;

            if (xElement != null && !string.IsNullOrWhiteSpace(attribute))
            {
                var attr = xElement.Attribute(attribute);
                if (attr != null)
                {
                    result = attr.Value;
                }
            }

            return result;
        }

        #endregion

        #region IEnumerable, ICollection, IList

        /// <summary>
        /// Joins the within format.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="format">The format.</param>
        /// <returns>System.String.</returns>
        public static string JoinWithinFormat<T>(this  IEnumerable<T> instance, string format)
        {
            StringBuilder builder = new StringBuilder();

            if (instance != null && !string.IsNullOrWhiteSpace(format))
            {
                int index = 1;
                foreach (var one in instance)
                {
                    builder.AppendFormat(format, index, one.ToString());
                    index++;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Joins the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="separator">The separator.</param>
        /// <returns>System.String.</returns>
        public static string Join<T>(this IEnumerable<T> instance, string separator)
        {
            return string.Join<T>(separator, instance);
        }

        #endregion

        #region Bytes

        /// <summary>
        /// Reads the stream to bytes.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="closeWhenFinish">if set to <c>true</c> [close when finish].</param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] ReadStreamToBytes(this Stream stream, bool closeWhenFinish = false)
        {
            long originalPosition = 0;

            try
            {
                stream.CheckNullObject("stream");

                if (stream.CanSeek)
                {
                    originalPosition = stream.Position;
                    stream.Position = 0;
                }

                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream != null)
                {
                    if (stream.CanSeek)
                    {
                        stream.Position = originalPosition;
                    }

                    if (closeWhenFinish)
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                }

            }
        }


        /// <summary>
        /// To the bytes.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The <see cref="Byte" />  array of stream.</returns>
        /// <exception cref="OperationFailureException">StreamToBytes</exception>
        public static byte[] ToBytes(this Stream stream)
        {
            return ReadStreamToBytes(stream, true);
        }

        /// <summary>
        /// To the stream.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>The <see cref="Stream" />  for byte array.</returns>
        /// <exception cref="OperationFailureException">BytesToStream</exception>
        public static Stream ToStream(this byte[] bytes)
        {
            Stream stream = null;

            try
            {
                if (bytes != null)
                {
                    stream = new MemoryStream(bytes);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return stream;
        }

        #endregion

        #region IO

        /// <summary>
        /// The dot
        /// </summary>
        const string dot = ".";

        /// <summary>
        /// Combines the extension.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="pureFileName">Name of the pure file.</param>
        /// <param name="extension">The extension.</param>
        /// <returns>System.String.</returns>
        public static string CombineExtension(this object anyObject, string pureFileName, string extension)
        {
            if (!string.IsNullOrWhiteSpace(extension))
            {
                extension = dot + extension.Replace(dot, string.Empty);
            }

            return anyObject.GetStringValue(pureFileName) + extension;
        }

        /// <summary>
        /// Gets the temporary folder.
        /// If the tempIdentity is null, method would assign one.
        /// If the folder of tempIdentity is not existed, it would be created.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="tempIdentity">The temporary identity.</param>
        /// <returns>DirectoryInfo.</returns>
        public static DirectoryInfo GetTempFolder(this object anyObject, ref Guid? tempIdentity)
        {
            if (tempIdentity == null)
            {
                tempIdentity = Guid.NewGuid();
            }

            var path = Path.Combine(Path.GetTempPath(), tempIdentity.Value.ToString());
            var directory = new DirectoryInfo(path);

            if (!directory.Exists)
            {
                directory.Create();
            }

            return directory;
        }

        /// <summary>
        /// Reads the file content lines.
        /// This method would not impact the conflict for reading and writing.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String[][].</returns>
        /// <exception cref="OperationFailureException">GetFileContentLines</exception>
        public static string[] ReadFileContentLines(this object anyObject, string path, Encoding encoding)
        {
            Stream stream = null;

            StreamReader streamReader = null;
            List<string> lines = new List<string>();

            try
            {
                stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                streamReader = new StreamReader(stream, encoding); string stringLine = string.Empty;

                do
                {
                    stringLine = streamReader.ReadLine();
                    lines.Add(stringLine);
                }
                while (stringLine != null);

                if (lines.Count > 0)
                {
                    lines.RemoveAt(lines.Count - 1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                streamReader.Close();
                stream.Close();
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Reads the file content lines.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="path">The path.</param>
        /// <returns>System.String[][].</returns>
        public static string[] ReadFileContentLines(this object anyObject, string path)
        {
            return ReadFileContentLines(anyObject, path, Encoding.UTF8);
        }

        /// <summary>
        /// Reads the file contents.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="path">The path.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="OperationFailureException">ReadFileContens</exception>
        public static string ReadFileContents(this object anyObject, string path, Encoding encoding)
        {
            Stream stream = null;
            StreamReader streamReader = null;

            try
            {
                stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                streamReader = new StreamReader(stream, encoding);
                return streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                streamReader.Close();
                stream.Close();
            }
        }

        /// <summary>
        /// Reads the file contents.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        public static string ReadFileContents(this object anyObject, string path)
        {
            return ReadFileContents(anyObject, path);
        }

        /// <summary>
        /// Reads the file bytes.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="path">The path.</param>
        /// <returns>System.Byte[][].</returns>
        /// <exception cref="OperationFailureException">ReadFileBytes</exception>
        public static byte[] ReadFileBytes(this object anyObject, string path)
        {
            Stream stream = null;
            StreamReader streamReader = null;

            try
            {
                stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return stream.ReadFileBytes(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                streamReader.Close();
                stream.Close();
            }
        }

        #endregion

        #region Regex

        /// <summary>
        /// Gets the regex match value.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="content">The content.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>System.String.</returns>
        public static string GetRegexMatchValue(this Regex regex, string content, string variable)
        {
            string result = string.Empty;

            if (regex != null && !string.IsNullOrWhiteSpace(content) && variable != null)
            {
                var match = regex.Match(content);
                if (match.Success)
                {
                    result = match.Result("${" + variable + "}");
                }
            }

            return result;
        }


        /// <summary>
        /// Gets the regex match values.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="content">The content.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>System.String[][].</returns>
        public static string[] GetRegexMatchValues(this Regex regex, string content, string variable)
        {
            List<string> result = new List<string>();

            if (regex != null && !string.IsNullOrWhiteSpace(content) && variable != null)
            {
                var matches = regex.Matches(content);
                if (matches.Count > 0)
                {
                    foreach (Match one in matches)
                    {
                        string value = one.Result("${" + variable + "}");
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            result.Add(value);
                        }
                    }
                }
            }

            return result.ToArray();
        }

        /// <summary>
        /// Fills the regex match value.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="content">The content.</param>
        /// <param name="variables">The variables.</param>
        /// <returns>Dictionary{System.StringSystem.String}.</returns>
        public static Dictionary<string, string> GetRegexMatchValues(this Regex regex, string content, Dictionary<string, string> variables)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (regex != null && !string.IsNullOrWhiteSpace(content) && variables != null)
            {
                var match = regex.Match(content);
                if (match.Success)
                {
                    foreach (var key in variables.Keys)
                    {
                        result.Add(key, match.Result("${" + key + "}"));
                    }
                }
            }

            return result;
        }

        #endregion

        #region Json

        /// <summary>
        /// Jsons to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson">The STR json.</param>
        /// <returns>``0.</returns>
        public static T JsonToObject<T>(this string strJson) where T : new()
        {
            T t = new T();

            if (!string.IsNullOrWhiteSpace(strJson))
            {
                t = JsonConvert.DeserializeObject<T>(strJson);
            }

            return t;
        }

        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <returns>System.String.</returns>
        public static string ToJson(this object anyObject)
        {
            string result = string.Empty;

            if (anyObject != null)
            {
                result = JsonConvert.SerializeObject(anyObject);
            }

            return result;
        }

        #endregion

        #region Thread

        /// <summary>
        /// Sets the thread data.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void SetThreadData(this object anyObject, string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                Thread.SetData(Thread.GetNamedDataSlot(key), value);
            }
        }

        /// <summary>
        /// Gets the thread data.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="key">The key.</param>
        /// <returns>System.Object.</returns>
        public static object GetThreadData(this object anyObject, string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return Thread.GetData(Thread.GetNamedDataSlot(key));
            }

            return null;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// To the pure XML.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="createDeclaration">if set to <c>true</c> [create declaration].</param>
        /// <returns>XElement.</returns>
        public static XElement ToPureXml(this object anyObject, bool createDeclaration = false)
        {
            if (anyObject != null)
            {
                StringBuilder stringBuilder = new StringBuilder();

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                //Add an empty namespace and empty value
                ns.Add(string.Empty, string.Empty);
                XmlWriterSettings settings = new XmlWriterSettings();

                settings.OmitXmlDeclaration = !createDeclaration; // Remove the <?xml version="1.0" encoding="utf-8"?>

                XmlWriter writer = XmlWriter.Create(stringBuilder, settings);
                //Create the serializer
                XmlSerializer serializer = new XmlSerializer(anyObject.GetType());

                //Serialize the object with our own namespaces (notice the overload)
                serializer.Serialize(writer, anyObject, ns);
                writer.Flush();

                return XElement.Parse(stringBuilder.ToString());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Converts pure xml to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlObject">The XML object.</param>
        /// <returns>T.</returns>
        public static T PureXmlToObject<T>(this XElement xmlObject)
        {
            XmlReader reader = xmlObject.CreateReader();

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            var obj = serializer.Deserialize(reader);

            if (obj != null)
            {
                return (T)obj;
            }
            else
            {
                return default(T);
            }
        }

        #endregion

        #region Random

        /// <summary>
        /// The random
        /// </summary>
        private static Random random = new Random();

        /// <summary>
        /// The alpha
        /// </summary>
        private static char[] alpha = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        /// <summary>
        /// Gets the random number only.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public static string CreateRandomNumberString(this object anyObject, int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(alpha[random.Next(10)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the random hex string.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public static string CreateRandomHexString(this object anyObject, int length)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(alpha[random.Next(0, 16)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the random hex.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.Byte[].</returns>
        public static byte[] CreateRandomHex(this object anyObject, int length)
        {
            byte[] result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(alpha[random.Next(0, 16)]);
                sb.Append(alpha[random.Next(0, 16)]);

                result[i] = Convert.ToByte(sb.ToString(), 16);
            }

            return result;
        }

        /// <summary>
        /// Gets the random string.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String.</returns>
        public static string CreateRandomString(this object anyObject, int length)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(alpha[random.Next(36)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Generates the name of the upload file.
        /// </summary>
        /// <param name="anyObject">Any object.</param>
        /// <param name="suffixRndLength">Length of the suffix RND.</param>
        /// <returns>System.String.</returns>
        public static string GenerateUploadFileName(this  object anyObject, int suffixRndLength)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00"));
            sb.Append(DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00"));
            sb.Append(DateTime.Now.Millisecond.ToString("000"));
            sb.Append(sb.CreateRandomString(suffixRndLength));

            return sb.ToString();
        }



        #endregion

        #region String

        /// <summary>
        /// MD5 hash加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToMD5(this string s)
        {
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var result = BitConverter.ToString(md5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(s.Trim())));
            return result;
        }

        #endregion

        #region StringBuilder

        /// <summary>
        /// Appends the indent.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="amount">The amount.</param>
        public static void AppendIndent(this StringBuilder builder, char indentChar, int amount)
        {
            if (builder != null && amount > 0)
            {
                builder.Append(new string(indentChar, amount));
            }
        }

        /// <summary>
        /// Appends the line with format.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void AppendLineWithFormat(this StringBuilder builder, string format, params object[] args)
        {
            if (builder != null && !string.IsNullOrWhiteSpace(format))
            {
                builder.AppendLine(string.Format(format, args));
            }
        }

        /// <summary>
        /// Appends the line with format.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="indentChar">The indent character.</param>
        /// <param name="indentAmount">The indent amount.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void AppendLineWithFormat(this StringBuilder builder, char indentChar, int indentAmount, string format, params object[] args)
        {
            if (builder != null && !string.IsNullOrWhiteSpace(format))
            {
                builder.AppendIndent(indentChar, indentAmount);
                builder.AppendLineWithFormat(format, args);
            }
        }

        #endregion


        /// <summary>
        /// Determines whether the specified data reader has column.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns><c>true</c> if the specified dr has column; otherwise, <c>false</c>.</returns>
        public static bool HasColumn(this IDataRecord dataReader, string columnName)
        {
            if (dataReader != null)
            {
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    if (dataReader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
