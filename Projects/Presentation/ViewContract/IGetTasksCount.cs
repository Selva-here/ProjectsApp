using Projects.Core.AppEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Presentation.ViewContract
{
    internal interface IGetTasksCount:IView
    {
        void LoadTasksCount(TasksType tasksType,int count);
    }
}
