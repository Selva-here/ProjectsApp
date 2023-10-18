using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Domain.UseCase.ZTaskUseCase;
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
    public class DeleteTasksVM
    {
        IDeleteTasks _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public DeleteTasksVM(IDeleteTasks view)
        {
            this._View = view;
        }
        public void DeleteTask(IList<ZTaskObj> tasks)
        {
            DeleteTasksUseCase useCase = new DeleteTasksUseCase(new DeleteTasksUseCaseRequest(tasks), new DeleteTaskPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class DeleteTaskPresenterCallback : IPresenterCallBack<DeleteTaskUseCaseResponse>
        {
            IDeleteTasks View;

            public DeleteTaskPresenterCallback(IDeleteTasks view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Tasks Deleting Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(DeleteTaskUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Tasks Deleted Successfully");
                });

            }
        }
    }
}
