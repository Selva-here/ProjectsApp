using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Core.AppEnum
{
    public enum MilestonesType
    {
        All,
        Active,
        Completed,
        Overdue,
        DueToday,
        DueThisWeek,
        DueThisMonth,

        MyActive,
        MyCompleted,
        CreatedByMe,

        Particular,
        NameSearch,
        ProjectNameSearch,
        Project,
        LastMilestone
    }
}
