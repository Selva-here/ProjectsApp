using Projects.Core.AppEnum;
using Projects.Domain.UseCase.ProjectUseCase;
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
    internal class EditProjectVM
    {
        IEditProject _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public EditProjectVM(IEditProject view)
        {
            this._View = view;
        }
        public void EditProjectProperty(int projectId, ProjectPropertyEditType propertyType, object value)
        {
            EditProjectPropertyUseCase useCase = new EditProjectPropertyUseCase(new EditProjectPropertyUseCaseRequest(projectId, propertyType, value), new EditProjectPropertyPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class EditProjectPropertyPresenterCallback : IPresenterCallBack<EditProjectPropertyUseCaseResponse>
        {
            IEditProject View;

            public EditProjectPropertyPresenterCallback(IEditProject view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Projects Updating Cancled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(EditProjectPropertyUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if(usecaseRespone.ProjectPropertyEditType!=ProjectPropertyEditType.Description)
                    View.ShowNotification("Project " + usecaseRespone.ProjectPropertyEditType + " Updated Successfully");
                });

            }
        }
    }
}
