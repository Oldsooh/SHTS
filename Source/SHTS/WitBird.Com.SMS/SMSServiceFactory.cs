using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Witbird.SHTS.Common;

namespace WitBird.Com.SMS
{
    public sealed class SMSServiceFactory
    {
        private static readonly string FullNamespace =
            ConfigurationManager.AppSettings["SMSService"];

        private static string AssemblyPath = null;
        private static string ClassNamespace = null;

        /// <summary>
        /// 根据配置创建实例。
        /// </summary>
        /// <returns></returns>
        public static ISendShortMessageService Create()
        {
            if (AssemblyPath == null)
            {
                var assemblyInfos=FullNamespace.Split(',');
                AssemblyPath = assemblyInfos[0];
                ClassNamespace = assemblyInfos[1];
            }
            ISendShortMessageService SMSService =
                CreateObject(AssemblyPath, ClassNamespace) as ISendShortMessageService;
            return SMSService;
        }

        /// <summary>
        /// 把实例缓存。
        /// </summary>
        /// <param name="AssemblyPath"></param>
        /// <param name="classNamespace"></param>
        /// <returns></returns>
        private static object CreateObject(string AssemblyPath, string classNamespace)
        {
            object objType = Caching.Get(classNamespace);
            if (objType == null)
            {
                objType = Assembly.Load(AssemblyPath).CreateInstance(classNamespace);
                Caching.Set(classNamespace, objType);// 写入缓存
            }
            return objType;
        }
    }
}
