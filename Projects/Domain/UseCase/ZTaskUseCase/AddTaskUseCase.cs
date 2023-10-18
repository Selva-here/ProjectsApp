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

    public class AddTaskUseCase : UseCaseBase<AddTaskUseCaseResponse>
    {
        IAddTaskUseCaseRequest _AddTaskUseCaseRequest;

        IPresenterCallBack<AddTaskUseCaseResponse> _AddTaskPresenterCallback;

        IAddTaskDataManger addTaskDataManager;
        public AddTaskUseCase(IAddTaskUseCaseRequest AddTaskUseCaseRequest, IPresenterCallBack<AddTaskUseCaseResponse> addTaskPresenterCallback, CancellationToken ct) : base(ct, addTaskPresenterCallback)
        {
            _AddTaskUseCaseRequest = AddTaskUseCaseRequest;
            _AddTaskPresenterCallback = addTaskPresenterCallback;
            addTaskDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IAddTaskDataManger>();
        }
        public override void Action()
        {

            addTaskDataManager.AddTask(_AddTaskUseCaseRequest, new AddTaskUseCaseCallBack(_AddTaskPresenterCallback));
        }
        class AddTaskUseCaseCallBack : IUseCaseCallBackBase<AddTaskUseCaseResponse>
        {
            IPresenterCallBack<AddTaskUseCaseResponse> addTaskPresenterCallback;
            public AddTaskUseCaseCallBack(IPresenterCallBack<AddTaskUseCaseResponse> getUserOwnedTasksPresenterCallback)
            {
                this.addTaskPresenterCallback = getUserOwnedTasksPresenterCallback;
            }
            public void OnSuccess(AddTaskUseCaseResponse usecaseResponse)
            {
                addTaskPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                addTaskPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                addTaskPresenterCallback.OnError(message);
            }
        }
    }
    public interface IAddTaskUseCaseRequest
    {
        ZTaskObj Task { get; set; }
    }

    public class AddTaskUseCaseRequest : IAddTaskUseCaseRequest
    {
        public ZTaskObj Task { get; set; }
        public AddTaskUseCaseRequest(ZTaskObj task)
        {
            Task = task;
        }
    }
    public class AddTaskUseCaseResponse
    {
        public ZTaskObj ZTask;
        public AddTaskUseCaseResponse(ZTaskObj task)
        {
            ZTask=task;
        }
    }
    public interface IAddTaskDataManger
    {
        void AddTask(IAddTaskUseCaseRequest addTaskUseCaseRequest, IUseCaseCallBackBase<AddTaskUseCaseResponse> addTaskUseCaseCallBack);
    }
}