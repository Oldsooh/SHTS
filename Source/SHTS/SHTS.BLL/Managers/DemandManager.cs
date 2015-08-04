using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

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

        public Demand GetDemandById(int id)
        {
            Demand result = null;
            try
            {
                result = demandRepository.FindOne(o=>o.Id == id);
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
                    d=>d.InsertTime>=createTime&&d.IsActive,v=>v.InsertTime,true).ToList();
            }
            catch (Exception e)
            {
                LogService.Log("删除供求类别失败", e.ToString());
            }
            return result;
        }
    }
}
