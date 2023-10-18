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
    public class EditSubTaskPropertyUseCase : UseCaseBase<EditSubTaskPropertyUseCaseResponse>
    {
        IEditSubTaskPropertyUseCaseRequest _EditSubTaskPropertyUseCaseRequest;

        IPresenterCallBack<EditSubTaskPropertyUseCaseResponse> _EditSubTaskPropertyPresenterCallback;

        IEditSubTaskPropertyDataManger _EditSubTaskPropertyDataManager;
        public EditSubTaskPropertyUseCase(IEditSubTaskPropertyUseCaseRequest editSubTaskPropertyUseCaseRequest, IPresenterCallBack<EditSubTaskPropertyUseCaseResponse> editSubTaskPropertyPresenterCallback, CancellationToken ct) : base(ct, editSubTaskPropertyPresenterCallback)
        {
            this._EditSubTaskPropertyUseCaseRequest = editSubTaskPropertyUseCaseRequest;
            this._EditSubTaskPropertyPresenterCallback = editSubTaskPropertyPresenterCallback;
            _EditSubTaskPropertyDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IEditSubTaskPropertyDataManger>();
        }
        public override void Action()
        {
            _EditSubTaskPropertyDataManager.EditSubTaskProperty(_EditSubTaskPropertyUseCaseRequest, new EditSubTaskPropertyUseCaseCallBack(_EditSubTaskPropertyPresenterCallback));
        }
        class EditSubTaskPropertyUseCaseCallBack : IUseCaseCallBackBase<EditSubTaskPropertyUseCaseResponse>
        {
            IPresenterCallBack<EditSubTaskPropertyUseCaseResponse> _EditSubTaskPropertyPresenterCallback;
            public EditSubTaskPropertyUseCaseCallBack(IPresenterCallBack<EditSubTaskPropertyUseCaseResponse> editSubTaskPropertyPresenterCallback)
            {
                this._EditSubTaskPropertyPresenterCallback = editSubTaskPropertyPresenterCallback;
            }
            public void OnSuccess(EditSubTaskPropertyUseCaseResponse usecaseResponse)
            {
                _EditSubTaskPropertyPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                _EditSubTaskPropertyPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                _EditSubTaskPropertyPresenterCallback.OnError(message);
            }
        }
    }
    public interface IEditSubTaskPropertyUseCaseRequest
    {
        int TaskID { get; set; }
        TaskPropertyEditType TaskPropertyEditType { get; set; }
        Object Value { get; set; }
    }

    public class EditSubTaskPropertyUseCaseRequest : IEditSubTaskPropertyUseCaseRequest
    {
        public int TaskID { get; set; }
        public TaskPropertyEditType TaskPropertyEditType { get; set; }
        public Object Value { get; set; }

        public EditSubTaskPropertyUseCaseRequest(int taskId, TaskPropertyEditType taskPropertyType, object property)
        {
            TaskID = taskId;
            TaskPropertyEditType = taskPropertyType;
            Value = property;
        }
    }
    public class EditSubTaskPropertyUseCaseResponse
    {
        public TaskPropertyEditType TaskPropertyEditType;
        public EditSubTaskPropertyUseCaseResponse(TaskPropertyEditType taskPropertyEditType)
        {
            TaskPropertyEditType = taskPropertyEditType;
        }
    }
    public interface IEditSubTaskPropertyDataManger
    {
        void EditSubTaskProperty(IEditSubTaskPropertyUseCaseRequest editSubTaskPropertyUseCaseRequest, IUseCaseCallBackBase<EditSubTaskPropertyUseCaseResponse> editSubTaskPropertyUseCaseCallBack);
    }
}
