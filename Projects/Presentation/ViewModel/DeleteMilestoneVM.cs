using Milestones.Domain.UseCase.MilestoneUseCase;
using Projects.Core.EntityObj;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Presentation.ViewContract;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Windows.UI.Core;

namespace Projects.Presentation.ViewModel
{
    public class DeleteMilestoneVM
    {
        IDeleteMilestones _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public DeleteMilestoneVM(IDeleteMilestones view)
        {
            this._View = view;
        }
        public void DeleteMilestone(IList<MilestoneObj> milestones)
        {
            DeleteMilestoneUseCase useCase = new DeleteMilestoneUseCase(new DeleteMilestonesUseCaseRequest(milestones), new DeleteMilestonePresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class DeleteMilestonePresenterCallback : IPresenterCallBack<DeleteMilestoneUseCaseResponse>
        {
            IDeleteMilestones View;

            public DeleteMilestonePresenterCallback(IDeleteMilestones view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Milestones Deleting Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(DeleteMilestoneUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Milestones Deleted Successfully");
                });

            }
        }
    }
}
