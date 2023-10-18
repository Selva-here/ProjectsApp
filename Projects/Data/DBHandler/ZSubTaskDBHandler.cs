using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DataBaseAdapter;
using Projects.Domain.UseCase.ZTaskUseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Data.DBHandler
{
    public interface ISubTaskDBHandler {
        List<ZSubTask> GetTaskSubTasks(int taskID);
        void AddSubTask(ZSubTask subTask);
        void EditSubTaskProperty(IEditSubTaskPropertyUseCaseRequest editSubTaskPropertyUseCaseRequest);
        void DeleteSubTask(int subTaskID);
        void DeleteTaskSubTasks(int taskID);
    }

    public class ZSubTaskDBHandler : ISubTaskDBHandler
    {
        IDBAdapter _DBAdapter;
        public ZSubTaskDBHandler(IDBAdapter dBAdapter)
        {
            _DBAdapter = dBAdapter;
        }
        public List<ZSubTask> GetTaskSubTasks(int taskID)
        {
            return _DBAdapter.ExecuteQuery<ZSubTask>(QueryStrings.GetSubTasksBaseQuery, taskID);
        }
        public void AddSubTask(ZSubTask subTask)
        {
            throw new NotImplementedException();
        }

        public void DeleteSubTask(int subTaskID)
        {
            throw new NotImplementedException();
        }

        public void DeleteTaskSubTasks(int taskID)
        {
             _DBAdapter.ExecuteQuery<ZSubTask>(QueryStrings.DeleteTaskSubTasksBaseQuery, taskID);
        }

        public void EditSubTaskProperty(IEditSubTaskPropertyUseCaseRequest editSubTaskPropertyUseCaseRequest)
        {
            throw new NotImplementedException();
        }
    }
}
