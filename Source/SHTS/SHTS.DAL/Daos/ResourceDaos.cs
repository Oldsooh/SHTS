using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Witbird.SHTS.Model;
using Witbird.SHTS.Model.Extensions;
using Witbird.SHTS.Common;

namespace Witbird.SHTS.DAL.Daos
{
    public class ResourceDao
    {
        private const string sp_GetResourceByFilter = "sp_GetResourceByFilter";
        private const string sp_GetUserResource = "sp_GetUserResource";

        private const string col_state = "@state";
        private const string col_cityId = "@cityId";
        private const string col_areaId = "@areaId";
        private const string col_resourceType = "@resourceType";
        private const string col_spaceTypeId = "@spaceTypeId";
        private const string col_spaceFeature = "@spaceFeature";
        private const string col_spaceFacility = "@spaceFacility";
        private const string col_spaceSizeId = "@spaceSizeId";
        private const string col_spacePeopleId = "@spacePeopleId";
        private const string col_spaceTreat = "@spaceTreat";
        private const string col_actorTypeId = "@actorTypeId";
        private const string col_equipTypeId = "@equipTypeId";
        private const string col_otherTypeId = "@otherTypeId";
        private const string col_pageIndex = "@pageIndex";
        private const string col_pageSize = "@pageSize";
        private const string col_userId = "@userId";
        private const string col_actorFromId = "@actorFromId";
        private const string col_actorSex = "@actorSex";

        public QueryResourceResult GetResourceByFilter(
            SqlConnection sqlConn,
            int state,
            string cityId,
            string areaId,
            int resourceType,

            int spaceType,
            int spaceFeature,
            int spaceFacility,
            int spaceSizeId,
            int spacePeopleId,
            int spaceTreat,

            int actorTypeId,

            int equipTypeId,

            int otherTypeId,

            int pageindex,
            int pagesize,
            
            int actorFromId,
            int actorSex)//pageindex from 0
        {
            QueryResourceResult result = new QueryResourceResult();

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(col_state,state),
                new SqlParameter(col_cityId,cityId??(object)DBNull.Value),
                new SqlParameter(col_areaId,areaId??(object)DBNull.Value),
                new SqlParameter(col_resourceType,resourceType),
                new SqlParameter(col_spaceTypeId,spaceType),
                new SqlParameter(col_spaceFeature,spaceFeature),
                new SqlParameter(col_spaceFacility,spaceFacility),
                new SqlParameter(col_spaceSizeId,spaceSizeId),
                new SqlParameter(col_spacePeopleId,spacePeopleId),
                new SqlParameter(col_spaceTreat,spaceTreat),
                new SqlParameter(col_actorTypeId,actorTypeId),
                new SqlParameter(col_equipTypeId,equipTypeId),
                new SqlParameter(col_otherTypeId,otherTypeId),
                new SqlParameter(col_pageIndex,pageindex),
                new SqlParameter(col_pageSize,pagesize),
                new SqlParameter(col_actorFromId,actorFromId),
                new SqlParameter(col_actorSex,actorSex)
            };

            SqlDataReader reader = DBHelper.RunProcedure(sqlConn, sp_GetResourceByFilter, sqlParameters);

            result.Items = new List<Resource>();

            if (reader.Read())
            {
                result.TotalCount = reader["TotalCount"].DBToInt32();
            }
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    Resource resource = new Resource();
                    resource.Id = reader["Id"].DBToInt32();
                    resource.ResourceType = reader["ResourceType"].DBToInt32();
                    resource.Title = reader["Title"].DBToString();
                    resource.SpaceSizeId = reader["SpaceSizeId"].DBToInt32();
                    resource.ShortDesc = reader["ShortDesc"].DBToString();
                    resource.ProvinceId = reader["ProvinceId"].DBToString();
                    resource.CityId = reader["CityId"].DBToString();
                    resource.AreaId = reader["AreaId"].DBToString();
                    resource.CreateTime = reader["CreateTime"].DBToDateTime().Value;
                    resource.UserId = reader["UserId"].DBToInt32();
                    resource.Telephone = reader["Telephone"].DBToString();
                    resource.Mobile = reader["Mobile"].DBToString();
                    resource.ImageUrls = reader["ImageUrls"].DBToString();
                    resource.ActorTypeId = reader["ActorTypeId"].DBToInt32();
                    resource.ClickTime = reader["ClickTime"].DBToDateTime().Value;
                    resource.UserName = reader["UserName"].DBToString();
                    //还有一些字段没有添加

                    result.Items.Add(resource);
                }
            }

            if (reader != null)
            {
                reader.Close();
            }

            result.ResourceType = resourceType;

            result.Paging = new Paging
            {
                PageCount = result.TotalCount / pagesize + (result.TotalCount % pagesize == 0 ? 0 : 1),
                PageStep = 10,
                PageIndex = pageindex + 1
            };
            switch (resourceType)
            {
                case 1:
                    result.Paging.ActionName = "spacelist";
                    break;
                case 2:
                    result.Paging.ActionName = "actorlist";
                    break;
                case 3:
                    result.Paging.ActionName = "equipmentlist";
                    break;
                case 4:
                    result.Paging.ActionName = "otherlist";
                    break;
                default:
                    break;
            }

            return result;
        }

        public QueryResourceResult GetUserResource(SqlConnection sqlConn, int userId, int pageIndex, int pageSize)
        {
            QueryResourceResult result = new QueryResourceResult();

            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(col_userId,userId),
                new SqlParameter(col_pageIndex,pageIndex),
                new SqlParameter(col_pageSize,pageSize)
            };

            SqlDataReader reader = DBHelper.RunProcedure(sqlConn, sp_GetUserResource, sqlParameters);

            if (reader.Read())
            {
                result.TotalCount = reader["TotalCount"].DBToInt32();
            }
            if (reader.NextResult())
            {
                result.Items = new List<Resource>();
                while (reader.Read())
                {
                    Resource resource = new Resource();
                    resource.Id = reader["Id"].DBToInt32();
                    resource.Title = reader["Title"].DBToString();
                    resource.ResourceType = reader["ResourceType"].DBToInt32();
                    resource.SpaceSizeId = reader["SpaceSizeId"].DBToInt32();
                    resource.ShortDesc = reader["ShortDesc"].DBToString();
                    resource.ProvinceId = reader["ProvinceId"].DBToString();
                    resource.CityId = reader["CityId"].DBToString();
                    resource.AreaId = reader["AreaId"].DBToString();
                    resource.CreateTime = reader["CreateTime"].DBToDateTime().Value;
                    resource.UserId = reader["UserId"].DBToInt32();
                    resource.Telephone = reader["Telephone"].DBToString();
                    resource.Mobile = reader["Mobile"].DBToString();
                    resource.State = reader["State"].DBToInt32();
                    resource.ResourceType = reader["ResourceType"].DBToInt32();
                    resource.ClickTime = reader["ClickTime"].DBToDateTime().Value;
                    //还有一些字段没有添加

                    result.Items.Add(resource);
                }
            }
            if (reader != null)
            {
                reader.Close();
            }

            result.Paging = new Paging
            {
                PageCount = result.TotalCount / pageSize + (result.TotalCount % pageSize == 0 ? 0 : 1),
                PageStep = 10,
                PageIndex = pageIndex + 1
            };
            result.Paging.ActionName = "my";

            return result;
        }
    }
}
