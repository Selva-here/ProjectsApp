using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Windows.Web.Http;
using System.Linq;

namespace Projects.Data.DataManager.ProjectDataManager
{
    public class GetProjectsDataManager : IGetProjectsDataManger
    {
        IProjectDBHandler _ProjectDBHandler;
        IUserDBHandler _UserDBHandler;
        public GetProjectsDataManager(IProjectDBHandler projectDBHandler, IUserDBHandler userDBHandler, IMilestoneDBHandler milestoneDBHandler)
        {
            _ProjectDBHandler = projectDBHandler;
            _UserDBHandler = userDBHandler;
        }
        public void GetProjects(IGetProjectsUseCaseRequest getProjectsUseCaseRequest, IUseCaseCallBackBase<GetProjectsUseCaseResponse> getProjectsUseCaseCallBack)
        {
            try
            {
                List<ProjectObj> fetchedProjects = _ProjectDBHandler.GetProjects(getProjectsUseCaseRequest.ProjectsType, getProjectsUseCaseRequest.Value);
                foreach (ProjectObj project in fetchedProjects)
                {
                    List<ZUser> fetchedUsers = _UserDBHandler.GetProjectUsers(project.ID);
                    foreach (ZUser user in fetchedUsers)
                    {
                        project.UserCollection.Add(user);
                    }
                    List<ZUser> fetchedOwners = _UserDBHandler.GetUser(project.OwnerID);
                    if (fetchedOwners.Count > 0)
                    {
                        project.Owner = fetchedOwners.First();
                    }
                    else
                    {
                        throw new Exception("Project Owner not Found");
                    }
                    List<ZUser> fetchedCreatedUser = _UserDBHandler.GetUser(project.CreatedUserID);
                    if (fetchedOwners.Count > 0)
                    {
                        project.CreatedUser = fetchedCreatedUser.First();
                    }
                    else
                    {
                        throw new Exception("Project Created User not Found");
                    }
                }

                GetProjectsUseCaseResponse getProjectsUseCaseResponse = new GetProjectsUseCaseResponse(fetchedProjects,getProjectsUseCaseRequest.ProjectsType, getProjectsUseCaseRequest.AutoSuggestBox);
                getProjectsUseCaseCallBack.OnSuccess(getProjectsUseCaseResponse);
            }
            catch(Exception ex)
            {
                getProjectsUseCaseCallBack.OnError(ex.Message);
            }
        }
    }
}
