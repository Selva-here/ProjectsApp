using Projects.Core.EntityObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Presentation.ViewContract
{
    public interface IAddProject:IView
    {
        void ProjectAdded(ProjectObj project);
    }
}
