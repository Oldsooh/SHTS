using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Witbird.SHTS.Model;
using Witbird.SHTS.Web.Public;

namespace Witbird.SHTS.Web.Models
{
    public class DemandModel : BaseModel
    {
        public Demand Demand { get; set; }

        public List<Demand> Demands { get; set; }

        //public DemandCategory DemandCategory { get; set; }

        //public List<DemandCategory> DemandCategories { get; set; }

        public string LastResourceType { get; set; }
        public string ResourceType { get; set; }
        public string ResourceTypeId { get; set; }

        //public string StartBudget { get; set; }

        //public string EndBudget { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string BudgetCondition { get; set; }

        public string BudgetConditionDisplayName
        {
            get
            {
                BudgetCondition = BudgetCondition ?? string.Empty;

                var matchedCondition = MiscData.BudgetFilters.FirstOrDefault(item =>
                    item.Condition.Trim().Equals(BudgetCondition.Trim(), StringComparison.CurrentCultureIgnoreCase));
                return matchedCondition != null ? matchedCondition.DisplayName : "预算金额";
            }
        }

        private Dictionary<string, string> _routeFilters = new Dictionary<string, string>();

        public string GetRouteFilters(params string [] execludeRoutes)
        {
            _routeFilters.Clear();

            if (!string.IsNullOrWhiteSpace(ResourceType))
            {
                _routeFilters.Add(nameof(ResourceType), ResourceType);
            }
            if (!string.IsNullOrWhiteSpace(LastResourceType))
            {
                _routeFilters.Add(nameof(LastResourceType), LastResourceType);
            }
            if (!string.IsNullOrWhiteSpace(ResourceTypeId))
            {
                _routeFilters.Add(nameof(ResourceTypeId), ResourceTypeId);
            }
            if (!string.IsNullOrWhiteSpace(City))
            {
                _routeFilters.Add(nameof(City), City);
            }
            if (!string.IsNullOrWhiteSpace(Area))
            {
                _routeFilters.Add(nameof(Area), Area);
            }
            if (!string.IsNullOrWhiteSpace(BudgetCondition))
            {
                _routeFilters.Add(nameof(BudgetCondition), BudgetCondition);
            }
            if (!string.IsNullOrWhiteSpace(StartTime))
            {
                _routeFilters.Add(nameof(StartTime), StartTime);
            }
            if (!string.IsNullOrWhiteSpace(EndTime))
            {
                _routeFilters.Add(nameof(EndTime), EndTime);
            }

            //_routeFilters.Add(nameof(StartBudget), StartBudget};
            //_routeFilters.Add(nameof(EndBudget), EndBudget};

            if (execludeRoutes != null && execludeRoutes.Length > 0)
            {
                foreach (var execludeKey in execludeRoutes)
                {
                    if (_routeFilters.ContainsKey(execludeKey))
                    {
                        _routeFilters.Remove(execludeKey);
                    }
                }
            }

            var routeBuilder = new StringBuilder();
            foreach (var item in _routeFilters)
            {
                routeBuilder.Append($"{item.Key}={item.Value}&");
            }
            return routeBuilder.ToString();
        }

        /// <summary>
        /// 是否为会员, 会员可能查询详情
        /// </summary>
        public bool IsMember { get; set; }

        /// <summary>
        /// 是否为VIP，VIP可以查询联系方式
        /// </summary>
        public bool IsVIP { get; set; }

        /// <summary>
        /// 场地类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "场地类型设置不正确")]
        public string SpaceTypeId { get; set; }

        /// <summary>
        /// 演员类别
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "演员类别设置不正确")]
        public string ActorTypeId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "设备类型设置不正确")]
        public string EquipTypeId { get; set; }
        
        /// <summary>
        /// “其他资源”的类型
        /// </summary>
        [RegularExpression(@"\d+", ErrorMessage = "此类型不存在")]
        public string OtherTypeId { get; set; }
    }
}