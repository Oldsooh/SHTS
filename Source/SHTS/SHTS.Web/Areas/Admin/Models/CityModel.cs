using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Areas.Admin.Models
{
    public class CityModel : BaseModel
    {
        /// <summary>
        /// 省份
        /// </summary>
        public List<City> Provices { get; set; }

        /// <summary>
        /// 城市
        /// </summary>
        public List<City> Cities { get; set; }

        /// <summary>
        /// 地区（商圈）
        /// </summary>
        public List<City> Areas { get; set; }

        /// <summary>
        /// 添加城市页下拉框城市
        /// </summary>
        public List<City> SelectCities { get; set; }

        /// <summary>
        /// 当前选中城市Id
        /// </summary>
        private string ProvinceId { get; set; }
    }
}