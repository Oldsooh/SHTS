using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return geos[0];
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
        /// 不要用，用前先手动复制.Richard
        /// </summary>
        public bool IsVip { get; set; }
        /// <summary>
        /// 不要用，用前先手动复制.Richard
        /// </summary>
        public bool IsIdentified { get; set; }
    }
}
