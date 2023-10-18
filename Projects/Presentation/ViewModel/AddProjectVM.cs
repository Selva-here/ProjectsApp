using Projects.Core.EntityObj;
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
    internal class AddProjectVM
    {
        IAddProject _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public AddProjectVM(IAddProject view)
        {
            this._View = view;
        }
        public void AddProject(ProjectObj project)
        {
            AddProjectUseCase useCase = new AddProjectUseCase(new AddProjectUseCaseRequest(project), new AddProjectPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }

        class AddProjectPresenterCallback : IPresenterCallBack<AddProjectUseCaseResponse>
        {
            IAddProject _View;

            public AddProjectPresenterCallback(IAddProject view)
            {
                _View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Projects Adding Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(AddProjectUseCaseResponse usecaseRespone)
            {
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ProjectAdded(usecaseRespone.Project);
                });

            }
        }
    }
}
