using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Domain.UseCase.ZUserUseCase;
using Projects.Presentation.ViewContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Windows.UI.Core;

namespace Projects.Presentation.ViewModel
{
    public class AddUserVM
    {
        IAddUser _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public AddUserVM(IAddUser view)
        {
            this._View = view;
        }
        public void AddUser(ZUser User)
        {
            AddUserUseCase useCase = new AddUserUseCase(new AddUserUseCaseRequest(User), new AddUserPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class AddUserPresenterCallback : IPresenterCallBack<AddUserUseCaseResponse>
        {
            IAddUser View;

            public AddUserPresenterCallback(IAddUser view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Users Adding Canceled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(AddUserUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.UserAdded(usecaseRespone.ZUser);
                });

            }
        }
    }
}
