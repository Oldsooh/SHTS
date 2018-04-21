using System;
using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.New;
using WitBird.Common;

namespace Witbird.SHTS.BLL.Managers
{
    public class ResourceManager
    {
        LinqToShtsDataContext context = new LinqToShtsDataContext(DBHelper.GetSqlConnectionString);

        #region 添加资源
        public void CreateResource(Resource resource)
        {
            resource.State = 1;
            context.Resources.InsertOnSubmit(resource);

            context.SubmitChanges();
        }
        #endregion

        #region 修改资源
        public void EditResource(Resource resource)
        {
            var res = context.Resources.SingleOrDefault(v => v.Id == resource.Id);
            if (res != null)
            {
                res.CanFriendlyLink = resource.CanFriendlyLink;
                res.ActorTypeId = resource.ActorTypeId;
                res.EquipTypeId = resource.EquipTypeId;
                res.OtherTypeId = resource.OtherTypeId;
                res.ResourceType = resource.ResourceType;
                res.SpaceFacilityValue = resource.SpaceFacilityValue;
                res.SpaceFeatureValue = resource.SpaceFeatureValue;
                res.SpacePeopleId = resource.SpacePeopleId;
                res.SpaceSizeId = resource.SpaceSizeId;
                res.SpaceTreat = resource.SpaceTreat;
                res.SpaceTypeId = resource.SpaceTypeId;
                res.State = 1;
                res.AreaId = resource.AreaId;
                res.CityId = resource.CityId;
                res.Contract = resource.Contract;
                res.Description = resource.Description;
                res.DetailAddress = resource.DetailAddress;
                res.Email = resource.Email;
                res.Href = resource.Href;
                res.ImageUrls = resource.ImageUrls;
                res.Mobile = resource.Mobile;
                res.ProvinceId = resource.ProvinceId;
                res.QQ = resource.QQ;
                res.ShortDesc = resource.ShortDesc;
                res.Telephone = resource.Telephone;
                res.Title = resource.Title;
                res.WeChat = resource.WeChat;
                res.LastUpdatedTime = DateTime.Now;
                res.EndDate = resource.EndDate;
                res.StartDate = resource.StartDate;
                res.Budget = resource.Budget;
            }
            context.SubmitChanges();
        }

        #endregion

        #region 获取资源列表
        ///// <summary>
        ///// 前台根据筛选项目获取审核通过了的资源
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //public QueryResourceResult GetResourceByFilter(QueryResource query)
        //{
        //    QueryResourceResult result = new QueryResourceResult();
        //    result.ResourceType = query.ResourceType;

        //    int totalCount = context.Resources.Count(e =>
        //        (query.State == 0 || e.State == query.State) &&

        //        (string.IsNullOrEmpty(query.CityId) || e.CityId.Equals(query.CityId)) &&

        //        e.ResourceType == query.ResourceType &&

        //        (query.SpaceType == 0 || e.SpaceTypeId == query.SpaceType) &&
        //        (query.SpaceFeature == 0 || (e.SpaceFeatureValue & query.SpaceFeature) > 0) &&
        //        (query.SpaceFacility == 0 || (e.SpaceFacilityValue & query.SpaceFacility) > 0) &&
        //        (query.SpaceSizeId == 0 || e.SpaceSizeId == query.SpaceSizeId) &&
        //        (query.SpacePeopleId == 0 || e.SpacePeopleId == query.SpacePeopleId) &&
        //        (query.SpaceTreat == 0 ? true : e.SpaceTreat == query.SpaceTreat) &&

        //        (query.ActorTypeId == 0 || e.ActorTypeId == query.ActorTypeId) &&

        //        (query.EquipTypeId == 0 || e.EquipTypeId == query.EquipTypeId) &&

        //        (query.OtherTypeId == 0 || e.OtherTypeId == query.OtherTypeId)
        //        );

        //    var list = (from e in context.Resources
        //                where e.State == 2 &&

        //                (string.IsNullOrEmpty(query.CityId) || e.CityId.Equals(query.CityId)) &&

        //                e.ResourceType == query.ResourceType &&

        //                (query.SpaceType == 0 || e.SpaceTypeId == query.SpaceType) &&
        //                (query.SpaceFeature == 0 || (e.SpaceFeatureValue & query.SpaceFeature) > 0) &&
        //                (query.SpaceFacility == 0 || (e.SpaceFacilityValue & query.SpaceFacility) > 0) &&
        //                (query.SpaceSizeId == 0 || e.SpaceSizeId == query.SpaceSizeId) &&
        //                (query.SpacePeopleId == 0 || e.SpacePeopleId == query.SpacePeopleId) &&
        //                (query.SpaceTreat == 0 ? true : e.SpaceTreat == query.SpaceTreat) &&

        //                (query.ActorTypeId == 0 || e.ActorTypeId == query.ActorTypeId) &&

        //                (query.EquipTypeId == 0 || e.EquipTypeId == query.EquipTypeId) &&

        //                (query.OtherTypeId == 0 || e.OtherTypeId == query.OtherTypeId)

        //                orderby e.LastUpdatedTime, e.ReadCount

        //                select e)
        //        .Skip(query.PageSize * query.PageIndex)
        //        .Take(query.PageSize)
        //        .ToList();

        //    result.TotalCount = totalCount;
        //    result.Items = list;
        //    result.Paging = new Paging
        //    {
        //        PageCount = result.TotalCount / query.PageSize + (result.TotalCount % query.PageSize == 0 ? 0 : 1),
        //        PageStep = 10,
        //        PageIndex = query.PageIndex + 1
        //    };

        //    switch (query.ResourceType)
        //    {
        //        case 1:
        //            result.Paging.ActionName = "spacelist";
        //            break;
        //        case 2:
        //            result.Paging.ActionName = "actorlist";
        //            break;
        //        case 3:
        //            result.Paging.ActionName = "equipmentist";
        //            break;
        //        case 4:
        //            result.Paging.ActionName = "otherlist";
        //            break;
        //        default:
        //            break;
        //    }

        //    return result;
        //}

        #endregion

        #region 根据Id查找资源信息
        public Resource GetResourceById(int id, bool filterSensitiveWords = true)
        {
            var resource = context.Resources.SingleOrDefault(v => v.Id == id && v.State != 3);
            if (resource != null)
            {
                resource.CommentList = context.Comments.Where(v => v.ResourceId == id)
                    .OrderByDescending(v => v.CreateTime).Take(20).ToList();

                if (filterSensitiveWords)
                {
                    resource.ShortDesc = FilterHelper.Filter(resource.ShortDesc, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    resource.Description = FilterHelper.Filter(resource.Description, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);

                    if (resource.CommentList != null)
                    {
                        foreach (var item in resource.CommentList)
                        {
                            item.Content = FilterHelper.Filter(item.Content, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                        }
                    }
                }
            }
            return resource;
        }
        #endregion

        #region 根据Id查找资源信息，包含点评

        #endregion

        #region 根据Id删除资源
        public void DeleteResourceById(int id)
        {
            var space = context.Resources.SingleOrDefault(v => v.Id == id);
            if (space != null)
            {
                space.State = (int)Witbird.SHTS.Model.ResourceState.Deleted;
            }
            context.SubmitChanges();
        }
        #endregion

        #region 根据Id将资源设置为审核通过
        public void ApproResourceById(int id)
        {
            var space = context.Resources.SingleOrDefault(v => v.Id == id);
            if (space != null)
            {
                space.State = (int)Witbird.SHTS.Model.ResourceState.Approved;
            }
            context.SubmitChanges();
        }
        #endregion

        /// <summary>
        /// 置顶资源
        /// </summary>
        /// <param name="id"></param>
        public void ClickResourceById(int id)
        {
            var space = context.Resources.SingleOrDefault(v => v.Id == id);
            if (space != null && space.ClickTime < DateTime.Now.Date)
            {
                space.ClickTime = DateTime.Now;
                context.SubmitChanges();
            }
        }

        //public QueryUserResourceResult GetUserResource(QueryUserResource query)
        //{
        //    //    int totalCount = context.Resources.Where(v => v.UserId == query.UserId).Count();

        //    //    var list = context.Resources.Where(v => v.UserId == query.UserId)
        //    //        .OrderByDescending(v => v.LastUpdatedTime)
        //    //        .Skip(query.PageSize * query.PageIndex)
        //    //        .Take(query.PageSize)
        //    //        .ToList();

        //    //    result.TotalCount = totalCount;
        //    //    result.Items = list;
        //    //    result.Paging = new Paging
        //    //    {
        //    //        PageCount = result.TotalCount / query.PageSize + (result.TotalCount % query.PageSize == 0 ? 0 : 1),
        //    //        PageStep = 10,
        //    //        PageIndex = query.PageIndex + 1,
        //    //        ActionName = "my"
        //    //    };
        //}

        #region 不分类的资源


        public List<Resource> GetResourcesByTime(DateTime lastUpdatedTime)
        {
            var list = context.Resources
                .Where(v => v.LastUpdatedTime >= lastUpdatedTime)
                .Where(v => v.State != 3)
                .OrderByDescending(v => v.LastUpdatedTime)
                .ToList();

            return list;
        }

        public void CommentOnResource(int resourceId, int userId, string content)
        {
            Comment comment = new Comment
            {
                ResourceId = resourceId,
                UserId = userId,
                Content = content,
                MarkForDelete = false,
                CreateTime = DateTime.Now
            };

            context.Comments.InsertOnSubmit(comment);

            context.SubmitChanges();
        }
        #endregion

        #region 批量操作
        /// <summary>
        /// 批量删除资源
        /// </summary>
        /// <param name="ids"></param>
        public void DeleteResources(List<int> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                throw new ArgumentNullException("idList", "没有输入要删除的资源的id");
            }

            var resToDelete = context.Resources.Where(v => idList.Contains(v.Id));
            if (resToDelete.Count() > 0)
            {
                foreach (var item in resToDelete)
                {
                    item.State = (int)Witbird.SHTS.Model.ResourceState.Deleted;
                }

                context.SubmitChanges();
            }
        }

        /// <summary>
        /// 批量审核资源
        /// </summary>
        /// <param name="ids"></param>
        public void ApproResources(List<int> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                throw new ArgumentNullException("idList", "没有输入要删除的资源的id");
            }

            var resToDelete = context.Resources.Where(v => idList.Contains(v.Id));
            if (resToDelete.Count() > 0)
            {
                foreach (var item in resToDelete)
                {
                    item.State = (int)Witbird.SHTS.Model.ResourceState.Approved;
                }

                context.SubmitChanges();
            }
        }
        #endregion
    }
}