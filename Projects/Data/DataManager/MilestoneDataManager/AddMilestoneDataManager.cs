using Projects.Core.EntityObj;
using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ZTaskUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Domain.UseCase.MilestoneUseCase;
using Projects.Core.Entity;
using Projects.Core.AppEnum;

namespace Projects.Data.DataManager.MilestoneDataManager
{
    public class AddMilestoneDataManager : IAddMilestoneDataManger
    {
        IMilestoneDBHandler _MilestoneDBHandler;
        public AddMilestoneDataManager(IMilestoneDBHandler milestoneDBHandler)
        {
            this._MilestoneDBHandler = milestoneDBHandler;
        }

        public void AddMilestones(IAddMilestoneUseCaseRequest addMilestoneUseCaseRequest, IUseCaseCallBackBase<AddMilestoneUseCaseResponse> addMilestoneUseCaseCallBack)
        {
            try
            {
                _MilestoneDBHandler.AddMilestone(addMilestoneUseCaseRequest.Milestone);

                List<MilestoneObj> lastMilestone=_MilestoneDBHandler.GetMilestones(MilestonesType.LastMilestone,null,-1);
                if (lastMilestone.Count < 1)
                {
                    throw new Exception("Last Milestone Not Found");
                }
                addMilestoneUseCaseRequest.Milestone.ID = lastMilestone[0].ID;

                 AddMilestoneUseCaseResponse AddMilestoneUseCaseResponseUseCaseResponse = new AddMilestoneUseCaseResponse(addMilestoneUseCaseRequest.Milestone);
                addMilestoneUseCaseCallBack.OnSuccess(AddMilestoneUseCaseResponseUseCaseResponse);
            }
            catch (Exception ex)
            {
                addMilestoneUseCaseCallBack.OnError(ex.Message);
            }
        }

    }
}
