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

    public class AddSubTaskUseCase : UseCaseBase<AddSubTaskUseCaseResponse>
    {
        IAddSubTaskUseCaseRequest _AddSubTaskUseCaseRequest;

        IPresenterCallBack<AddSubTaskUseCaseResponse> _AddSubTaskPresenterCallback;

        IAddSubTaskDataManger _AddTaskDataManager;
        public AddSubTaskUseCase(IAddSubTaskUseCaseRequest addSubTaskUseCaseRequest, IPresenterCallBack<AddSubTaskUseCaseResponse> addSubTaskPresenterCallback, CancellationToken ct) : base(ct, addSubTaskPresenterCallback)
        {
            _AddSubTaskUseCaseRequest = addSubTaskUseCaseRequest;
            _AddSubTaskPresenterCallback = addSubTaskPresenterCallback;
            _AddTaskDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IAddSubTaskDataManger>();
        }
        public override void Action()
        {
            _AddTaskDataManager.AddSubTask(_AddSubTaskUseCaseRequest, new AddSubTaskUseCaseCallBack(_AddSubTaskPresenterCallback));
        }
        class AddSubTaskUseCaseCallBack : IUseCaseCallBackBase<AddSubTaskUseCaseResponse>
        {
            IPresenterCallBack<AddSubTaskUseCaseResponse> _AddTaskPresenterCallback;
            public AddSubTaskUseCaseCallBack(IPresenterCallBack<AddSubTaskUseCaseResponse> getUserOwnedTasksPresenterCallback)
            {
                this._AddTaskPresenterCallback = getUserOwnedTasksPresenterCallback;
            }
            public void OnSuccess(AddSubTaskUseCaseResponse usecaseResponse)
            {
                _AddTaskPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                _AddTaskPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                _AddTaskPresenterCallback.OnError(message);
            }
        }
    }
    public interface IAddSubTaskUseCaseRequest
    {
        ZSubTask SubTask { get; set; }
    }

    public class AddSubTaskUseCaseRequest : IAddSubTaskUseCaseRequest
    {
        public ZSubTask SubTask { get; set; }
        public AddSubTaskUseCaseRequest(ZSubTask task)
        {
            SubTask = task;
        }
    }
    public class AddSubTaskUseCaseResponse
    {
        public ZSubTask SubTask;
        public AddSubTaskUseCaseResponse(ZSubTask task)
        {
            SubTask=task;
        }
    }
    public interface IAddSubTaskDataManger
    {
        void AddSubTask(IAddSubTaskUseCaseRequest addSubTaskUseCaseRequest, IUseCaseCallBackBase<AddSubTaskUseCaseResponse> addSubTaskUseCaseCallBack);
    }
}