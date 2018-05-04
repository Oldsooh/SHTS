using System;
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
            return context.SpaceTypes.Where(item => item != null && !item.MarkForDelete).OrderBy(x => x.DisplayOrder).ToList();
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
            return context.EquipTypes.Where(item => item != null && !item.MarkForDelete).OrderBy(x => x.DisplayOrder).ToList();
        }

        public List<OtherType> GetOtherTypeList()
        {
            return context.OtherTypes.Where(item => item != null && !item.MarkForDelete).OrderBy(x => x.DisplayOrder).ToList();
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

        #region 资源类型配置

        public List<ResourceType> GetResourceTypes(string resourceTypeKey, int pageIndex, int pageSize, out int totalCount)
        {
            var resourceTypes = new List<ResourceType>();
            totalCount = 0;

            // selects all types
            if (string.IsNullOrWhiteSpace(resourceTypeKey))
            {
                var allTypes = context.SpaceTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                {
                    ResourceTypeKey = "1",
                    ResourceTypeName = "活动场地",
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    DisplayOrder = item.DisplayOrder,
                    MarkForDelete = item.MarkForDelete
                }).Concat(context.ActorTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                {
                    ResourceTypeKey = "2",
                    ResourceTypeName = "演艺人员",
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    DisplayOrder = item.DisplayOrder,
                    MarkForDelete = item.MarkForDelete
                })).Concat(context.EquipTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                {
                    ResourceTypeKey = "3",
                    ResourceTypeName = "活动设备",
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    DisplayOrder = item.DisplayOrder,
                    MarkForDelete = item.MarkForDelete
                })).Concat(context.OtherTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                {
                    ResourceTypeKey = "4",
                    ResourceTypeName = "其他资源",
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    DisplayOrder = item.DisplayOrder,
                    MarkForDelete = item.MarkForDelete
                }));

                totalCount = allTypes.Count();
                resourceTypes.AddRange(allTypes.Skip((pageIndex - 1) * pageSize).Take(pageSize));
            }
            else
            {
                switch (resourceTypeKey)
                {
                    case "1":
                        totalCount = context.SpaceTypes.Where(item => !item.MarkForDelete).Count();
                        resourceTypes.AddRange(context.SpaceTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                        {
                            ResourceTypeKey = "2",
                            ResourceTypeName = "活动场地",
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            DisplayOrder = item.DisplayOrder,
                            MarkForDelete = item.MarkForDelete
                        }).Skip((pageIndex - 1) * pageSize).Take(pageSize));
                        break;
                    case "2":
                        totalCount = context.ActorTypes.Where(item => !item.MarkForDelete).Count();
                        resourceTypes.AddRange(context.ActorTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                        {
                            ResourceTypeKey = "1",
                            ResourceTypeName = "演艺人员",
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            DisplayOrder = item.DisplayOrder,
                            MarkForDelete = item.MarkForDelete
                        }).Skip((pageIndex - 1) * pageSize).Take(pageSize));
                        break;
                    case "3":
                        totalCount = context.EquipTypes.Where(item => !item.MarkForDelete).Count();
                        resourceTypes.AddRange(context.EquipTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                        {
                            ResourceTypeKey = "3",
                            ResourceTypeName = "活动设备",
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            DisplayOrder = item.DisplayOrder,
                            MarkForDelete = item.MarkForDelete
                        }).Skip((pageIndex - 1) * pageSize).Take(pageSize));
                        break;
                    case "4":
                        totalCount = context.OtherTypes.Where(item => !item.MarkForDelete).Count();
                        resourceTypes.AddRange(context.OtherTypes.Where(item => !item.MarkForDelete).OrderBy(item => item.DisplayOrder).Select(item => new ResourceType()
                        {
                            ResourceTypeKey = "4",
                            ResourceTypeName = "其他资源",
                            Id = item.Id,
                            Name = item.Name,
                            Description = item.Description,
                            DisplayOrder = item.DisplayOrder,
                            MarkForDelete = item.MarkForDelete
                        }).Skip((pageIndex - 1) * pageSize).Take(pageSize));
                        break;
                    default:
                        break;
                }
            }

            return resourceTypes;
        }

        public void CreateOrUpdateResourceType(string resourceTypeKey, int typeId, string name, string description, int displayOrder, bool isDelete)
        {
            var isUpdate = typeId > 0;
            switch (resourceTypeKey)
            {
                case "1":
                    var spaceType = new SpaceType() { Id = typeId, Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };
                    if (isUpdate)
                        UpdateSpaceType(spaceType, isDelete);
                    else
                        InsertSpaceType(spaceType);
                    break;
                case "2":
                    var actorType = new ActorType() { Id = typeId, Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };

                    if (isUpdate)
                        UpdateActorType(actorType, isDelete);
                    else
                        InsertActorType(actorType);
                    break;
                case "3":
                    var equipType = new EquipType() { Id = typeId, Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };
                    if (isUpdate)
                        UpdateEquipType(equipType, isDelete);
                    else
                        InsertEquipType(equipType);
                    break;
                case "4":
                    var otherType = new OtherType() { Id = typeId, Description = description, DisplayOrder = displayOrder, MarkForDelete = false, Name = name };
                    if (isUpdate)
                        UpdateOtherType(otherType, isDelete);
                    else
                        InsertOtherType(otherType);
                    break;
                default:
                    break;

            }
        }

        public ResourceType GetResourceById(string resourceTypeKey, int typeId)
        {
            ResourceType resourceType = null;
            switch (resourceTypeKey)
            {
                case "1":
                    var spaceType = GetSpaceTypeById(typeId);
                    resourceType = new ResourceType()
                    {
                        Description = spaceType.Description,
                        DisplayOrder = spaceType.DisplayOrder,
                        Id = spaceType.Id,
                        MarkForDelete = spaceType.MarkForDelete,
                        Name = spaceType.Name,
                        ResourceTypeKey = resourceTypeKey,
                        ResourceTypeName = "活动场地"
                    };
                    break;
                case "2":
                    var actorType = GetActorTypeById(typeId);
                    resourceType = new ResourceType()
                    {
                        Description = actorType.Description,
                        DisplayOrder = actorType.DisplayOrder,
                        Id = actorType.Id,
                        MarkForDelete = actorType.MarkForDelete,
                        Name = actorType.Name,
                        ResourceTypeKey = resourceTypeKey,
                        ResourceTypeName = "演艺人员"
                    };
                    break;
                case "3":
                    var equipType = GetEquipTypeById(typeId);
                    resourceType = new ResourceType()
                    {
                        Description = equipType.Description,
                        DisplayOrder = equipType.DisplayOrder,
                        Id = equipType.Id,
                        MarkForDelete = equipType.MarkForDelete,
                        Name = equipType.Name,
                        ResourceTypeKey = resourceTypeKey,
                        ResourceTypeName = "活动设备"
                    };
                    break;
                case "4":
                    var otherType = GetOtherTypeById(typeId);
                    resourceType = new ResourceType()
                    {
                        Description = otherType.Description,
                        DisplayOrder = otherType.DisplayOrder,
                        Id = otherType.Id,
                        MarkForDelete = otherType.MarkForDelete,
                        Name = otherType.Name,
                        ResourceTypeKey = resourceTypeKey,
                        ResourceTypeName = "其他资源"
                    };
                    break;
                default:
                    throw new Exception("资源类型分类不正确");
            }

            return resourceType;
        }

        private void InsertActorType(ActorType type)
        {
            context.ActorTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        private void UpdateActorType(ActorType type, bool isDelete = false)
        {
            var originalType = context.ActorTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                if (!isDelete)
                {
                    originalType.Description = type.Description;
                    originalType.DisplayOrder = type.DisplayOrder;
                    originalType.Name = type.Name;
                }

                originalType.MarkForDelete = isDelete;
            }

            context.SubmitChanges();
        }

        private void InsertSpaceType(SpaceType type)
        {
            context.SpaceTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        private void UpdateSpaceType(SpaceType type, bool isDelete = false)
        {
            var originalType = context.SpaceTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                if (!isDelete)
                {
                    originalType.Description = type.Description;
                    originalType.DisplayOrder = type.DisplayOrder;
                    originalType.Name = type.Name;
                }

                originalType.MarkForDelete = isDelete;
            }

            context.SubmitChanges();
        }

        private void InsertEquipType(EquipType type)
        {
            context.EquipTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        private void UpdateEquipType(EquipType type, bool isDelete = false)
        {
            var originalType = context.EquipTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                if (!isDelete)
                {
                    originalType.Description = type.Description;
                    originalType.DisplayOrder = type.DisplayOrder;
                    originalType.Name = type.Name;
                }

                originalType.MarkForDelete = isDelete;
            }

            context.SubmitChanges();
        }

        private void InsertOtherType(OtherType type)
        {
            context.OtherTypes.InsertOnSubmit(type);
            context.SubmitChanges();
        }

        private void UpdateOtherType(OtherType type, bool isDelete = false)
        {
            var originalType = context.OtherTypes.FirstOrDefault(item => item.Id == type.Id);
            if (originalType != null)
            {
                if (!isDelete)
                {
                    originalType.Description = type.Description;
                    originalType.DisplayOrder = type.DisplayOrder;
                    originalType.Name = type.Name;
                }

                originalType.MarkForDelete = isDelete;
            }

            context.SubmitChanges();
        }

        #endregion
    }
}
