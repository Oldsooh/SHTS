using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Witbird.SHTS.Model
{
    public class Right
    {
        /// <summary>
        /// 右侧需求列表
        /// </summary>
        public List<Demand> Demands { get; set; }

        /// <summary>
        /// 右侧场地列表
        /// </summary>
        public List<Resource> Spaces { get; set; }

        /// <summary>
        /// 右侧演义人员
        /// </summary>
        public List<Resource> Actors { get; set; }

        /// <summary>
        /// 右侧设备列表
        /// </summary>
        public List<Resource> Equipments { get; set; }

        /// <summary>
        /// 右侧其它资源
        /// </summary>
        public List<Resource> Others { get; set; }
    }
}
