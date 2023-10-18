using Projects.Core.EntityObj;
using Projects.Domain.UseCase.ProjectUseCase;
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
    public class AddTaskVM
    {
        IAddTask _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public AddTaskVM(IAddTask view)
        {
            this._View = view;
        }
        public void AddTask(ZTaskObj Task)
        {
            AddTaskUseCase useCase = new AddTaskUseCase(new AddTaskUseCaseRequest(Task), new AddTaskPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class AddTaskPresenterCallback : IPresenterCallBack<AddTaskUseCaseResponse>
        {
            IAddTask _View;

            public AddTaskPresenterCallback(IAddTask view)
            {
                _View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Tasks Adding Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(AddTaskUseCaseResponse usecaseRespone)
            {
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ShowNotification("Task Added");
                    _View.TaskAdded(usecaseRespone.ZTask);
                });

            }
        }
    }
}
