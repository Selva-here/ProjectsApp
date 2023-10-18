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
    public class DeleteProjectsVM
    {
        IDeleteProjects _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public DeleteProjectsVM(IDeleteProjects view)
        {
            this._View = view;
        }
        public void DeleteProject(IList<ProjectObj> projects)
        {
            DeleteProjectsUseCase useCase = new DeleteProjectsUseCase(new DeleteProjectsUseCaseRequest(projects), new DeleteProjectPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class DeleteProjectPresenterCallback : IPresenterCallBack<DeleteProjectUseCaseResponse>
        {
            IDeleteProjects View;

            public DeleteProjectPresenterCallback(IDeleteProjects view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Projects Deleting Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(DeleteProjectUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Projects Deleted Successfully");
                });

            }
        }
    }
}
