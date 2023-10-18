using Microsoft.Extensions.DependencyInjection;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
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
    public class DeleteSubTasksUseCase : UseCaseBase<DeleteSubTaskUseCaseResponse>
    {
        IDeleteSubTasksUseCaseRequest _DeleteSubTasksUseCaseRequest;

        IPresenterCallBack<DeleteSubTaskUseCaseResponse> _DeleteSubTasksPresenterCallback;

        IDeleteSubTasksDataManger _DeleteSubTasksDataManager;
        public DeleteSubTasksUseCase(IDeleteSubTasksUseCaseRequest deleteSubTasksUseCaseRequest, IPresenterCallBack<DeleteSubTaskUseCaseResponse> deleteSubTasksPresenterCallback, CancellationToken ct) : base(ct, deleteSubTasksPresenterCallback)
        {
            _DeleteSubTasksUseCaseRequest = deleteSubTasksUseCaseRequest;
            _DeleteSubTasksPresenterCallback = deleteSubTasksPresenterCallback;
            _DeleteSubTasksDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IDeleteSubTasksDataManger>();
        }
        public override void Action()
        {

            _DeleteSubTasksDataManager.DeleteSubTasks(_DeleteSubTasksUseCaseRequest, new DeleteSubTasksUseCaseCallBack(_DeleteSubTasksPresenterCallback));
        }
        class DeleteSubTasksUseCaseCallBack : IUseCaseCallBackBase<DeleteSubTaskUseCaseResponse>
        {
            IPresenterCallBack<DeleteSubTaskUseCaseResponse> deleteSubTaskPresenterCallback;
            public DeleteSubTasksUseCaseCallBack(IPresenterCallBack<DeleteSubTaskUseCaseResponse> getUserOwnedTasksPresenterCallback)
            {
                this.deleteSubTaskPresenterCallback = getUserOwnedTasksPresenterCallback;
            }
            public void OnSuccess(DeleteSubTaskUseCaseResponse usecaseResponse)
            {
                deleteSubTaskPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                deleteSubTaskPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                deleteSubTaskPresenterCallback.OnError(message);
            }
        }
    }
    public interface IDeleteSubTasksUseCaseRequest
    {
        IList<ZTaskObj> SubTasks { get; set; }
    }

    public class DeleteSubTasksUseCaseRequest : IDeleteSubTasksUseCaseRequest
    {
        public IList<ZTaskObj> SubTasks { get; set; }
        public DeleteSubTasksUseCaseRequest(IList<ZTaskObj> task)
        {
            SubTasks = task;
        }
    }
    public class DeleteSubTaskUseCaseResponse
    {

        public DeleteSubTaskUseCaseResponse()
        {
        }
    }
    public interface IDeleteSubTasksDataManger
    {
        void DeleteSubTasks(IDeleteSubTasksUseCaseRequest deleteSubTasksUseCaseRequest, IUseCaseCallBackBase<DeleteSubTaskUseCaseResponse> deleteSubTasksUseCaseCallBack);
    }
}
