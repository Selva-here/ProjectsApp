
using Projects.Core.Entity;
using Projects.Data.DBHandler;
using Projects.Domain;
using Projects.Domain.UseCase.ProjectUseCase;
using System;

namespace Projects.Data.DataManager.ProjectDataManager
{
    public class DeleteProjectsDataManager : IDeleteProjectsDataManger
    {
        IProjectDBHandler _ProjectDBHandler;
        IMilestoneDBHandler _MilestoneDBHandler;
        ITaskDBHandler _TaskDBHandler;
        IProjectAndUserConnectionDBHandler _ProjectAndUserConnectionDBHandler;
        public DeleteProjectsDataManager(IProjectDBHandler projectDBHandler, IMilestoneDBHandler milestoneDBHandler, ITaskDBHandler taskDBHandler, IProjectAndUserConnectionDBHandler projectAndUserConnectionDBHandler)
        {
            _ProjectDBHandler = projectDBHandler;
            _MilestoneDBHandler = milestoneDBHandler;
            _TaskDBHandler = taskDBHandler;
            _ProjectAndUserConnectionDBHandler = projectAndUserConnectionDBHandler;
        }

        public void DeleteProjects(IDeleteProjectsUseCaseRequest deleteProjectUseCaseRequest, IUseCaseCallBackBase<DeleteProjectUseCaseResponse> deleteProjectUseCaseCallBack)
        {

            try
            {
                foreach (Project project in deleteProjectUseCaseRequest.Projects)
                {
                    _ProjectDBHandler.DeleteProject(project.ID);
                    _MilestoneDBHandler.DeleteProjectMilestones(project.ID);
                    _TaskDBHandler.DeleteProjectTasks(project.ID);
                    _ProjectAndUserConnectionDBHandler.DeleteProjectUsers(project.ID);
                }
               
                DeleteProjectUseCaseResponse deleteProjectUseCaseResponseUseCaseResponse = new DeleteProjectUseCaseResponse();
                deleteProjectUseCaseCallBack.OnSuccess(deleteProjectUseCaseResponseUseCaseResponse);
            }
            catch (Exception ex)
            {
                deleteProjectUseCaseCallBack.OnError(ex.Message);
            }
        }

    }
}
