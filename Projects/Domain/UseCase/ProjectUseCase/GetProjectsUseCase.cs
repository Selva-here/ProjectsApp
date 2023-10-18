using Microsoft.Extensions.DependencyInjection;
using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Projects.Domain.UseCase.ProjectUseCase
{
    public class GetProjectsUseCase : UseCaseBase<GetProjectsUseCaseResponse>
    {
        IGetProjectsUseCaseRequest _GetProjectsUseCaseRequest;

        IPresenterCallBack<GetProjectsUseCaseResponse> _GetProjectsPresenterCallback;

        IGetProjectsDataManger _GetProjectsDataManager;
        public GetProjectsUseCase(IGetProjectsUseCaseRequest getProjectsUseCaseRequest, IPresenterCallBack<GetProjectsUseCaseResponse> getProjectsPresenterCallback, CancellationToken ct) : base(ct, getProjectsPresenterCallback)
        {
            this._GetProjectsUseCaseRequest = getProjectsUseCaseRequest;
            this._GetProjectsPresenterCallback = getProjectsPresenterCallback;
            _GetProjectsDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IGetProjectsDataManger>();
        }
        public override void Action()
        {

            _GetProjectsDataManager.GetProjects(_GetProjectsUseCaseRequest, new GetProjectsUseCaseCallBack(_GetProjectsPresenterCallback));
        }
        class GetProjectsUseCaseCallBack : IUseCaseCallBackBase<GetProjectsUseCaseResponse>
        {
            IPresenterCallBack<GetProjectsUseCaseResponse> getProjectsPresenterCallback;
            public GetProjectsUseCaseCallBack(IPresenterCallBack<GetProjectsUseCaseResponse> getUserOwnedProjectsPresenterCallback)
            {
                this.getProjectsPresenterCallback = getUserOwnedProjectsPresenterCallback;
            }
            public void OnSuccess(GetProjectsUseCaseResponse usecaseResponse)
            {
                getProjectsPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                getProjectsPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                getProjectsPresenterCallback.OnError(message);
            }
        }
    }
    public interface IGetProjectsUseCaseRequest
    {
        ProjectsType ProjectsType { get; set; }
        object Value { get; set; }
       AutoSuggestBox AutoSuggestBox { get; set; }
    }

    public class GetProjectsUseCaseRequest : IGetProjectsUseCaseRequest
    {
        public ProjectsType ProjectsType { get; set; }
        public object Value { get; set; }
        public AutoSuggestBox AutoSuggestBox { get; set; }
        public GetProjectsUseCaseRequest(ProjectsType projectsType,Object value,AutoSuggestBox autoSuggestBox)
        {
            ProjectsType = projectsType;
            Value = value;
            AutoSuggestBox = autoSuggestBox;
        }
    }
    public class GetProjectsUseCaseResponse
    {
        public List<ProjectObj> Projects;
        public ProjectsType ProjectsType { get; set; }
        public AutoSuggestBox AutoSuggestBox { get; set; }
        public GetProjectsUseCaseResponse(List<ProjectObj> projects,ProjectsType projectsType,AutoSuggestBox autoSuggestBox)
        {
            this.Projects = projects;
            this.ProjectsType = projectsType;
            AutoSuggestBox = autoSuggestBox;
        }
    }
    public interface IGetProjectsDataManger
    {
        void GetProjects(IGetProjectsUseCaseRequest getProjectsUseCaseRequest, IUseCaseCallBackBase<GetProjectsUseCaseResponse> getProjectsUseCaseCallBack);
    }
}
