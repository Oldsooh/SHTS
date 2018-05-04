using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.DAL.New;

namespace Witbird.SHTS.Web.Public
{
    public static class MiscData
    {
        static MiscManager manager = new MiscManager();


        public static void RefreshResourceTypesCache()
        {
            SpaceTypeList = manager.GetSpaceTypeList();
            ActorTypeList = manager.GetActorTypeList();
            EquipTypeList = manager.GetEquipTypeList();
            OtherTypeList = manager.GetOtherTypeList();
        }

        /// <summary>
        /// 场地类型
        /// </summary>
        public static List<SpaceType> SpaceTypeList { get; private set; } = manager.GetSpaceTypeList();
        /// <summary>
        /// 场地特点
        /// </summary>
        public static List<SpaceFeature> SpaceFeatureList { get; } = manager.GetSpaceFeatureList();
        /// <summary>
        /// 场地设施
        /// </summary>
        public static List<SpaceFacility> SpaceFacilityList { get; } = manager.GetSpaceFacilityList();
        /// <summary>
        /// 场地面积
        /// </summary>
        public static List<SpaceSize> SpaceSizeList { get; } = manager.GetSpaceSizeList();
        /// <summary>
        /// 场地人数
        /// </summary>
        public static List<SpacePeople> SpacePeopleList { get; } = manager.GetSpacePeopleList();
        /// <summary>
        /// 演员工作
        /// </summary>
        public static List<ActorType> ActorTypeList { get; private set; } = manager.GetActorTypeList();
        /// <summary>
        /// 演员组织类别
        /// </summary>
        public static List<ActorFrom> ActorFromList { get; } = manager.GetActorFromList();
        /// <summary>
        /// 演员性别
        /// </summary>
        public static List<ActorSex> ActorSexList { get; } = manager.GetActorSexList();
        /// <summary>
        /// 设备类别
        /// </summary>
        public static List<EquipType> EquipTypeList { get; private set; } = manager.GetEquipTypeList();
        /// <summary>
        /// 其他类别
        /// </summary>
        public static List<OtherType> OtherTypeList { get; private set; } = manager.GetOtherTypeList();

        /// <summary>
        /// 其他类别
        /// </summary>
        public static List<BudgetFilter> BudgetFilters { get; } = manager.GetBudgetFilters();

        public static SpaceType GetSpaceTypeById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return SpaceTypeList.FirstOrDefault(t => t.Id == x);
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
                return SpaceFeatureList.FirstOrDefault(t => t.Id == x);
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
                return SpaceFacilityList.FirstOrDefault(t => t.Id == x);
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
                return SpaceSizeList.FirstOrDefault(t => t.Id == x);
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
                return SpacePeopleList.FirstOrDefault(t => t.Id == x);
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
                return ActorTypeList.FirstOrDefault(t => t.Id == x);
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
                return EquipTypeList.FirstOrDefault(t => t.Id == x);
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
                return OtherTypeList.FirstOrDefault(t => t.Id == x);
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
                return ActorFromList.FirstOrDefault(t => t.Id == x);
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
                return ActorSexList.FirstOrDefault(t => t.Id == x);
            }
            else
            {
                return null;
            }
        }
    }
}
