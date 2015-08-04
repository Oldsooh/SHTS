using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Common;
using Witbird.SHTS.DAL;
using Witbird.SHTS.DAL.Daos;
using Witbird.SHTS.Model.Extensions;

namespace Witbird.SHTS.BLL.Services
{
    public class ResourceService
    {
        private ResourceDao resourceDao;

        public ResourceService()
        {
            resourceDao = new ResourceDao();
        }

        /// <summary>
        /// 前台根据筛选项目获取审核通过了的资源
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public QueryResourceResult GetResourceByFilter(QueryResource query)
        {
            QueryResourceResult result = new QueryResourceResult();
            var sqlConn = DBHelper.GetSqlConnection();
            try
            {
                sqlConn.Open();
                result = resourceDao.GetResourceByFilter(
                    sqlConn,
                    query.State,
                    query.CityId,
                    query.AreaId,
                    query.ResourceType,
                    query.SpaceType,
                    query.SpaceFeature,
                    query.SpaceFacility,
                    query.SpaceSizeId,
                    query.SpacePeopleId,
                    query.SpaceTreat,
                    query.ActorTypeId,
                    query.EquipTypeId,
                    query.OtherTypeId,
                    query.PageIndex,
                    query.PageSize,
                    query.ActorFromId,
                    query.ActorSex);
            }
            catch (Exception ex)
            {
                LogService.Log("ResourceService.GetResourceByFilter", ex.ToString());
            }
            finally
            {
                sqlConn.Close();
            }

            return result;
        }

        public QueryResourceResult GetResourceByUser(int userId, int pageIndex, int pageSize)
        {
            QueryResourceResult result = new QueryResourceResult();
            var sqlConn = DBHelper.GetSqlConnection();
            try
            {
                sqlConn.Open();
                result = resourceDao.GetUserResource(sqlConn, userId, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                LogService.Log("ResourceService.GetResourceByFilter", ex.ToString());
            }
            finally
            {
                sqlConn.Close();
            }

            return result;
        }
    }
}
