
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Witbird.SHTS.BLL.Managers;
using Witbird.SHTS.BLL.Services;
using Witbird.SHTS.Common;
using Witbird.SHTS.Web.Areas.Admin.Authorize;
using Witbird.SHTS.Web.Areas.Admin.Models;
using Witbird.SHTS.Web.Areas.Admin.Models.Resource;

namespace Witbird.SHTS.Web.Areas.Admin.Controllers
{
    [Permission(EnumRole.Editer)]
    public class ResourceController : AdminBaseController
    {
        ResourceManager resourceManager = new ResourceManager();
        ResourceService resourceService = new ResourceService();
        MiscManager miscManager = new MiscManager();

        #region 列表页
        /// <summary>
        /// Space
        /// </summary>
        /// <returns></returns>
        public ActionResult SpaceList(int id = 1)
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 1;

            query.PageIndex = id - 1;
            query.PageSize = 15;

            var result = resourceService.GetResourceByFilter(query);

            return View("ResourceList", result);
        }

        public ActionResult ActorList(int id = 1)
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 2;

            query.PageIndex = id - 1;
            query.PageSize = 15;

            var result = resourceService.GetResourceByFilter(query);

            return View("ResourceList", result);
        }

        public ActionResult EquipmentList(int id = 1)
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 3;

            query.PageIndex = id - 1;
            query.PageSize = 15;

            var result = resourceService.GetResourceByFilter(query);

            return View("ResourceList", result);
        }

        public ActionResult OtherList(int id = 1)
        {
            var query = new Witbird.SHTS.Model.Extensions.QueryResource();
            query.ResourceType = 4;

            query.PageIndex = id - 1;
            query.PageSize = 15;

            var result = resourceService.GetResourceByFilter(query);

            return View("ResourceList", result);
        }
        #endregion

        #region 详情页
        public ActionResult Show(int id)
        {
            var space = resourceManager.GetResourceById(id);

            if (space == null)
            {
                return RedirectToAction("SpaceList");
            }
            else
            {
                return View(space);
            }
        }
        #endregion

        #region 设置资源状态
        /// <summary>
        /// 删除所选择的资源
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        private ActionResult DeleteResources(string ids)
        {
            AjaxResponse result = new AjaxResponse();

            try
            {
                if (string.IsNullOrEmpty(ids))
                {
                    result.Status = -1;
                    result.Message = "请选择要删除的资源";
                }
                else
                {
                    List<int> idtodelete = new List<int>();
                    List<string> idslist = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    int temp = 0;
                    foreach (var id in idslist)
                    {
                        if (int.TryParse(id, out temp) && temp != 0)
                        {
                            idtodelete.Add(temp);
                        }
                    }
                    if (idtodelete.Count == 0)
                    {
                        result.Status = -1;
                        result.Message = "请选择要删除的资源";
                    }
                    else
                    {
                        resourceManager.DeleteResources(idtodelete);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = -1;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 删除指定资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteResourceById(int id)
        {
            AjaxResponse result = new AjaxResponse();

            try
            {
                if (id <= 0)
                {
                    result.Status = -1;
                    result.Message = "不存在该资源";
                }
                else
                {
                    resourceManager.DeleteResourceById(id);
                    result.Status = 0;
                }
            }
            catch (Exception ex)
            {
                result.Status = -1;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 审核所选择的资源
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        private ActionResult ApproResources(string ids)
        {
            AjaxResponse result = new AjaxResponse();

            try
            {
                if (string.IsNullOrEmpty(ids))
                {
                    result.Status = -1;
                    result.Message = "请选择要审核的资源";
                }
                else
                {
                    List<int> idtodelete = new List<int>();
                    List<string> idslist = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    int temp = 0;
                    foreach (var id in idslist)
                    {
                        if (int.TryParse(id, out temp) && temp != 0)
                        {
                            idtodelete.Add(temp);
                        }
                    }
                    if (idtodelete.Count == 0)
                    {
                        result.Status = -1;
                        result.Message = "请选择要审核的资源";
                    }
                    else
                    {
                        resourceManager.ApproResources(idtodelete);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Status = -1;
                result.Message = ex.Message;
            }

            return Json(result);
        }

        /// <summary>
        /// 审核指定资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ApproResourceById(int id)
        {
            AjaxResponse result = new AjaxResponse();

            try
            {
                if (id <= 0)
                {
                    result.Status = -1;
                    result.Message = "不存在该资源";
                }
                else
                {
                    resourceManager.ApproResourceById(id);
                }
            }
            catch (Exception ex)
            {
                result.Status = -1;
                result.Message = ex.Message;
            }

            return Json(result);
        }
        #endregion

        #region 资源类型设置

        public ActionResult Manage(string resourceTypeKey="", int id=1)
        {
            ResourceMiscModel model = new ResourceMiscModel();

            var totalCount = 0;
            int pageSize = 15;
            model.ResourceTypes.AddRange(miscManager.GetResourceTypes(resourceTypeKey, id, pageSize, out totalCount));

            model.ResourceTypeKey = resourceTypeKey;
            model.PageIndex = id;//当前页数
            model.PageSize = 15;//每页显示多少条
            model.PageStep = 10;//每页显示多少页码
            model.AllCount = totalCount;//总条数
            if (model.AllCount % model.PageSize == 0)
            {
                model.PageCount = model.AllCount / model.PageSize;
            }
            else
            {
                model.PageCount = model.AllCount / model.PageSize + 1;
            }

            return View(model);
        }
        
        [HttpPost]
        public ActionResult CreateOrUpdateType(string resourceTypeKey, string name, string description, bool markForDelete, int displayOrder = 99, int typeId = -1)
        {
            AjaxResponse response = new AjaxResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    response.Status = -1;
                    response.Message = "请填写类型名称";
                }
                else
                {
                    miscManager.CreateOrUpdateResourceType(resourceTypeKey, typeId, name, description, displayOrder);
                }
            }
            catch (Exception ex)
            {
                response.Status = -1;
                response.Message = "创建或更新资源类型失败";
                LogService.Log("创建或更新资源类型失败", ex.ToString());
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
