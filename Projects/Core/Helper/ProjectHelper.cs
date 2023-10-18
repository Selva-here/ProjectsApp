using Projects.Core.AppEnum;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZohoProjects;

namespace Projects.Core.Helper
{
    public static class ProjectHelper
    {
        public static string ConvertProjectStatusToString(ProjectStatus status)
        {
           switch (status)
            {
                case ProjectStatus.InProgress:
                    return "In Progress";
                case ProjectStatus.OnTrack:
                    return "On Track";
                case ProjectStatus.InTesting:
                    return "In Testing";
                case ProjectStatus.OnHold:
                    return "On Hold";
               default:
                    return status.ToString();

            }
        }
    }
}
