using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Projects.Presentation.ViewContract
{
    public interface IGetMilestones:IView
    {
        void LoadMilestone(MilestoneObj milestone);
        void AutoSuggestionBoxMilestoneSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<MilestoneObj> milestones);
        void LoadMilestones(MilestonesType milestonesType, IEnumerable<MilestoneObj> milestones);
    }
}
