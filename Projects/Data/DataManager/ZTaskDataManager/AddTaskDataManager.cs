using Projects.Core.EntityObj;
using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Domain.UseCase.ZTaskUseCase;
using Projects.Core.Entity;

namespace Projects.Data.DataManager.ZTaskDataManagerAddTask
{
    public class AddTaskDataManager : IAddTaskDataManger
    {
        ITaskDBHandler _TaskDBHandler;
        public AddTaskDataManager(ITaskDBHandler taskDBHandler)
        {
            this._TaskDBHandler = taskDBHandler;
        }

        public void AddTask(IAddTaskUseCaseRequest addTaskUseCaseRequest, IUseCaseCallBackBase<AddTaskUseCaseResponse> addTaskUseCaseCallBack)
        {

            _TaskDBHandler.AddTask(addTaskUseCaseRequest.Task);
            List<ZTaskObj> lastTask = _TaskDBHandler.GetTasks(Core.AppEnum.TasksType.LastTask,null);
            if (lastTask.Count < 0)
            {
                throw new Exception("Last Task Not Found");
            }
           addTaskUseCaseRequest.Task.ID = lastTask[0].ID;

            AddTaskUseCaseResponse addTaskUseCaseResponseUseCaseResponse = new AddTaskUseCaseResponse(addTaskUseCaseRequest.Task);
            addTaskUseCaseCallBack.OnSuccess(addTaskUseCaseResponseUseCaseResponse);
        }

    }
}
