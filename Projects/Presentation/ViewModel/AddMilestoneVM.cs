using Projects.Core.EntityObj;
using Projects.Domain.UseCase.MilestoneUseCase;
using Projects.Presentation.ViewContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Projects.Presentation.ViewModel
{
    internal class AddMilestoneVM
    {
        IAddMilestone _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public AddMilestoneVM(IAddMilestone view)
        {
            this._View = view;
        }
        public void AddMilestone(MilestoneObj milestone)
        {
            AddMilestoneUseCase useCase = new AddMilestoneUseCase(new AddMilestoneUseCaseRequest(milestone), new AddMilestonePresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class AddMilestonePresenterCallback : IPresenterCallBack<AddMilestoneUseCaseResponse>
        {
            IAddMilestone _View;

            public AddMilestonePresenterCallback(IAddMilestone view)
            {
                _View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Milestones Adding Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(AddMilestoneUseCaseResponse usecaseRespone)
            {
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ShowNotification("Milestone Uploaded Sucessfully");
                    _View.MilestoneAdded(usecaseRespone._Milestone);
                });

            }
        }
    }
}
