using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Domain.UseCase.ZUserUseCase;
using Projects.Presentation.ViewContract;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Projects.Presentation.ViewModel
{
    internal class GetUsersVM
    {
        IGetUsers _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public GetUsersVM(IGetUsers view)
        {
            this._View = view;
        }
        public void GetUsers(UsersType usersType, object value,int projectID,AutoSuggestBox autoSuggestBox)
        {
            GetUsersUseCase useCase = new GetUsersUseCase(new GetUsersUseCaseRequest(usersType, value,projectID,autoSuggestBox), new GetUsersPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class GetUsersPresenterCallback : IPresenterCallBack<GetUsersUseCaseResponse>
        {
            IGetUsers View;

            public GetUsersPresenterCallback(IGetUsers view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Users Loading Cancled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(GetUsersUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (usecaseRespone.UsersType == UsersType.Particular || usecaseRespone.UsersType == UsersType.MailID)
                    {
                        if (usecaseRespone.Users.Count < 1)
                        {
                            View.LoadUser(null);
                        }
                        else
                        {
                            View.LoadUser(usecaseRespone.Users.First());
                        }
                    }
                    else if (usecaseRespone.UsersType == UsersType.NameSearch || usecaseRespone.UsersType == UsersType.ProjectNameSearch)
                    {
                       View.AutoSuggestionBoxUserSuggestion(usecaseRespone.AutoSuggestBox,usecaseRespone.Users);
                    }
                    else
                    {
                        View.LoadUsers(usecaseRespone.Users);
                    }

                });

            }
        }
    }
}
