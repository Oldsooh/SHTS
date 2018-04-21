using System;

namespace Witbird.SHTS.Model
{
    public partial class User
    {
        public const string SPLT = ",";

        private string[] geos;
        public string Province
        {
            get
            {
                if (geos == null || geos.Length == 0)
                {
                    if (!string.IsNullOrEmpty(LocationId))
                    {
                        geos = LocationId.Split(SPLT.ToCharArray());
                    }
                    else
                    {
                        geos = new string[1] { string.Empty };
                    }
                }

                var province = geos[0];

                if (province.Equals("shanghaishi", StringComparison.CurrentCultureIgnoreCase) ||
                    province.Equals("beijingshi", StringComparison.CurrentCultureIgnoreCase) ||
                    province.Equals("tianjinshi", StringComparison.CurrentCultureIgnoreCase) ||
                    province.Equals("chongqingshi", StringComparison.CurrentCultureIgnoreCase))
                {
                    province = "zhixiashi";
                }

                return province;
            }
        }

        public string City
        {
            get
            {
                if (geos == null || geos.Length == 0)
                {
                    if (!string.IsNullOrEmpty(LocationId))
                    {
                        geos = LocationId.Split(SPLT.ToCharArray());
                    }
                    else
                    {
                        geos = new string[1] { string.Empty };
                    }
                }
                return geos.Length >= 2 ? geos[1] : string.Empty;
            }
        }

        public string Area
        {
            get
            {
                if (geos == null || geos.Length == 0)
                {
                    if (!string.IsNullOrEmpty(LocationId))
                    {
                        geos = LocationId.Split(SPLT.ToCharArray());
                    }
                    else
                    {
                        geos = new string[1] { string.Empty };
                    }
                }
                return geos.Length >= 3 ? geos[2] : string.Empty;
            }
        }
        
        /// <summary>
        /// 是否是认证会员
        /// </summary>
        public bool IsIdentified
        {
            get
            {
                return (this != null && 
                    this.Vip.HasValue &&
                    (this.Vip.Value == (int)VipState.Identified || this.Vip.Value == (int)VipState.VIP));
            }
        }

        /// <summary>
        /// 是否是VIP会员
        /// </summary>
        public bool IsVip
        {
            get
            {
                return (this != null && 
                    this.Vip.HasValue && 
                    (this.Vip.Value == (int)VipState.VIP));
            }
        }
    }
}
