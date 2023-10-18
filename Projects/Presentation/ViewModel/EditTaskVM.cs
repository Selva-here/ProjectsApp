using Projects.Core.AppEnum;
using Projects.Domain.UseCase.ZTaskUseCase;
using Projects.Presentation.ViewContract;
using System.Diagnostics;
using System.Threading;
using Windows.UI.Core;

namespace Projects.Presentation.ViewModel
{
    internal class EditTaskVM
    {
        IEditTask _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public EditTaskVM(IEditTask view)
        {
            this._View = view;
        }
        public void EditTaskProperty(int taskId, TaskPropertyEditType propertyType, object value)
        {
            EditTaskPropertyUseCase useCase = new EditTaskPropertyUseCase(new EditTaskPropertyUseCaseRequest(taskId, propertyType, value), new EditTaskPropertyPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class EditTaskPropertyPresenterCallback : IPresenterCallBack<EditTaskPropertyUseCaseResponse>
        {
            IEditTask View;

            public EditTaskPropertyPresenterCallback(IEditTask view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Tasks Updating Cancled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(EditTaskPropertyUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if(usecaseRespone.TaskPropertyEditType!=TaskPropertyEditType.Description)
                    View.ShowNotification("Task " + usecaseRespone.TaskPropertyEditType + " Updated Successfully");
                });

            }
        }
    }
}
