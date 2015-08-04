using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class BaseModel
    {
        public string Title { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 当前页数
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页显示条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 每页显示页码数
        /// </summary>
        public int PageStep { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        public int AllCount { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 区、县、商圈
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public List<City> Provinces { get; set; }

        /// <summary>
        /// 一级城市
        /// </summary>
        public List<City> Cities { get; set; }

        /// <summary>
        /// 二级城市、区域、商圈
        /// </summary>
        public List<City> Areas { get; set; }
    }
}