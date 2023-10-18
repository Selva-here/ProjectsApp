using Projects.Core.AppEnum;
using Projects.Domain.UseCase.MilestoneUseCase;
using Projects.Presentation.ViewContract;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Projects.Presentation.ViewModel
{
    internal class GetMilestonesVM
    {
        IGetMilestones _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public GetMilestonesVM(IGetMilestones view)
        {
            this._View = view;
        }
        public void GetMilestones(MilestonesType milestoneType, object value,int projectID,AutoSuggestBox autoSuggestBox)
        {
            GetMilestonesUseCase useCase = new GetMilestonesUseCase(new GetMilestonesUseCaseRequest(milestoneType, value,projectID,autoSuggestBox), new GetMilestonesPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class GetMilestonesPresenterCallback : IPresenterCallBack<GetMilestonesUseCaseResponse>
        {
            IGetMilestones View;

            public GetMilestonesPresenterCallback(IGetMilestones view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Milestones Loading Cancled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(GetMilestonesUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (usecaseRespone.MilestonesType == MilestonesType.Particular)
                    {
                        if (usecaseRespone.Milestones.Count < 1)
                        {
                            OnError("Particular Milestone Not Fetched");
                        }
                        else
                        {
                            View.LoadMilestone(usecaseRespone.Milestones.First());
                        }
                    }
                    else if (usecaseRespone.MilestonesType == MilestonesType.NameSearch || usecaseRespone.MilestonesType == MilestonesType.ProjectNameSearch)
                    {
                        View.AutoSuggestionBoxMilestoneSuggestion(usecaseRespone.AutoSuggestBox, usecaseRespone.Milestones);
                    }
                    else
                    {
                        View.LoadMilestones(usecaseRespone.MilestonesType, usecaseRespone.Milestones);
                    }
                });

            }
        }
    }
}
