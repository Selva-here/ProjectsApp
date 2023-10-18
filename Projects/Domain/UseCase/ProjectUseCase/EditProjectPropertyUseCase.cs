using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Projects.Presentation.ViewModel;
using Projects.Core.AppEnum;

namespace Projects.Domain.UseCase.ProjectUseCase
{
    public class EditProjectPropertyUseCase : UseCaseBase<EditProjectPropertyUseCaseResponse>
    {
        IEditProjectPropertyUseCaseRequest _EditProjectPropertyUseCaseRequest;

        IPresenterCallBack<EditProjectPropertyUseCaseResponse> _EditProjectPropertyPresenterCallback;

        IEditProjectPropertyDataManger _EditProjectPropertyDataManager;
        public EditProjectPropertyUseCase(IEditProjectPropertyUseCaseRequest editProjectPropertyUseCaseRequest, IPresenterCallBack<EditProjectPropertyUseCaseResponse> editProjectPropertyPresenterCallback, CancellationToken ct) : base(ct, editProjectPropertyPresenterCallback)
        {
            this._EditProjectPropertyUseCaseRequest = editProjectPropertyUseCaseRequest;
            this._EditProjectPropertyPresenterCallback = editProjectPropertyPresenterCallback;
            _EditProjectPropertyDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IEditProjectPropertyDataManger>();
        }
        public override void Action()
        {
            _EditProjectPropertyDataManager.EditProjectProperty(_EditProjectPropertyUseCaseRequest, new EditProjectPropertyUseCaseCallBack(_EditProjectPropertyPresenterCallback));
        }
        class EditProjectPropertyUseCaseCallBack : IUseCaseCallBackBase<EditProjectPropertyUseCaseResponse>
        {
            IPresenterCallBack<EditProjectPropertyUseCaseResponse> _EditProjectPropertyPresenterCallback;
            public EditProjectPropertyUseCaseCallBack(IPresenterCallBack<EditProjectPropertyUseCaseResponse> editProjectPropertyPresenterCallback)
            {
                this._EditProjectPropertyPresenterCallback = editProjectPropertyPresenterCallback;
            }
            public void OnSuccess(EditProjectPropertyUseCaseResponse usecaseResponse)
            {
                _EditProjectPropertyPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                _EditProjectPropertyPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                _EditProjectPropertyPresenterCallback.OnError(message);
            }
        }
    }
    public interface IEditProjectPropertyUseCaseRequest
    {
        int ProjectID { get; set; }
        ProjectPropertyEditType ProjectPropertyType { get; set; }
        Object Value { get; set; }
    }

    public class EditProjectPropertyUseCaseRequest : IEditProjectPropertyUseCaseRequest
    {
        public int ProjectID { get; set; }
        public ProjectPropertyEditType ProjectPropertyType { get; set; }
        public Object Value { get; set; }
       
        public EditProjectPropertyUseCaseRequest(int projectId,ProjectPropertyEditType projectPropertyType,object property)
        {
            ProjectID = projectId;
            ProjectPropertyType = projectPropertyType; 
            Value = property;
        }
    }
    public class EditProjectPropertyUseCaseResponse
    {
        public ProjectPropertyEditType ProjectPropertyEditType;
        public EditProjectPropertyUseCaseResponse(ProjectPropertyEditType projectPropertyEditType)
        {
            ProjectPropertyEditType = projectPropertyEditType;
        }
    }
    public interface IEditProjectPropertyDataManger
    {
        void EditProjectProperty(IEditProjectPropertyUseCaseRequest editProjectPropertyUseCaseRequest, IUseCaseCallBackBase<EditProjectPropertyUseCaseResponse> editProjectPropertyUseCaseCallBack);
    }
}
