using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Domain.UseCase.MilestoneUseCase;

namespace Projects.Data.DataManager.ProjectDataManager
{
    public class AddProjectDataManager: IAddProjectDataManger
    {
        IProjectDBHandler _ProjectDBHandler;
        IProjectAndUserConnectionDBHandler _ProjectAndUserConnectionDBHandler;
        public AddProjectDataManager(IProjectDBHandler projectDBHandler, IProjectAndUserConnectionDBHandler projectAndUserConnectionDBHandler)
        {
            this._ProjectDBHandler = projectDBHandler;
            this._ProjectAndUserConnectionDBHandler = projectAndUserConnectionDBHandler;
        }

        public void AddProjects(IAddProjectUseCaseRequest addProjectUseCaseRequest, IUseCaseCallBackBase<AddProjectUseCaseResponse> addProjectUseCaseCallBack)
        {
           
            _ProjectDBHandler.AddProject(addProjectUseCaseRequest.Project);

            List<ProjectObj> lastProject = _ProjectDBHandler.GetProjects(Core.AppEnum.ProjectsType.LastProject,null);
            if (lastProject.Count < 1)
            {
                throw new Exception("Last Project Not Found");
            }
            addProjectUseCaseRequest.Project.ID = lastProject[0].ID;

            _ProjectAndUserConnectionDBHandler.AddProjectUsers(addProjectUseCaseRequest.Project);

            AddProjectUseCaseResponse addProjectUseCaseResponseUseCaseResponse = new AddProjectUseCaseResponse(addProjectUseCaseRequest.Project);
            addProjectUseCaseCallBack.OnSuccess(addProjectUseCaseResponseUseCaseResponse);
        }
       
    }
}
