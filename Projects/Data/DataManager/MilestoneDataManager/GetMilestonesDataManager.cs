using Projects.Data.DBHandler;
using Projects.Domain.UseCase.MilestoneUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Projects.Core.EntityObj;
using Projects.Core.Entity;
using Windows.System;

namespace Projects.Data.DataManager.MilestoneDataManager
{
    public class GetMilestonesDataManager : IGetMilestonesDataManger
    {
        IMilestoneDBHandler _MilestoneDBHandler;
        IUserDBHandler _UserDBHandler;
        IProjectDBHandler _ProjectDBHandler;
        public GetMilestonesDataManager(IMilestoneDBHandler milestoneDBHandler, IUserDBHandler userDBHandler,IProjectDBHandler projectDBHandler)
        {
            _MilestoneDBHandler = milestoneDBHandler;
            _UserDBHandler = userDBHandler;
            _ProjectDBHandler = projectDBHandler;
        }
        public void GetMilestonesType(IGetMilestonesUseCaseRequest getMilestonesByTypeUseCaseRequest, IUseCaseCallBackBase<GetMilestonesUseCaseResponse> getMilestonesByTypeUseCaseCallBack)
        {
            List<MilestoneObj> fetchedMilestones= _MilestoneDBHandler.GetMilestones(getMilestonesByTypeUseCaseRequest.MilestonesType,getMilestonesByTypeUseCaseRequest.Value,getMilestonesByTypeUseCaseRequest.ProjectID);
            foreach (MilestoneObj milestone in fetchedMilestones)
            {

                List<ProjectObj> fetchedProjects = _ProjectDBHandler.GetProjectByID(milestone.ProjectID);
                if (fetchedProjects.Count > 0)
                {
                    milestone.Project = fetchedProjects.First();
                }
                List<ZUser> fetchedOwners = _UserDBHandler.GetUser(milestone.OwnerID);
                if (fetchedOwners.Count > 0)
                {
                    milestone.Owner = fetchedOwners.First();
                }

                List<ZUser> fetchedCreatedUsers = _UserDBHandler.GetUser(milestone.CreatedUserID);
                if (fetchedCreatedUsers.Count > 0)
                {
                    milestone.CreatedUser = fetchedOwners.First();
                }
                else
                {
                    throw new Exception("Created User not fetched");
                }

            }
            GetMilestonesUseCaseResponse getMilestonesByTypeUseCaseResponse = new GetMilestonesUseCaseResponse(fetchedMilestones,getMilestonesByTypeUseCaseRequest.MilestonesType,getMilestonesByTypeUseCaseRequest.AutoSuggestBox);
            getMilestonesByTypeUseCaseCallBack.OnSuccess(getMilestonesByTypeUseCaseResponse);
        }
    }
}
