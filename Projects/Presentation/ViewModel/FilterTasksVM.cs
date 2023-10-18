using Projects.Core.AppEnum;
using Projects.Domain;
using Projects.Domain.UseCase.ZTaskUseCase;
using Projects.Presentation.View.AppUserControl;
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
    public class FilterTasksVM
    {
        IFilterTasks _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public FilterTasksVM(IFilterTasks view)
        {
            _View = view;
        }
       
        public void FilterTasks(FilterMethod filterMethod, List<FilterPropertyAndValue> filterPropertyAndValueList)
        {
            FilterTasksUseCase useCase = new FilterTasksUseCase(new FilterTasksUseCaseRequest(filterMethod,filterPropertyAndValueList), new FilterTasksPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class FilterTasksPresenterCallback : IPresenterCallBack<FilterTasksUseCaseResponse>
        {
            IFilterTasks _View;

            public FilterTasksPresenterCallback(IFilterTasks view)
            {
                _View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Tasks Loading Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    _View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(FilterTasksUseCaseResponse usecaseRespone)
            {
                _ = _View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                   _View.LoadTasks(usecaseRespone.Tasks);
                });

            }
        }
    }
}
