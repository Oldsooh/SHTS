using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class PublicConfigModel
    {
        private Dictionary<string, PublicConfig> multipleConfigs;
        public PublicConfigModel()
        {
            multipleConfigs = new Dictionary<string, PublicConfig>();
        }

        /// <summary>
        /// Gets or sets single config.
        /// </summary>
        public PublicConfig SingleConfig { get; set; }
        
        /// <summary>
        /// Gets multiple configs.
        /// </summary>
        public Dictionary<string, PublicConfig> MultipleConfigs
        {
            get
            {
                return multipleConfigs;
            }
        }
    }
}