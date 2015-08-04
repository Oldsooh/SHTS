using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.BLL.Managers
{
    public class SinglePageManager
    {
        private SinglePageRepository singlePageRepository;

        public SinglePageManager()
        {
            singlePageRepository = new SinglePageRepository();
        }

        public List<SinglePage> GetSinglePages(int pageSize, int pageIndex, out int count, string entityType)
        {
            List<SinglePage> result = null;
            int tempCount = 0;
            try
            {
                result = singlePageRepository.FindPage(pageSize, pageIndex, out tempCount, s => s.EntityType == entityType && s.IsActive.Value, i => i.Id, false).ToList();
            }
            catch (Exception e)
            {
                LogService.Log("获取单页列表失败", e.ToString());
            }
            count = tempCount;

            return result;
        }

        public bool AddSinglePage(SinglePage singlePage)
        {
            bool result = false;
            try
            {
                if (singlePage != null)
                {
                    result = singlePageRepository.AddEntitySave(singlePage);
                }
            }
            catch (Exception e)
            {
                LogService.Log("添加单页失败", e.ToString());
            }
            return result;
        }

        //public bool EditSinglePage(SinglePage singlePage)
        //{
        //    bool result = false;
        //    try
        //    {
        //        if (singlePage != null)
        //        {
        //            result = singlePageRepository.UpdateEntitySave(singlePage);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        LogService.Log("编辑单页失败", e.ToString());
        //    }
        //    return result;
        //}

        public bool RemoveSinglePage(string id)
        {
            bool result = false;
            try
            {
                SinglePage singlePage = singlePageRepository.FindOne(s => s.Id.ToString() == id);
                if (singlePage != null)
                {
                    result = singlePageRepository.DeleteEntitySave(singlePage);
                }
            }
            catch (Exception e)
            {
                LogService.Log("编辑单页失败", e.ToString());
            }
            return result;
        }
    }
}
