using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.DAL.New;

namespace Witbird.SHTS.BLL.Managers
{
    public class MiscManager
    {
        LinqToShtsDataContext context = new LinqToShtsDataContext();

        #region 获取杂项列表
        public List<SpaceType> GetSpaceTypeList()
        {
            return context.SpaceTypes.ToList();
        }

        public List<SpaceFeature> GetSpaceFeatureList()
        {
            return context.SpaceFeatures.ToList();
        }

        public List<SpaceFacility> GetSpaceFacilityList()
        {
            return context.SpaceFacilities.ToList();
        }
        public List<SpaceSize> GetSpaceSizeList()
        {
            return context.SpaceSizes.ToList();
        }
        public List<SpacePeople> GetSpacePeopleList()
        {
            return context.SpacePeoples.ToList();
        }
        public List<ActorType> GetActorTypeList()
        {
            return context.ActorTypes.OrderBy(x => x.DisplayOrder).ToList();
        }
        public List<ActorFrom> GetActorFromList()
        {
            return context.ActorFroms.ToList();
        }
        public List<ActorSex> GetActorSexList()
        {
            return context.ActorSexes.ToList();
        }

        public List<EquipType> GetEquipTypeList()
        {
            return context.EquipTypes.ToList();
        }

        public List<OtherType> GetOtherTypeList()
        {
            return context.OtherTypes.ToList();
        }
        #endregion

        #region 获取杂项内容
        public SpaceType GetSpaceTypeById(int id)
        {
            return context.SpaceTypes.FirstOrDefault(v => v.Id == id);
        }

        public SpaceFeature GetSpaceFeatureById(int id)
        {
            return context.SpaceFeatures.FirstOrDefault(v => v.Id == id);
        }

        public SpaceFacility GetSpaceFacilityById(int id)
        {
            return context.SpaceFacilities.FirstOrDefault(v => v.Id == id);
        }

        public SpaceSize GetSpaceSizeById(int id)
        {
            return context.SpaceSizes.FirstOrDefault(v => v.Id == id);
        }

        public SpacePeople GetSpacePeopleById(int id)
        {
            return context.SpacePeoples.FirstOrDefault(v => v.Id == id);
        }

        public ActorType GetActorTypeById(int id)
        {
            return context.ActorTypes.FirstOrDefault(v => v.Id == id);
        }

        public ActorFrom GetActorFromById(int id)
        {
            return context.ActorFroms.FirstOrDefault(v => v.Id == id);
        }

        public ActorSex GetActorSexById(int id)
        {
            return context.ActorSexes.FirstOrDefault(v => v.Id == id);
        }

        public EquipType GetEquipTypeById(int id)
        {
            return context.EquipTypes.FirstOrDefault(v => v.Id == id);
        }

        public OtherType GetOtherTypeById(int id)
        {
            return context.OtherTypes.FirstOrDefault(v => v.Id == id);
        }
        #endregion
    }
}
