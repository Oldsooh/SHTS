using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;
using WitBird.Common;

namespace Witbird.SHTS.BLL.Managers
{
    public class DemandManager
    {
        private DemandRepository demandRepository;
        private DemandCategoryRepository demandCategoryRepository;

        public DemandManager()
        {
            demandRepository = new DemandRepository();
            demandCategoryRepository = new DemandCategoryRepository();
        }

        public Demand GetDemandById(int id, bool isFilterSensitivewords = true)
        {
            Demand result = null;
            try
            {
                result = demandRepository.FindOne(o => o.Id == id);
                if (result != null && isFilterSensitivewords)
                {
                    result.Title = FilterHelper.Filter(result.Title, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    result.ContentStyle = FilterHelper.Filter(result.ContentStyle, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    result.ContentText = FilterHelper.Filter(result.ContentText, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                    result.Description = FilterHelper.Filter(result.Description, CommonService.Sensitivewords, CommonService.ReplacementForSensitiveWords);
                }
            }
            catch (Exception e)
            {
                LogService.Log("编辑供求失败", e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 添加供求
        /// </summary>
        public bool AddDemand(Demand demand)
        {
            bool result = false;
            try
            {
                if (demand != null)
                {
                    result = demandRepository.AddEntitySave(demand);
                }
            }
            catch (Exception e)
            {
                LogService.Log("添加供求失败", e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 发布需求
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="resourceType"></param>
        /// <param name="resourceSubTypeId"></param>
        /// <param name="title"></param>
        /// <param name="contentStyle"></param>
        /// <param name="provinceId"></param>
        /// <param name="cityId"></param>
        /// <param name="areaId"></param>
        /// <param name="address"></param>
        /// <param name="phone"></param>
        /// <param name="qqweixin"></param>
        /// <param name="email"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="timeLength"></param>
        /// <param name="peopleNumber"></param>
        /// <param name="budget"></param>
        /// <param name="weixinBuyDemandFee"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool AddDemand(int userId, int resourceType, int resourceSubTypeId, string title, string contentStyle,
            string provinceId, string cityId, string areaId, string address, string phone, string qqweixin, string email,
            string startTime, string endTime, string timeLength, string peopleNumber, int budget, int weixinBuyDemandFee, string imageUrls, out string errorMessage, out int demandId)
        {
            bool isAddSuccessfully = false;
            demandId = -1;
            errorMessage = string.Empty;

            try
            {
                DateTime demandStartTime = DateTime.MinValue;
                DateTime demandEndTime = DateTime.MinValue;

                //if (resourceType < 0 || resourceSubTypeId < 0)
                //{
                //    errorMessage = "请选择具体类别与类型";
                //}
                //else if (string.IsNullOrWhiteSpace(title))
                //{
                //    errorMessage = "请填写需求标题！如寻找某场地、设备等资源";
                //}
                //else if (string.IsNullOrWhiteSpace(provinceId) || string.IsNullOrWhiteSpace(cityId))
                //{
                //    errorMessage = "请选择需求地点！包括省份、城市及区县信息！";
                //}
                //else if (string.IsNullOrWhiteSpace(startTime) || string.IsNullOrWhiteSpace(endTime) ||
                //    !DateTime.TryParse(startTime, out demandStartTime) || !DateTime.TryParse(endTime, out demandEndTime) ||
                //    demandStartTime < DateTime.Now.Date || demandEndTime < demandStartTime)
                //{
                //    errorMessage = "请填写正确需求开始与结束时间！";
                //}
                //else if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(qqweixin))
                //{
                //    errorMessage = "为了保证资源提供商能及时联系到您，请填写电话号码或者微信！";
                //}
                //else if (budget < 0)
                //{
                //    errorMessage = "请填写正确的预算金额！填写0表示面议！";
                //}
                //else if (string.IsNullOrWhiteSpace(contentStyle))
                //{
                //    errorMessage = "请填写需求内容！请直接出现电话、邮箱，以及微信联系方式等信息，否则将被系统自动屏蔽！";
                //}
                //else
                //{
                Demand demand = new Demand();
                demand.UserId = userId;
                demand.ResourceType = resourceType;
                demand.ResourceTypeId = resourceSubTypeId;
                demand.Title = title;
                demand.Description = string.Empty;
                demand.ContentStyle = contentStyle;
                demand.ContentText = Witbird.SHTS.Common.Html.HtmlUtil.RemoveHTMLTags(demand.ContentStyle);
                demand.Province = provinceId;
                demand.City = cityId;
                demand.Area = areaId;
                demand.Address = address;
                demand.Phone = phone;
                demand.QQWeixin = qqweixin;
                demand.Email = email;
                demand.StartTime = demandStartTime;
                demand.EndTime = demandEndTime;
                demand.TimeLength = timeLength;
                demand.PeopleNumber = peopleNumber;
                demand.Budget = budget;
                demand.IsActive = true;
                demand.ViewCount = 0;
                demand.InsertTime = DateTime.Now;
                demand.Status = (int)DemandStatus.InProgress;
                demand.WeixinBuyFee = weixinBuyDemandFee;
                demand.ImageUrls = imageUrls;

                errorMessage = ValidateDemand(demand);
                if (string.IsNullOrWhiteSpace(errorMessage))
                {
                    isAddSuccessfully = demandRepository.AddEntitySave(demand);
                    
                    errorMessage = "success";
                    demandId = demand.Id;
                }
                else
                {
                    isAddSuccessfully = false;
                }
                //}
            }
            catch (Exception ex)
            {
                LogService.Log("发布需求失败", ex.ToString());
                errorMessage = "Ooops！发布需求的过程中产生了错误！请重新尝试！如频繁遇到此问题，请联系客户人员！";
                isAddSuccessfully = false;
            }

            return isAddSuccessfully;
        }

        /// <summary>
        /// 更新供求
        /// </summary>
        //public bool EditDemand(Demand demand)
        //{
        //    bool result = false;
        //    try
        //    {
        //        if (demand != null)
        //        {
        //            result = demandRepository.UpdateEntitySave(demand);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogService.Log("编辑供求失败", e.ToString());
        //    }
        //    return result;
        //}


        /// <summary>
        /// 根据ID查询供求类别
        /// </summary>
        /// <param name="id">类别Id</param>
        public DemandCategory GetDemandCategoryById(int id)
        {
            DemandCategory demandCategory = null;
            try
            {
                demandCategory = demandCategoryRepository.FindOne(c => c.Id.Equals(id));
            }
            catch (Exception e)
            {
                LogService.Log("查询单个供求类别失败", e.ToString());
            }
            return demandCategory;
        }

        /// <summary>
        /// 获取供求类别列表
        /// </summary>
        public List<DemandCategory> GetDemandCategories()
        {
            List<DemandCategory> result = null;
            try
            {
                result = demandCategoryRepository.FindAll().OrderBy(d => d.DisplayOrder).ToList();
            }
            catch (Exception e)
            {
                LogService.Log("查询供求类别列表失败", e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 添加供求类别
        /// </summary>
        public bool AddCategory(DemandCategory demandCategory)
        {
            bool result = false;
            try
            {
                if (demandCategory != null)
                {
                    result = demandCategoryRepository.AddEntitySave(demandCategory);
                }
            }
            catch (Exception e)
            {
                LogService.Log("添加供求类别失败", e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 更新供求类别
        /// </summary>
        public bool EditCategory(DemandCategory demandCategory)
        {
            bool result = false;
            try
            {
                if (demandCategory != null)
                {
                    result = demandCategoryRepository.UpdateEntitySave(demandCategory);
                }
            }
            catch (Exception e)
            {
                LogService.Log("编辑供求类别失败", e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 删除供求类别
        /// </summary>
        public bool DeleteCategory(DemandCategory demandCategory)
        {
            bool result = false;
            try
            {
                if (demandCategory != null)
                {
                    result = demandCategoryRepository.DeleteEntitySave(demandCategory);
                }
            }
            catch (Exception e)
            {
                LogService.Log("删除供求类别失败", e.ToString());
            }
            return result;
        }

        /// <summary>
        /// 根据时间获取需求
        /// </summary>
        /// <param name="createTime"></param>
        /// <returns></returns>
        public List<Demand> QueryDemandsByTime(DateTime createTime)
        {
            List<Demand> result = null;
            try
            {
                result = demandRepository.FindAll(
                    d => d.InsertTime >= createTime && d.IsActive, v => v.InsertTime, true).ToList();
            }
            catch (Exception e)
            {
                LogService.Log("删除供求类别失败", e.ToString());
            }
            return result;
        }

        public string ValidateDemand(Demand demand)
        {
            string errorMessage = "";

            if (demand == null)
            {
                errorMessage = "需求内容不能为空";
            }
            else if (demand.ResourceType < 0 || demand.ResourceTypeId < 0)
            {
                errorMessage = "请选择具体类别与类型";
            }
            else if (string.IsNullOrWhiteSpace(demand.Title))
            {
                errorMessage = "请填写需求标题！如寻找某场地、设备等资源";
            }
            else if (string.IsNullOrWhiteSpace(demand.Province) || string.IsNullOrWhiteSpace(demand.City))
            {
                errorMessage = "请选择需求地点！包括省份、城市及区县信息！";
            }
            else if (demand.StartTime < DateTime.Now.Date || demand.EndTime < demand.StartTime)
            {
                errorMessage = "请填写正确需求开始与结束时间！";
            }
            else if (string.IsNullOrWhiteSpace(demand.PeopleNumber) && string.IsNullOrWhiteSpace(demand.QQWeixin))
            {
                errorMessage = "为了保证资源提供商能及时联系到您，请填写电话号码或者微信！";
            }
            else if (demand.Budget < 0)
            {
                errorMessage = "请填写正确的预算金额！填写0表示面议！";
            }
            else if (string.IsNullOrWhiteSpace(demand.ContentStyle))
            {
                errorMessage = "请填写需求内容！请直接出现电话、邮箱，以及微信联系方式等信息，否则将被系统自动屏蔽！";
            }
            else
            {

            }

            return errorMessage;
        }
    }
}
