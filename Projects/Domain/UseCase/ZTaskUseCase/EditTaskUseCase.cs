using Projects.DI;
using Projects.Presentation.ViewModel;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Projects.Core.AppEnum;

namespace Projects.Domain.UseCase.ZTaskUseCase
{
    public class EditTaskPropertyUseCase : UseCaseBase<EditTaskPropertyUseCaseResponse>
    {
        IEditTaskPropertyUseCaseRequest _EditTaskPropertyUseCaseRequest;

        IPresenterCallBack<EditTaskPropertyUseCaseResponse> _EditTaskPropertyPresenterCallback;

        IEditTaskPropertyDataManger _EditTaskPropertyDataManager;
        public EditTaskPropertyUseCase(IEditTaskPropertyUseCaseRequest editTaskPropertyUseCaseRequest, IPresenterCallBack<EditTaskPropertyUseCaseResponse> editTaskPropertyPresenterCallback, CancellationToken ct) : base(ct, editTaskPropertyPresenterCallback)
        {
            this._EditTaskPropertyUseCaseRequest = editTaskPropertyUseCaseRequest;
            this._EditTaskPropertyPresenterCallback = editTaskPropertyPresenterCallback;
            _EditTaskPropertyDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IEditTaskPropertyDataManger>();
        }
        public override void Action()
        {
            _EditTaskPropertyDataManager.EditTaskProperty(_EditTaskPropertyUseCaseRequest, new EditTaskPropertyUseCaseCallBack(_EditTaskPropertyPresenterCallback));
        }
        class EditTaskPropertyUseCaseCallBack : IUseCaseCallBackBase<EditTaskPropertyUseCaseResponse>
        {
            IPresenterCallBack<EditTaskPropertyUseCaseResponse> _EditTaskPropertyPresenterCallback;
            public EditTaskPropertyUseCaseCallBack(IPresenterCallBack<EditTaskPropertyUseCaseResponse> editTaskPropertyPresenterCallback)
            {
                this._EditTaskPropertyPresenterCallback = editTaskPropertyPresenterCallback;
            }
            public void OnSuccess(EditTaskPropertyUseCaseResponse usecaseResponse)
            {
                _EditTaskPropertyPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                _EditTaskPropertyPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                _EditTaskPropertyPresenterCallback.OnError(message);
            }
        }
    }
    public interface IEditTaskPropertyUseCaseRequest
    {
        int TaskID { get; set; }
        TaskPropertyEditType TaskPropertyEditType { get; set; }
        Object Value { get; set; }
    }

    public class EditTaskPropertyUseCaseRequest : IEditTaskPropertyUseCaseRequest
    {
        public int TaskID { get; set; }
        public TaskPropertyEditType TaskPropertyEditType { get; set; }
        public Object Value { get; set; }

        public EditTaskPropertyUseCaseRequest(int taskId, TaskPropertyEditType taskPropertyType, object property)
        {
            TaskID = taskId;
            TaskPropertyEditType = taskPropertyType;
            Value = property;
        }
    }
    public class EditTaskPropertyUseCaseResponse
    {
        public TaskPropertyEditType TaskPropertyEditType;
        public EditTaskPropertyUseCaseResponse(TaskPropertyEditType taskPropertyEditType)
        {
            TaskPropertyEditType = taskPropertyEditType;
        }
    }
    public interface IEditTaskPropertyDataManger
    {
        void EditTaskProperty(IEditTaskPropertyUseCaseRequest editTaskPropertyUseCaseRequest, IUseCaseCallBackBase<EditTaskPropertyUseCaseResponse> editTaskPropertyUseCaseCallBack);
    }
}
