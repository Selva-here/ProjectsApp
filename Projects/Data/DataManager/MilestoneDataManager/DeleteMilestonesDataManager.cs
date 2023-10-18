
using Milestones.Domain.UseCase.MilestoneUseCase;
using Projects.Core.Entity;
using Projects.Data.DBHandler;
using Projects.Domain;
using System;
namespace Projects.Data.DataManager.MilestoneDataManager
{
    public class DeleteMilestonesDataManager : IDeleteMilestonesDataManger
    {
        IMilestoneDBHandler _MilestoneDBHandler;
        ITaskDBHandler _TaskDBHandler;
        public DeleteMilestonesDataManager(IMilestoneDBHandler milestoneDBHandler, ITaskDBHandler taskDBHandler)
        {
            _MilestoneDBHandler = milestoneDBHandler;
            _TaskDBHandler = taskDBHandler;
        }

        public void DeleteMilestones(IDeleteMilestonesUseCaseRequest deleteMilestoneUseCaseRequest, IUseCaseCallBackBase<DeleteMilestoneUseCaseResponse> deleteMilestoneUseCaseCallBack)
        {

            try
            {
                foreach(Milestone milestone in deleteMilestoneUseCaseRequest.Milestones)
                {
                    _MilestoneDBHandler.DeleteMilestone(milestone.ID);
                    _TaskDBHandler.DeleteMilestoneTasks(milestone.ID);
                }
               
                DeleteMilestoneUseCaseResponse deleteMilestoneUseCaseResponseUseCaseResponse = new DeleteMilestoneUseCaseResponse();
                deleteMilestoneUseCaseCallBack.OnSuccess(deleteMilestoneUseCaseResponseUseCaseResponse);
            }
            catch (Exception ex)
            {
                deleteMilestoneUseCaseCallBack.OnError(ex.Message);
            }
        }

    }
}

