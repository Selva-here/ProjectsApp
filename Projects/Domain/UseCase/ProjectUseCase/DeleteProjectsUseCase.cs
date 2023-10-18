using Microsoft.Extensions.DependencyInjection;
using Projects.Core.EntityObj;
using Projects.DI;
using Projects.Presentation;
using System.Collections.Generic;
using System.Threading;
namespace Projects.Domain.UseCase.ProjectUseCase
{
    public class DeleteProjectsUseCase : UseCaseBase<DeleteProjectUseCaseResponse>
    {
        IDeleteProjectsUseCaseRequest _DeleteProjectsUseCaseRequest;

        IPresenterCallBack<DeleteProjectUseCaseResponse> _DeleteProjectsPresenterCallback;

        IDeleteProjectsDataManger _DeleteProjectsDataManager;
        public DeleteProjectsUseCase(IDeleteProjectsUseCaseRequest deleteProjectsUseCaseRequest, IPresenterCallBack<DeleteProjectUseCaseResponse> deleteProjectsPresenterCallback, CancellationToken ct) : base(ct, deleteProjectsPresenterCallback)
        {
            _DeleteProjectsUseCaseRequest = deleteProjectsUseCaseRequest;
            _DeleteProjectsPresenterCallback = deleteProjectsPresenterCallback;
            _DeleteProjectsDataManager = DIServiceProvider.Instance._ServiceProvider.GetService<IDeleteProjectsDataManger>();
        }
        public override void Action()
        {
            _DeleteProjectsDataManager.DeleteProjects(_DeleteProjectsUseCaseRequest, new DeleteProjectsUseCaseCallBack(_DeleteProjectsPresenterCallback));
        }
        class DeleteProjectsUseCaseCallBack : IUseCaseCallBackBase<DeleteProjectUseCaseResponse>
        {
            IPresenterCallBack<DeleteProjectUseCaseResponse> deleteProjectPresenterCallback;
            public DeleteProjectsUseCaseCallBack(IPresenterCallBack<DeleteProjectUseCaseResponse> getUserOwnedProjectsPresenterCallback)
            {
                this.deleteProjectPresenterCallback = getUserOwnedProjectsPresenterCallback;
            }
            public void OnSuccess(DeleteProjectUseCaseResponse usecaseResponse)
            {
                deleteProjectPresenterCallback.OnSuccess(usecaseResponse);
            }
            public void OnCancel()
            {
                deleteProjectPresenterCallback.OnCancel();
            }

            public void OnError(string message)
            {
                deleteProjectPresenterCallback.OnError(message);
            }
        }
    }
    public interface IDeleteProjectsUseCaseRequest
    {
        IList<ProjectObj> Projects { get; set; }
    }

    public class DeleteProjectsUseCaseRequest : IDeleteProjectsUseCaseRequest
    {
        public IList<ProjectObj> Projects { get; set; }
        public DeleteProjectsUseCaseRequest(IList<ProjectObj> project)
        {
            Projects = project;
        }
    }
    public class DeleteProjectUseCaseResponse
    {
        public DeleteProjectUseCaseResponse()
        {
        }
    }
    public interface IDeleteProjectsDataManger
    {
        void DeleteProjects(IDeleteProjectsUseCaseRequest deleteProjectsUseCaseRequest, IUseCaseCallBackBase<DeleteProjectUseCaseResponse> deleteProjectsUseCaseCallBack);
    }
}
