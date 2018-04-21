using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.New;

namespace Witbird.SHTS.BLL.Managers
{
    public class MiscManager
    {
        LinqToShtsDataContext context = new LinqToShtsDataContext(DBHelper.GetSqlConnectionString);

        #region 获取杂项列表
        public List<SpaceType> GetSpaceTypeList()
        {
            return context.SpaceTypes.Where(item => item != null && !item.MarkForDelete).ToList();
        }

        public List<SpaceFeature> GetSpaceFeatureList()
        {
            return context.SpaceFeatures.Where(item => item != null && !item.MarkForDelete).ToList();
        }

        public List<SpaceFacility> GetSpaceFacilityList()
        {
            return context.SpaceFacilities.Where(item => item != null && !item.MarkForDelete).ToList();
        }
        public List<SpaceSize> GetSpaceSizeList()
        {
            return context.SpaceSizes.Where(item => item != null && !item.MarkForDelete).ToList();
        }
        public List<SpacePeople> GetSpacePeopleList()
        {
            return context.SpacePeoples.Where(item => item != null && !item.MarkForDelete).ToList();
        }
        public List<ActorType> GetActorTypeList()
        {
            return context.ActorTypes.Where(item => item != null && !item.MarkForDelete).OrderBy(x => x.DisplayOrder).ToList();
        }
        public List<ActorFrom> GetActorFromList()
        {
            return context.ActorFroms.Where(item => item != null && !item.MarkForDelete).ToList();
        }
        public List<ActorSex> GetActorSexList()
        {
            return context.ActorSexes.Where(item => item != null && !item.MarkForDelete).ToList();
        }

        public List<EquipType> GetEquipTypeList()
        {
            return context.EquipTypes.Where(item => item != null && !item.MarkForDelete).ToList();
        }

        public List<OtherType> GetOtherTypeList()
        {
            return context.OtherTypes.Where(item => item != null && !item.MarkForDelete).ToList();
        }

        public List<BudgetFilter> GetBudgetFilters()
        {
            return context.BudgetFilters.Where(item => item != null && item.IsActive).OrderBy(item => item.DisplayOrder).ToList();
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
