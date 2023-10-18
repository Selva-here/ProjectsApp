using Projects.Core.AppEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using ZohoProjects;

namespace Projects.Core.Helper
{
    public static class ZTaskHelper
    {
        public static Brush PriorityBrush(Priority priority)
        {
            switch (priority)
            {
                case Priority.None:
                    return new SolidColorBrush(Colors.Gray);
                case Priority.Low:
                    return new SolidColorBrush(Colors.DarkGreen);
                case Priority.Medium:
                    return new SolidColorBrush(Colors.Blue);
                case Priority.High:
                    return new SolidColorBrush(Colors.Red);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public static string ConvertTaskStatusToString(ZTaskStatus status)
        {
            switch (status)
            {
                case ZTaskStatus.InProgress:
                    return "In Progress";
                case ZTaskStatus.InReview:
                    return "In Review";
                case ZTaskStatus.ToBeTested:
                    return "To Be Tested";
                case ZTaskStatus.OnHold:
                    return "On Hold";
                default:
                    return status.ToString();
            }

        }


    }
}
