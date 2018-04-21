namespace Witbird.SHTS.Model
{
    public partial class Resource
    {
        public string ResourceTypeName
        {
            get
            {
                var name = "不确定";

                switch (ResourceType)
                {
                    case 1:
                        name = "活动场地";
                        break;
                    case 2:
                        name = "演艺人员";
                        break;
                    case 3:
                        name = "活动设备";
                        break;
                    case 4:
                        name = "其他资源";
                        break;
                    default:
                        break;
                }

                return name;
            }
        }
    }
}
