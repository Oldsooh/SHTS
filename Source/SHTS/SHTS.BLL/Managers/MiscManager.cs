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

        #region 更新类型配置

        public void CreateNewResourceType(string typeName, string name, string description, int displayOrder)
        {
            switch(typeName)
            {
                case "ActorType":
                    var actorType = new ActorType() { Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };
                    InsertActorType(actorType);
                    break;
                case "SpaceType":
                    var spaceType = new SpaceType() { Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };
                    InsertSpaceType(spaceType);
                    break;
                case "EquipType":
                    var equipType = new EquipType() { Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };
                    InsertEquipType(equipType);
                    break;
                case "OtherType":
                    var otherType = new OtherType() { Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };
                    InsertOtherType(otherType);
                    break;
                default:
                    break;

            }
        }

        public void InsertActorType(ActorType type)
        {
            context.ActorTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        public void UpdateActorType(ActorType type)
        {
            var originalType = context.ActorTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                originalType.Description = type.Description;
                originalType.DisplayOrder = type.DisplayOrder;
                originalType.MarkForDelete = type.MarkForDelete;
                originalType.Name = type.Name;
            }

            context.SubmitChanges();
        }

        public void InsertSpaceType(SpaceType type)
        {
            context.SpaceTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        public void UpdateSpaceType(SpaceType type)
        {
            var originalType = context.SpaceTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                originalType.Description = type.Description;
                originalType.DisplayOrder = type.DisplayOrder;
                originalType.MarkForDelete = type.MarkForDelete;
                originalType.Name = type.Name;
            }

            context.SubmitChanges();
        }

        public void InsertEquipType(EquipType type)
        {
            context.EquipTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        public void UpdateEquipType(EquipType type)
        {
            var originalType = context.EquipTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                originalType.Description = type.Description;
                originalType.DisplayOrder = type.DisplayOrder;
                originalType.MarkForDelete = type.MarkForDelete;
                originalType.Name = type.Name;
            }

            context.SubmitChanges();
        }

        public void InsertOtherType(OtherType type)
        {
            context.OtherTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        public void UpdateOtherType(OtherType type)
        {
            var originalType = context.OtherTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                originalType.Description = type.Description;
                originalType.DisplayOrder = type.DisplayOrder;
                originalType.MarkForDelete = type.MarkForDelete;
                originalType.Name = type.Name;
            }

            context.SubmitChanges();
        }

        #endregion
    }
}
