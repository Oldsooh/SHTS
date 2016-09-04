using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Witbird.SHTS.BLL;
using Witbird.SHTS.BLL.Managers;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtension
    {
        static MiscManager miscManager = new MiscManager();

        private static object lockObject = new object();

        public static List<SelectListItem> ResourceList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "活动场地", Value = "1" });
            list.Add(new SelectListItem { Text = "演艺人员", Value = "2" });
            list.Add(new SelectListItem { Text = "活动设备", Value = "3" });
            list.Add(new SelectListItem { Text = "其他资源", Value = "4" });

            return list;
        }

        #region 资源类型
        /// <summary>
        /// 活动场地类型
        /// </summary>
        private static List<Witbird.SHTS.DAL.New.SpaceType> spaceTypeList;
        public static List<Witbird.SHTS.DAL.New.SpaceType> SpaceTypeListProperty
        {
            get
            {
                if (spaceTypeList == null)
                {
                    lock (lockObject)
                    {
                        if (spaceTypeList == null)
                        {
                            spaceTypeList = miscManager.GetSpaceTypeList();
                        }
                    }
                }
                return spaceTypeList;
            }
        }
        public static List<SelectListItem> SpaceTypeList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var type in SpaceTypeListProperty)
            {
                list.Add(new SelectListItem { Text = type.Name, Value = type.Id.ToString() });
            }

            return list;
        }

        public static string GetSpaceTypeById(this HtmlHelper helper, int spaceTypeId)
        {
            return GetSpaceTypeById(spaceTypeId);
        }

        private static string GetSpaceTypeById(int spaceTypeId)
        {
            var space = SpaceTypeListProperty.FirstOrDefault(v => v.Id == spaceTypeId);

            return space == null ? "不限类型" : space.Name;
        }

        /// <summary>
        /// 设备类型
        /// </summary>
        private static List<Witbird.SHTS.DAL.New.EquipType> equipTypeList;
        public static List<Witbird.SHTS.DAL.New.EquipType> EquipTypeListProperty
        {
            get
            {
                if (equipTypeList == null)
                {
                    lock (lockObject)
                    {
                        if (equipTypeList == null)
                        {
                            equipTypeList = miscManager.GetEquipTypeList();
                        }
                    }
                }

                return equipTypeList;
            }
        }
        public static List<SelectListItem> EquipTypeList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var type in EquipTypeListProperty)
            {
                list.Add(new SelectListItem { Text = type.Name, Value = type.Id.ToString() });
            }

            return list;
        }
        public static string GetEquipTypeById(this HtmlHelper helper, int equipTypeId)
        {
            return GetEquipTypeById(equipTypeId);
        }

        private static string GetEquipTypeById(int equipTypeId)
        {
            var equip = EquipTypeListProperty.FirstOrDefault(v => v.Id == equipTypeId);

            return equip == null ? "不确定" : equip.Name;
        }

        /// <summary>
        /// 演员类型
        /// </summary>
        private static List<Witbird.SHTS.DAL.New.ActorType> actorTypeList;
        public static List<Witbird.SHTS.DAL.New.ActorType> ActorTypeListProperty
        {
            get
            {
                if (actorTypeList == null)
                {
                    lock (lockObject)
                    {
                        if (actorTypeList == null)
                        {
                            actorTypeList = miscManager.GetActorTypeList();
                        }
                    }
                }
                return actorTypeList;
            }
        }
        public static List<SelectListItem> ActorTypeList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var type in ActorTypeListProperty)
            {
                list.Add(new SelectListItem { Text = type.Name, Value = type.Id.ToString() });
            }

            return list;
        }
        public static string GetActorTypeById(this HtmlHelper helper, int actorTypeId)
        {
            return GetActorTypeById(actorTypeId);
        }

        private static string GetActorTypeById(int actorTypeId)
        {
            var actor = ActorTypeListProperty.FirstOrDefault(v => v.Id == actorTypeId);

            return actor == null ? "不限类型" : actor.Name;
        }
        /// <summary>
        /// 演员组织类型
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static List<SelectListItem> ActorFromList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "个人", Value = "1" });
            list.Add(new SelectListItem { Text = "团体", Value = "2" });
            list.Add(new SelectListItem { Text = "公司", Value = "3" });

            return list;
        }
        /// <summary>
        /// 演员性别
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static List<SelectListItem> ActorSexList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "女", Value = "2" });
            list.Add(new SelectListItem { Text = "男", Value = "1" });

            return list;
        }

        /// <summary>
        /// 其他类型
        /// </summary>
        private static List<Witbird.SHTS.DAL.New.OtherType> otherTypeList;
        public static List<Witbird.SHTS.DAL.New.OtherType> OtherTypeListProperty
        {
            get
            {
                if (otherTypeList == null)
                {
                    lock (lockObject)
                    {
                        if (otherTypeList == null)
                        {
                            otherTypeList = miscManager.GetOtherTypeList();
                        }
                    }
                }
                return otherTypeList;
            }
        }
        public static List<SelectListItem> OtherTypeList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var type in OtherTypeListProperty)
            {
                list.Add(new SelectListItem { Text = type.Name, Value = type.Id.ToString() });
            }

            return list;
        }
        public static string GetOtherTypeById(this HtmlHelper helper, int otherTypeId)
        {
            return GetOtherTypeById(otherTypeId);
        }

        private static string GetOtherTypeById(int otherTypeId)
        {
            var actor = OtherTypeListProperty.FirstOrDefault(v => v.Id == otherTypeId);

            return actor == null ? "不限类型" : actor.Name;
        }
        #endregion

        #region 活动场地元属性
        /// <summary>
        /// 活动场地特点
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static List<Witbird.SHTS.DAL.New.SpaceFeature> spaceFeatureList;
        public static List<Witbird.SHTS.DAL.New.SpaceFeature> SpaceFeatureListProperty
        {
            get
            {
                if (spaceFeatureList == null)
                {
                    lock (lockObject)
                    {
                        if (spaceFeatureList == null)
                        {
                            spaceFeatureList = miscManager.GetSpaceFeatureList();
                        }
                    }
                }
                return spaceFeatureList;
            }
        }
        public static List<SelectListItem> SpaceFeatureList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var feature in SpaceFeatureListProperty)
            {
                list.Add(new SelectListItem { Text = feature.Name, Value = feature.Id.ToString() });
            }

            return list;
        }

        /// <summary>
        /// 活动场地大小
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static List<Witbird.SHTS.DAL.New.SpaceSize> spaceSizeList;
        public static List<Witbird.SHTS.DAL.New.SpaceSize> SpaceSizeListProperty
        {
            get
            {
                if (spaceSizeList == null)
                {
                    lock (lockObject)
                    {
                        if (spaceSizeList == null)
                        {
                            spaceSizeList = miscManager.GetSpaceSizeList();
                        }
                    }
                }
                return spaceSizeList;
            }
        }
        public static List<SelectListItem> SpaceSizeList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var feature in SpaceSizeListProperty)
            {
                list.Add(new SelectListItem { Text = feature.Name, Value = feature.Id.ToString() });
            }

            return list;
        }
        public static string GetSpaceSizeById(this HtmlHelper helper, int spaceSizeId)
        {
            var size = SpaceSizeListProperty.FirstOrDefault(v => v.Id == spaceSizeId);

            return size == null ? "不确定" : size.Name;
        }

        /// <summary>
        /// 场地人数
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        private static List<Witbird.SHTS.DAL.New.SpacePeople> spacePeopleList;
        public static List<Witbird.SHTS.DAL.New.SpacePeople> SpacePeopleListProperty
        {
            get
            {
                if (spacePeopleList == null)
                {
                    lock (lockObject)
                    {
                        if (spacePeopleList == null)
                        {
                            spacePeopleList = miscManager.GetSpacePeopleList();
                        }
                    }
                }
                return spacePeopleList;
            }
        }
        public static List<SelectListItem> SpacePeopleList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var feature in SpacePeopleListProperty)
            {
                list.Add(new SelectListItem { Text = feature.Name, Value = feature.Id.ToString() });
            }

            return list;
        }

        public static string GetResourceSubTypeNameById(this HtmlHelper helper, int resourceTypeId, int? resourceSubTypeId)
        {
            return GetResourceSubTypeNameById(resourceTypeId, resourceSubTypeId);
        }

        public static string GetResourceSubTypeNameById(int resourceTypeId, int? resourceSubTypeId)
        {
            string typeName = string.Empty;
            var subTypeId = resourceSubTypeId ?? -1;

            switch (resourceTypeId)
            {
                case 1:
                    typeName = GetSpaceTypeById(subTypeId);
                    break;
                case 2:
                    typeName = GetActorTypeById(subTypeId);
                    break;
                case 3:
                    typeName = GetEquipTypeById(subTypeId);
                    break;
                case 4:
                    typeName = GetOtherTypeById(subTypeId);
                    break;
                default:
                    typeName = "不限类型";
                    break;
            }

            return typeName;
        }

        public static string GetResourceTypeNameById(this HtmlHelper helper, int resourceTypeId, int resourceSubTypeId)
        {
            string typeName = string.Empty;

            switch (resourceTypeId)
            {
                case 1:
                    typeName = "活动场地/" + GetSpaceTypeById(resourceSubTypeId);
                    break;
                case 2:
                    typeName = "演艺人员/" + GetActorTypeById(resourceSubTypeId);
                    break;
                case 3:
                    typeName = "活动设备/" + GetEquipTypeById(resourceSubTypeId);
                    break;
                case 4:
                    typeName = "其他资源/" + GetOtherTypeById(resourceSubTypeId);
                    break;
                default:
                    typeName = "不限类别/不限类型";
                    break;
            }

            return typeName;
        }

        /// <summary>
        /// 活动场地配套设施复选框组
        /// 后期需要修改为从数据库读取元数据
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="spaceFacilities"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckboxGroupForSpaceFacilities(this HtmlHelper helper, string spaceFacilities)
        {
            StringBuilder builder = new StringBuilder();

            List<int> checkeditems = new List<int>();
            if (!string.IsNullOrEmpty(spaceFacilities) && Regex.IsMatch(spaceFacilities, @"\d+(,\d+)*"))
            {
                checkeditems = spaceFacilities.Split(',').Select(v => int.Parse(v)).ToList();
            }

            builder.Append("<div>");

            foreach (var item in miscManager.GetSpaceFacilityList())
            {
                if (checkeditems.Contains(item.Id))
                {
                    builder.Append(string.Format("<input type=\"checkbox\" class=\"spfc\" value=\"{0}\" checked=\"checked\"/>&nbsp;{1}&nbsp;&nbsp;", item.Id, item.Name));
                }
                else
                {
                    builder.Append(string.Format("<input type=\"checkbox\" class=\"spfc\" value=\"{0}\"/>&nbsp;{1}&nbsp;&nbsp;", item.Id, item.Name));
                }
            }

            builder.Append("</div>");

            return new MvcHtmlString(builder.ToString());
        }

        /// <summary>
        /// 场地特点复选框组
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="spaceFacilities"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckboxGroupForSpaceFeatures(this HtmlHelper helper, string spaceFeatures)
        {
            StringBuilder builder = new StringBuilder();

            List<int> checkeditems = new List<int>();
            if (!string.IsNullOrEmpty(spaceFeatures) && Regex.IsMatch(spaceFeatures, @"\d+(,\d+)*"))
            {
                checkeditems = spaceFeatures.Split(',').Select(v => int.Parse(v)).ToList();
            }

            builder.Append("<div>");

            foreach (var item in miscManager.GetSpaceFeatureList())
            {
                if (checkeditems.Contains(item.Id))
                {
                    builder.Append(string.Format("<input type=\"checkbox\" class=\"spft\" value=\"{0}\" checked=\"checked\"/>&nbsp;{1}&nbsp;&nbsp;", item.Id, item.Name));
                }
                else
                {
                    builder.Append(string.Format("<input type=\"checkbox\" class=\"spft\" value=\"{0}\"/>&nbsp;{1}&nbsp;&nbsp;", item.Id, item.Name));
                }
            }

            builder.Append("</div>");

            return new MvcHtmlString(builder.ToString());
        }

        #endregion

        public static List<SelectListItem> ProviceList(this HtmlHelper helper)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            var provinces = Witbird.SHTS.Web.Public.StaticUtility.GetProvice();
            if (provinces != null)
            {
                list.AddRange(provinces.Select(v => new SelectListItem { Text = v.Name, Value = v.Id }));
            }

            return list;
        }

        public static List<SelectListItem> CityList(this HtmlHelper helper, string provinceId)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(provinceId))
            {
                var cities = Witbird.SHTS.Web.Public.StaticUtility.GetCity(provinceId);
                if (cities != null)
                {
                    list.AddRange(cities.Select(v => new SelectListItem { Text = v.Name, Value = v.Id }));
                }
            }

            return list;
        }

        public static List<SelectListItem> AreaList(this HtmlHelper helper, string cityId)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            if (!string.IsNullOrEmpty(cityId))
            {
                var areas = Witbird.SHTS.Web.Public.StaticUtility.GetArea(cityId);
                if (areas != null)
                {
                    list.AddRange(areas.Select(v => new SelectListItem { Text = v.Name, Value = v.Id }));
                }
            }

            return list;
        }

        public static string GetStatus(this HtmlHelper helper, int statuId)
        {
            string statu = string.Empty;

            switch (statuId)
            {
                case 0:
                    statu = "正常";
                    break;
                case 1:
                    statu = "已删除";
                    break;
                case 2:
                    statu = "未审核";
                    break;
                case 3:
                    statu = "审核通过";
                    break;
                case 4:
                    statu = "审核不通过";
                    break;
                default:
                    statu = "不确定";
                    break;
            }

            return statu;
        }

        public static string CreateShowUrl(this HtmlHelper helper, string channel, int id)
        {
            const string htmlUrl = "/{0}/{1}.html";
            const string sharp = "#";
            if (string.IsNullOrEmpty(channel) || id == 0)
            {
                return sharp;
            }
            return string.Format(htmlUrl, channel, id.ToString());
        }

        public static string CreateMobileShowUrl(this HtmlHelper helper, string channel, int id)
        {
            const string htmlUrl = "/m/{0}/{1}.html";
            const string sharp = "#";
            if (string.IsNullOrEmpty(channel) || id == 0)
            {
                return sharp;
            }
            return string.Format(htmlUrl, channel, id.ToString());
        }
    }
}