using Projects.Core.Entity;
using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ZTaskUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Data.DataManager.ZTaskDataManager
{
    public class DeleteTasksDataManager : IDeleteTasksDataManger
    {
        ITaskDBHandler _TaskDBHandler;
        ISubTaskDBHandler _SubTaskDBHandler;
        public DeleteTasksDataManager(ITaskDBHandler projectDBHandler, ISubTaskDBHandler subTaskDBHandler)
        {
            _TaskDBHandler = projectDBHandler;
            _SubTaskDBHandler =subTaskDBHandler;
        }

        public void DeleteTasks(IDeleteTasksUseCaseRequest deleteTaskUseCaseRequest, IUseCaseCallBackBase<DeleteTaskUseCaseResponse> deleteTaskUseCaseCallBack)
        {
           
            try
            {
                foreach(ZTask task in deleteTaskUseCaseRequest.Tasks)
                {
                    _TaskDBHandler.DeleteTask(task.ID);
                    _SubTaskDBHandler.DeleteSubTask(task.ID);
                }
               
                DeleteTaskUseCaseResponse deleteTaskUseCaseResponseUseCaseResponse = new DeleteTaskUseCaseResponse();
                deleteTaskUseCaseCallBack.OnSuccess(deleteTaskUseCaseResponseUseCaseResponse);
            }
            catch (Exception ex)
            {
                deleteTaskUseCaseCallBack.OnError(ex.Message); 
            }
        }

    }
}
