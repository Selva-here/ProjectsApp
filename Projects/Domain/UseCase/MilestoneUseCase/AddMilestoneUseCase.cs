using Microsoft.Extensions.DependencyInjection;
using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projects.Domain.UseCase.MilestoneUseCase
{
    public class AddMilestoneUseCase : UseCaseBase<AddMilestoneUseCaseResponse>
    {
        IAddMilestoneUseCaseRequest _AddMilestoneUseCaseRequest;

        IPresenterCallBack<AddMilestoneUseCaseResponse> _AddMilestonePresenterCallback;

        IAddMilestoneDataManger _AddMilestoneDataManager;
        public AddMilestoneUseCase(IAddMilestoneUseCaseRequest addMilestoneUseCaseRequest, IPresenterCallBack<AddMilestoneUseCaseResponse> addMilestonePresenterCallback, CancellationToken ct) : base(ct, addMilestonePresenterCallback)
        {
            _AddMilestoneUseCaseRequest = addMilestoneUseCaseRequest;
            _AddMilestonePresenterCallback = addMilestonePresenterCallback;
            _AddMilestoneDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IAddMilestoneDataManger>();
        }
        public override void Action()
        {

            _AddMilestoneDataManager.AddMilestones(_AddMilestoneUseCaseRequest, new AddMilestoneUseCaseCallBack(_AddMilestonePresenterCallback));
        }
        class AddMilestoneUseCaseCallBack : IUseCaseCallBackBase<AddMilestoneUseCaseResponse>
        {
            IPresenterCallBack<AddMilestoneUseCaseResponse> addMilestonePresenterCallback;
            public AddMilestoneUseCaseCallBack(IPresenterCallBack<AddMilestoneUseCaseResponse> getUserOwnedMilestonesPresenterCallback)
            {
                this.addMilestonePresenterCallback = getUserOwnedMilestonesPresenterCallback;
            }
            public void OnSuccess(AddMilestoneUseCaseResponse usecaseResponse)
            {
                addMilestonePresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                addMilestonePresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                addMilestonePresenterCallback.OnError(message);
            }
        }
    }
    public interface IAddMilestoneUseCaseRequest
    {
        MilestoneObj Milestone { get; set; }
    }

    public class AddMilestoneUseCaseRequest : IAddMilestoneUseCaseRequest
    {
        public MilestoneObj Milestone { get; set; }
        public AddMilestoneUseCaseRequest(MilestoneObj milestone)
        {
            Milestone = milestone;
        }
    }
    public class AddMilestoneUseCaseResponse
    {
        public MilestoneObj _Milestone;
        public AddMilestoneUseCaseResponse(MilestoneObj milestone)
        {
            _Milestone = milestone;
        }
    }
    public interface IAddMilestoneDataManger
    {
        void AddMilestones(IAddMilestoneUseCaseRequest addMilestoneUseCaseRequest, IUseCaseCallBackBase<AddMilestoneUseCaseResponse> addMilestoneUseCaseCallBack);
    }
}
