using Projects.Core.Entity;
using Projects.Core.AppEnum;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Threading;
using Projects.Domain;
using Projects.DI;
using Microsoft.Extensions.DependencyInjection;
using Projects.Core.EntityObj;

namespace Projects.Domain
{
    public class GetTasksUseCase : UseCaseBase<GetTasksUseCaseResponse>
    {
        IGetTasksUseCaseRequest _GetTasksUseCaseRequest;
        IPresenterCallBack<GetTasksUseCaseResponse> _GetTasksPresenterCallback;
        IGetTasksDataManger _GetTasksDataManager;
        public GetTasksUseCase(IGetTasksUseCaseRequest getTasksUseCaseRequest, IPresenterCallBack<GetTasksUseCaseResponse> getTasksPresenterCallback, CancellationToken ct) : base(ct, getTasksPresenterCallback)
        {
            _GetTasksUseCaseRequest = getTasksUseCaseRequest;
            _GetTasksPresenterCallback = getTasksPresenterCallback;
            _GetTasksDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IGetTasksDataManger>();
        }
        public override void Action()
        {
            _GetTasksDataManager.GetTasks(_GetTasksUseCaseRequest, new GetTasksUseCaseCallBack(_GetTasksPresenterCallback));
        }

        class GetTasksUseCaseCallBack : IUseCaseCallBackBase<GetTasksUseCaseResponse>
        {
            IPresenterCallBack<GetTasksUseCaseResponse> getTasksPresenterCallback;
            public GetTasksUseCaseCallBack(IPresenterCallBack<GetTasksUseCaseResponse> getUserOwnedTasksPresenterCallback)
            {
                this.getTasksPresenterCallback = getUserOwnedTasksPresenterCallback;
            }
            public void OnSuccess(GetTasksUseCaseResponse usecaseResponse)
            {
                getTasksPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                getTasksPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                getTasksPresenterCallback.OnError(message);
            }
        }
    }
    public interface IGetTasksUseCaseRequest
    {
        Object Value { get; set; }
       TasksType TasksType { get; set; }
    }

    public class GetTasksUseCaseRequest : IGetTasksUseCaseRequest
    {
        public Object Value { get; set; }
        public TasksType TasksType { get; set; }

        public GetTasksUseCaseRequest(TasksType taskType, object value)
        {
            this.TasksType = taskType;
            Value = value;
        }
    }
    public class GetTasksUseCaseResponse
    {
        public IList<ZTaskObj> Tasks;
        public TasksType TasksType { get; set; }
        public GetTasksUseCaseResponse(IList<ZTaskObj> tasks, TasksType tasksType)
        {
            this.Tasks = tasks;
            TasksType = tasksType;
        }
    }
    public interface IGetTasksDataManger
    {
        void GetTasks(IGetTasksUseCaseRequest getTasksUseCaseRequest, IUseCaseCallBackBase<GetTasksUseCaseResponse> getTasksUseCaseCallBack);
    }
}


