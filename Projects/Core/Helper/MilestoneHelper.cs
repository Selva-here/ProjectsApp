using Projects.Core.AppEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Core.Helper
{
    public static class MilestoneHelper
    {

        public static string ConvertMilestoneStatusToString(MilestoneStatus status)
        {
            switch (status)
            {
                case MilestoneStatus.InProgress:
                    return "In Progress";
                case MilestoneStatus.Reopen:
                    return "Re Open";
                case MilestoneStatus.OnHold:
                    return "On Hold";
                default:
                    return status.ToString();

            }
        }
    }
}
