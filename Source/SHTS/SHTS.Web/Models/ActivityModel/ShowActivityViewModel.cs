using System.Collections.Generic;
using System.Linq;
using Witbird.SHTS.Model;

namespace Witbird.SHTS.Web.Models.ActivityModel
{
    public class ShowActivityViewModel
    {
        public const string SPLINT = "|";

        public Activity Activity { get; set; }

        public List<Activity> ActivityList { set; get; }

        public List<Activity> Slides
        {
            get
            {
                if (ActivityList != null && ActivityList.Count > 0)
                {
                    return ActivityList.Take(5).ToList();
                }
                else
                {
                    return null;
                }
            }
        }

        public List<ActivityType> ActivityTypes { get; set; }

        public List<Activity> PreTypeActivityList { set; get; }

        public string GetTypeName(string typeid)
        {
            const string defaultType = "活动";
            string typeName = defaultType;
            if (ActivityTypes != null && ActivityTypes.Count > 0)
            {
                var activityType = ActivityTypes.Find(t => t.ActivityTypeId.ToString() == typeid);
                if (activityType != null)
                {
                    typeName = activityType.ActivityTypeName;
                }
            }
            return typeName;
        }

        public int ActivityVoteTotalCount { get; set; }

        public bool IsCurrentUserVoted { get; set; }
    }
}