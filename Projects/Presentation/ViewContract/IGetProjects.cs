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
    public interface IGetProjects:IView
    {
        void LoadProject(ProjectObj project);
        void AutoSuggestionBoxProjectSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ProjectObj> projects);
        void LoadProjects(ProjectsType projectsType, IEnumerable<ProjectObj> projects); 
    }
}
