using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Core.Entity
{
    public interface IWorkItem
    {
        int OwnerID { get; set; }
        int CompletedPercentage { get; set; }
        DateTime StartDate { get; set; }

        DateTime EndDate { get; set; }

    }
}
