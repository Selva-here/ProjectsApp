using Projects.Core.AppEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Presentation.ViewContract
{
    internal interface IGetMilestonesCount:IView
    {
        void LoadMilestoneCount(MilestonesType milestonesType,int count);
    }
}
