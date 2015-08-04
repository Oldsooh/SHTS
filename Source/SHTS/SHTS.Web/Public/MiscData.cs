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

        /// <summary>
        /// 场地类型
        /// </summary>
        public static List<SpaceType> SpaceTypeList
        {
            get
            {
                return manager.GetSpaceTypeList();
            }
        }

        /// <summary>
        /// 场地特点
        /// </summary>
        public static List<SpaceFeature> SpaceFeatureList
        {
            get
            {
                return manager.GetSpaceFeatureList();
            }
        }

        /// <summary>
        /// 场地设施
        /// </summary>
        public static List<SpaceFacility> SpaceFacilityList
        {
            get
            {
                return manager.GetSpaceFacilityList();
            }
        }

        /// <summary>
        /// 场地面积
        /// </summary>
        public static List<SpaceSize> SpaceSizeList
        {
            get
            {
                return manager.GetSpaceSizeList();
            }
        }

        /// <summary>
        /// 场地人数
        /// </summary>
        public static List<SpacePeople> SpacePeopleList
        {
            get
            {
                return manager.GetSpacePeopleList();
            }
        }

        /// <summary>
        /// 演员工作
        /// </summary>
        public static List<ActorType> ActorTypeList
        {
            get
            {
                return manager.GetActorTypeList();
            }
        }

        /// <summary>
        /// 演员组织类别
        /// </summary>
        public static List<ActorFrom> ActorFromList
        {
            get
            {
                return manager.GetActorFromList();
            }
        }

        /// <summary>
        /// 演员性别
        /// </summary>
        public static List<ActorSex> ActorSexList
        {
            get
            {
                return manager.GetActorSexList();
            }
        }

        /// <summary>
        /// 设备类别
        /// </summary>
        public static List<EquipType> EquipTypeList
        {
            get
            {
                return manager.GetEquipTypeList();
            }
        }

        /// <summary>
        /// 其他类别
        /// </summary>
        public static List<OtherType> OtherTypeList
        {
            get
            {
                return manager.GetOtherTypeList();
            }
        }

        public static SpaceType GetSpaceTypeById(string id)
        {
            int x = 0;
            if (int.TryParse(id, out x))
            {
                return manager.GetSpaceTypeById(x);
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
                return manager.GetSpaceFeatureById(x);
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
                return manager.GetSpaceFacilityById(x);
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
                return manager.GetSpaceSizeById(x);
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
                return manager.GetSpacePeopleById(x);
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
                return manager.GetActorTypeById(x);
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
                return manager.GetEquipTypeById(x);
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
                return manager.GetOtherTypeById(x);
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
                return manager.GetActorFromById(x);
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
                return manager.GetActorSexById(x);
            }
            else
            {
                return null;
            }
        }
    }
}
