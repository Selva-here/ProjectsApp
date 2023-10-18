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

namespace Projects.Domain.UseCase.ProjectUseCase
{

    public class AddProjectUseCase : UseCaseBase<AddProjectUseCaseResponse>
    {
         IAddProjectUseCaseRequest _AddProjectUseCaseRequest;

        IPresenterCallBack<AddProjectUseCaseResponse> _AddProjectPresenterCallback;

        IAddProjectDataManger _AddProjectDataManager;
        public AddProjectUseCase(IAddProjectUseCaseRequest addProjectUseCaseRequest, IPresenterCallBack<AddProjectUseCaseResponse> addProjectPresenterCallback, CancellationToken ct) : base(ct, addProjectPresenterCallback)
        {
            _AddProjectUseCaseRequest = addProjectUseCaseRequest;
            _AddProjectPresenterCallback = addProjectPresenterCallback;
            _AddProjectDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IAddProjectDataManger>();
        }
        public override void Action()
        {

            _AddProjectDataManager.AddProjects(_AddProjectUseCaseRequest, new AddProjectUseCaseCallBack(_AddProjectPresenterCallback));
        }
        class AddProjectUseCaseCallBack : IUseCaseCallBackBase<AddProjectUseCaseResponse>
        {
            IPresenterCallBack<AddProjectUseCaseResponse> addProjectPresenterCallback;
            public AddProjectUseCaseCallBack(IPresenterCallBack<AddProjectUseCaseResponse> getUserOwnedProjectsPresenterCallback)
            {
                this.addProjectPresenterCallback = getUserOwnedProjectsPresenterCallback;
            }
            public void OnSuccess(AddProjectUseCaseResponse usecaseResponse)
            {
                addProjectPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                addProjectPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                addProjectPresenterCallback.OnError(message);
            }
        }
    }
    public interface IAddProjectUseCaseRequest
    {
        ProjectObj Project { get; set; }
    }

    public class AddProjectUseCaseRequest : IAddProjectUseCaseRequest
    {
        public ProjectObj Project { get; set; }
        public AddProjectUseCaseRequest(ProjectObj project)
        {
            Project = project;
        }
    }
    public class AddProjectUseCaseResponse
    {
        public ProjectObj Project { get; set; }
        public AddProjectUseCaseResponse(ProjectObj project)
        {
            Project = project;
        }
    }
    public interface IAddProjectDataManger
    {
        void AddProjects(IAddProjectUseCaseRequest addProjectUseCaseRequest, IUseCaseCallBackBase<AddProjectUseCaseResponse> addProjectUseCaseCallBack);
    }
}
