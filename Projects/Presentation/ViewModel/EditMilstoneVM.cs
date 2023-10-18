using Projects.Core.AppEnum;
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
    internal class EditMilstoneVM
    {
        IEditMilstone _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public EditMilstoneVM(IEditMilstone view)
        {
            this._View = view;
        }
        public void EditMilestoneProperty(int milestoneId, MilestonePropertyEditType propertyType, object value)
        {
            EditMilestonePropertyUseCase useCase = new EditMilestonePropertyUseCase(new EditMilestonePropertyUseCaseRequest(milestoneId, propertyType, value), new EditMilestonePropertyPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class EditMilestonePropertyPresenterCallback : IPresenterCallBack<EditMilestonePropertyUseCaseResponse>
        {
            IEditMilstone _View;

            public EditMilestonePropertyPresenterCallback(IEditMilstone view)
            {
                _View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Milestones Updating Cancled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(EditMilestonePropertyUseCaseResponse usecaseRespone)
            {
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if(usecaseRespone.MilestonePropertyEditType!=MilestonePropertyEditType.Description)
                    _View.ShowNotification("Milestone " + usecaseRespone.MilestonePropertyEditType + " Loaded Successfully");
                });

            }
        }
    }
}
