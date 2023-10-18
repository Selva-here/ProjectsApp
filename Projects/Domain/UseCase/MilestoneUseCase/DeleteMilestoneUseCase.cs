using Microsoft.Extensions.DependencyInjection;
using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Domain;
using Projects.Presentation;
using System.Collections.Generic;
using System.Threading;

namespace Milestones.Domain.UseCase.MilestoneUseCase
{
    public class DeleteMilestoneUseCase : UseCaseBase<DeleteMilestoneUseCaseResponse>
    {
        IDeleteMilestonesUseCaseRequest _DeleteMilestonesUseCaseRequest;

        IPresenterCallBack<DeleteMilestoneUseCaseResponse> _DeleteMilestonesPresenterCallback;

        IDeleteMilestonesDataManger _DeleteMilestonesDataManager;
        public DeleteMilestoneUseCase(IDeleteMilestonesUseCaseRequest deleteMilestonesUseCaseRequest, IPresenterCallBack<DeleteMilestoneUseCaseResponse> deleteMilestonesPresenterCallback, CancellationToken ct) : base(ct, deleteMilestonesPresenterCallback)
        {
            _DeleteMilestonesUseCaseRequest = deleteMilestonesUseCaseRequest;
            _DeleteMilestonesPresenterCallback = deleteMilestonesPresenterCallback;
            _DeleteMilestonesDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IDeleteMilestonesDataManger>();
        }
        public override void Action()
        {
            _DeleteMilestonesDataManager.DeleteMilestones(_DeleteMilestonesUseCaseRequest, new DeleteMilestonesUseCaseCallBack(_DeleteMilestonesPresenterCallback));
        }
        class DeleteMilestonesUseCaseCallBack : IUseCaseCallBackBase<DeleteMilestoneUseCaseResponse>
        {
            IPresenterCallBack<DeleteMilestoneUseCaseResponse> deleteMilestonePresenterCallback;
            public DeleteMilestonesUseCaseCallBack(IPresenterCallBack<DeleteMilestoneUseCaseResponse> getUserOwnedMilestonesPresenterCallback)
            {
                this.deleteMilestonePresenterCallback = getUserOwnedMilestonesPresenterCallback;
            }
            public void OnSuccess(DeleteMilestoneUseCaseResponse usecaseResponse)
            {
                deleteMilestonePresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                deleteMilestonePresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                deleteMilestonePresenterCallback.OnError(message);
            }
        }
    }
    public interface IDeleteMilestonesUseCaseRequest
    {
        IList<MilestoneObj> Milestones { get; set; }
    }

    public class DeleteMilestonesUseCaseRequest : IDeleteMilestonesUseCaseRequest
    {
        public IList<MilestoneObj> Milestones { get; set; }
        public DeleteMilestonesUseCaseRequest(IList<MilestoneObj> Milestone)
        {
            Milestones = Milestone;
        }
    }
    public class DeleteMilestoneUseCaseResponse
    {
        public DeleteMilestoneUseCaseResponse()
        {
        }
    }
    public interface IDeleteMilestonesDataManger
    {
        void DeleteMilestones(IDeleteMilestonesUseCaseRequest deleteMilestonesUseCaseRequest, IUseCaseCallBackBase<DeleteMilestoneUseCaseResponse> deleteMilestonesUseCaseCallBack);
    }
}
