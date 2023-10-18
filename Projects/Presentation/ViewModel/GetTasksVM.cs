using Projects.Core.AppEnum;
using Projects.Domain;
using Projects.Domain.UseCase.ZUserUseCase;
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
    public class GetTasksVM
    {
        IGetTasks _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public GetTasksVM(IGetTasks view)
        {
            this._View = view;
        }
        public void GetTasks(TasksType taskType, object value)
        {
            GetTasksUseCase useCase = new GetTasksUseCase(new GetTasksUseCaseRequest(taskType, value), new GetTasksPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class GetTasksPresenterCallback : IPresenterCallBack<GetTasksUseCaseResponse>
        {
            IGetTasks View;

            public GetTasksPresenterCallback(IGetTasks view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Tasks Loading Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(GetTasksUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (usecaseRespone.TasksType == TasksType.Particular)
                    {
                        if (usecaseRespone.Tasks.Count < 1)
                        {
                            OnError("Particular Task Not Fetched");
                        }
                        else
                        {
                            View.LoadTask(usecaseRespone.Tasks.First());
                        }
                    }
                    else
                    {
                        View.LoadTasks(usecaseRespone.TasksType, usecaseRespone.Tasks);
                    }

                });

            }
        }
    }
}
