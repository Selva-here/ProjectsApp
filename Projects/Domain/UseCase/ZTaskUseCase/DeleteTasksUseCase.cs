using Microsoft.Extensions.DependencyInjection;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DBHandler;
using Projects.DI;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projects.Domain.UseCase.ZTaskUseCase
{
    public class DeleteTasksUseCase : UseCaseBase<DeleteTaskUseCaseResponse>
    {
        IDeleteTasksUseCaseRequest _DeleteTasksUseCaseRequest;
       
        IPresenterCallBack<DeleteTaskUseCaseResponse> _DeleteTasksPresenterCallback;

        IDeleteTasksDataManger _DeleteTasksDataManager;
      
        public DeleteTasksUseCase(IDeleteTasksUseCaseRequest deleteTasksUseCaseRequest, IPresenterCallBack<DeleteTaskUseCaseResponse> deleteTasksPresenterCallback, CancellationToken ct) : base(ct, deleteTasksPresenterCallback)
        {
            _DeleteTasksUseCaseRequest = deleteTasksUseCaseRequest;
            _DeleteTasksPresenterCallback = deleteTasksPresenterCallback;
            _DeleteTasksDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IDeleteTasksDataManger>();
        }
        public override void Action()
        {

            _DeleteTasksDataManager.DeleteTasks(_DeleteTasksUseCaseRequest, new DeleteTasksUseCaseCallBack(_DeleteTasksPresenterCallback));
        }
        class DeleteTasksUseCaseCallBack : IUseCaseCallBackBase<DeleteTaskUseCaseResponse>
        {
            IPresenterCallBack<DeleteTaskUseCaseResponse> deleteTaskPresenterCallback;
            public DeleteTasksUseCaseCallBack(IPresenterCallBack<DeleteTaskUseCaseResponse> getUserOwnedTasksPresenterCallback)
            {
                this.deleteTaskPresenterCallback = getUserOwnedTasksPresenterCallback;
            }
            public void OnSuccess(DeleteTaskUseCaseResponse usecaseResponse)
            {
                deleteTaskPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                deleteTaskPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                deleteTaskPresenterCallback.OnError(message);
            }
        }
    }
    public interface IDeleteTasksUseCaseRequest
    {
        IList<ZTaskObj> Tasks { get; set; }
    }

    public class DeleteTasksUseCaseRequest : IDeleteTasksUseCaseRequest
    {
        public IList<ZTaskObj> Tasks { get; set; }
        public DeleteTasksUseCaseRequest(IList<ZTaskObj> task)
        {
            Tasks = task;
        }
    }
    public class DeleteTaskUseCaseResponse
    {

        public DeleteTaskUseCaseResponse()
        {
        }
    }
    public interface IDeleteTasksDataManger
    {
        void DeleteTasks(IDeleteTasksUseCaseRequest deleteTasksUseCaseRequest, IUseCaseCallBackBase<DeleteTaskUseCaseResponse> deleteTasksUseCaseCallBack);
    }
}
