using Projects.Core.AppEnum;
using Projects.Domain;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Presentation.ViewContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Projects.Presentation.ViewModel
{
    internal class GetProjectsVM
    {
        IGetProjects _View;
        CancellationTokenSource _ZCTS = new CancellationTokenSource();
        public GetProjectsVM(IGetProjects view)
        {
            this._View = view;
        }
        public void GetProjects(ProjectsType projectType, object value,AutoSuggestBox autoSuggestBox)
        {
            GetProjectsUseCase useCase = new GetProjectsUseCase(new GetProjectsUseCaseRequest(projectType, value,autoSuggestBox), new GetProjectsPresenterCallback(_View), _ZCTS.Token);
            useCase.Execute();
        }
        class GetProjectsPresenterCallback : IPresenterCallBack<GetProjectsUseCaseResponse>
        {
            IGetProjects View;

            public GetProjectsPresenterCallback(IGetProjects view)
            {
                View = view;
            }
            public void OnCancel()
            {
                Debug.WriteLine("Projects Loading Cancled");
            }
            public void OnError(string message)
            {
                Debug.WriteLine(message);
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    View.ShowNotification("Sorry! Something Went Wrong");
                });
            }

            public void OnSuccess(GetProjectsUseCaseResponse usecaseRespone)
            {
                _ = View.ZCoreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (usecaseRespone.ProjectsType == ProjectsType.Particular)
                    {
                        if (usecaseRespone.Projects.Count < 1)
                        {
                            OnError("Particular Project Not Fetched");
                        }
                        else
                        {
                            View.LoadProject(usecaseRespone.Projects.First());
                        }
                    }
                    else if (usecaseRespone.ProjectsType == ProjectsType.NameSearch)
                    {
                        View.AutoSuggestionBoxProjectSuggestion(usecaseRespone.AutoSuggestBox, usecaseRespone.Projects);
                    }
                    else
                    {
                        View.LoadProjects(usecaseRespone.ProjectsType, usecaseRespone.Projects);
                    }

                });

            }
        }
    }
}
