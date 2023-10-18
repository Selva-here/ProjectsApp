using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Projects.Presentation.ViewContract
{
    public interface IGetTasks:IView
    {
        void LoadTask(ZTaskObj task);
        void LoadTasks(TasksType tasksType, IEnumerable<ZTaskObj> tasks);
    }
}
