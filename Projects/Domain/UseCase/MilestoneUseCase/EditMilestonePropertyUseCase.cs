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

namespace Projects.Domain.UseCase.MilestoneUseCase
{
    public class EditMilestonePropertyUseCase : UseCaseBase<EditMilestonePropertyUseCaseResponse>
    {
        IEditMilestonePropertyUseCaseRequest _EditMilestonePropertyUseCaseRequest;

        IPresenterCallBack<EditMilestonePropertyUseCaseResponse> _EditMilestonePropertyPresenterCallback;

        IEditMilestonePropertyDataManger _EditMilestonePropertyDataManager;
        public EditMilestonePropertyUseCase(IEditMilestonePropertyUseCaseRequest editMilestonePropertyUseCaseRequest, IPresenterCallBack<EditMilestonePropertyUseCaseResponse> editMilestonePropertyPresenterCallback, CancellationToken ct) : base(ct, editMilestonePropertyPresenterCallback)
        {
            this._EditMilestonePropertyUseCaseRequest = editMilestonePropertyUseCaseRequest;
            this._EditMilestonePropertyPresenterCallback = editMilestonePropertyPresenterCallback;
            _EditMilestonePropertyDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IEditMilestonePropertyDataManger>();
        }
        public override void Action()
        {
            _EditMilestonePropertyDataManager.EditMilestoneProperty(_EditMilestonePropertyUseCaseRequest, new EditMilestonePropertyUseCaseCallBack(_EditMilestonePropertyPresenterCallback));
        }
        class EditMilestonePropertyUseCaseCallBack : IUseCaseCallBackBase<EditMilestonePropertyUseCaseResponse>
        {
            IPresenterCallBack<EditMilestonePropertyUseCaseResponse> _EditMilestonePropertyPresenterCallback;
            public EditMilestonePropertyUseCaseCallBack(IPresenterCallBack<EditMilestonePropertyUseCaseResponse> editMilestonePropertyPresenterCallback)
            {
                this._EditMilestonePropertyPresenterCallback = editMilestonePropertyPresenterCallback;
            }
            public void OnSuccess(EditMilestonePropertyUseCaseResponse usecaseResponse)
            {
                _EditMilestonePropertyPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                _EditMilestonePropertyPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                _EditMilestonePropertyPresenterCallback.OnError(message);
            }
        }
    }
    public interface IEditMilestonePropertyUseCaseRequest
    {
        int MilestoneID { get; set; }
        MilestonePropertyEditType MilestonePropertyType { get; set; }
        Object Value { get; set; }
    }

    public class EditMilestonePropertyUseCaseRequest : IEditMilestonePropertyUseCaseRequest
    {
        public int MilestoneID { get; set; }
        public MilestonePropertyEditType MilestonePropertyType { get; set; }
        public Object Value { get; set; }

        public EditMilestonePropertyUseCaseRequest(int milestoneId, MilestonePropertyEditType milestonePropertyType, object property)
        {
            MilestoneID = milestoneId;
            MilestonePropertyType = milestonePropertyType;
            Value = property;
        }
    }
    public class EditMilestonePropertyUseCaseResponse
    {
        public MilestonePropertyEditType MilestonePropertyEditType;
        public EditMilestonePropertyUseCaseResponse(MilestonePropertyEditType milestonePropertyEditType)
        {
            MilestonePropertyEditType= milestonePropertyEditType;
        }
    }
    public interface IEditMilestonePropertyDataManger
    {
        void EditMilestoneProperty(IEditMilestonePropertyUseCaseRequest editMilestonePropertyUseCaseRequest, IUseCaseCallBackBase<EditMilestonePropertyUseCaseResponse> editMilestonePropertyUseCaseCallBack);
    }
}
