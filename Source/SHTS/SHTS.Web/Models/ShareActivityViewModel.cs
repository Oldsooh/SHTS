using System.Collections.Generic;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models
{
    public class ShareActivityViewModel : BaseModel
    {
        public string ErrorMsg { set; get; }
        public string ErrorCode { set; get; }

        private Activity _Activity;
        public Activity Activity {
            get
            {
                if (_Activity == null)
                {
                    _Activity=new Activity();
                }
                return _Activity;
            }
            set { this._Activity = value; }
        }

        public List<ActivityType> ActivityTypes { get; set; }
    }
}