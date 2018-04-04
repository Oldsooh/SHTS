using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.DAL.New;

namespace Witbird.SHTS.Web.Public
{
    public static class MiscData
    {
        static MiscManager manager = new MiscManager();

        static List<SpaceType> spaceTypeList = manager.GetSpaceTypeList();

        /// <summary>
        /// 场地类型
        /// </summary>
        public static List<SpaceType> SpaceTypeList
        {
            get
            {
                return spaceTypeList;
            }
        }

        static List<SpaceFeature> spaceFeatureList = manager.GetSpaceFeatureList();
        /// <summary>
        /// 场地特点
        /// </summary>
        public static List<SpaceFeature> SpaceFeatureList
        {
            get
            {
                return spaceFeatureList;
            }
        }

        static List<SpaceFacility> spaceFacilityList = manager.GetSpaceFacilityList();
        /// <summary>
        /// 场地设施
        /// </summary>
        public static List<SpaceFacility> SpaceFacilityList
        {
            get
            {
                return spaceFacilityList;
            }
        }

        static List<SpaceSize> spaceSizeList = manager.GetSpaceSizeList();
        /// <summary>
        /// 场地面积
        /// </summary>
        public static List<SpaceSize> SpaceSizeList
        {
            get
            {
                return spaceSizeList;
            }
        }

        static List<SpacePeople> spacePeopleList = manager.GetSpacePeopleList();
        /// <summary>
        /// 场地人数
        /// </summary>
        public static List<SpacePeople> SpacePeopleList
        {
            get
            {
                return spacePeopleList;
            }
        }

        static List<ActorType> actorTypeList = manager.GetActorTypeList();
        /// <summary>
        /// 演员工作
        /// </summary>
        public static List<ActorType> ActorTypeList
        {
            get
            {
                return actorTypeList;
            }
        }

        static List<ActorFrom> actorFromList = manager.GetActorFromList();
        /// <summary>
        /// 演员组织类别
        /// </summary>
        public static List<ActorFrom> ActorFromList
        {
            get
            {
                return actorFromList;
            }
        }

        static List<ActorSex> actorSexList = manager.GetActorSexList();
        /// <summary>
        /// 演员性别
        /// </summary>
        public static List<ActorSex> ActorSexList
        {
            get
            {
                return actorSexList;
            }
        }

        static List<EquipType> equipTypeList = manager.GetEquipTypeList();
        /// <summary>
        /// 设备类别
        /// </summary>
        public static List<EquipType> EquipTypeList
        {
            get
            {
                return equipTypeList;
            }
        }

        static List<OtherType> otherTypeList = manager.GetOtherTypeList();
        /// <summary>
        /// 其他类别
        /// </summary>
        public static List<OtherType> OtherTypeList
        {
            get
            {
                return otherTypeList;
            }
        }

        /// <summary>
        /// 其他类别
        /// </summary>
        public static List<QuotePriceCategory> BudgetFilters { get; } = manager.GetBudgetFilters();

        public static SpaceType GetSpaceTypeById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return spaceTypeList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }

        public static SpaceFeature GetSpaceFeatureById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return spaceFeatureList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }

        public static SpaceFacility GetSpaceFacilityById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return spaceFacilityList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }
        public static SpaceSize GetSpaceSizeById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return spaceSizeList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }
        public static SpacePeople GetSpacePeopleById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return spacePeopleList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }
        public static ActorType GetActorTypeById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return actorTypeList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }
        public static EquipType GetEquipTypeById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return equipTypeList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }

        public static OtherType GetOtherTypeById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return otherTypeList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }

        public static ActorFrom GetActorFromById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return actorFromList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }

        public static ActorSex GetActorSexById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return actorSexList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }
    }
}
