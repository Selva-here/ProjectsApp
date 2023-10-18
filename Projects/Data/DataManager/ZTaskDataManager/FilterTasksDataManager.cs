using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DBHandler;
using Projects.Domain;
using Projects.Domain.UseCase.ZTaskUseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Data.DataManager.ZTaskDataManager
{

    public class FilterTasksDataManager : IFilterTasksDataManger
    {
        ITaskDBHandler _TaskDBHandler;
        ISubTaskDBHandler _SubTaskDBHandler;
        IProjectDBHandler _ProjectDBHandler;
        IMilestoneDBHandler _MilestoneDBHandler;
        IUserDBHandler _UserDBHandler;

        public FilterTasksDataManager(ITaskDBHandler taskDBHandler, ISubTaskDBHandler subTaskDBHandler, IProjectDBHandler projectDBHandler, IMilestoneDBHandler milestoneDBHandler, IUserDBHandler userDBHandler)
        {
            _TaskDBHandler = taskDBHandler;
            _ProjectDBHandler = projectDBHandler;
            _MilestoneDBHandler = milestoneDBHandler;
            _UserDBHandler = userDBHandler;
            _SubTaskDBHandler = subTaskDBHandler;
        }
        public void FilterTasks(IFilterTasksUseCaseRequest filterTasksUseCaseRequest, IUseCaseCallBackBase<FilterTasksUseCaseResponse> filterTasksUseCaseCallBack)
        {
            try
            {
                List<ZTaskObj> fetchedTasks = _TaskDBHandler.GetFilteredTasks(filterTasksUseCaseRequest.AppliedFilterMethod,filterTasksUseCaseRequest.AppliedFilterPropertyAndValueList);
                foreach (ZTaskObj task in fetchedTasks)
                {
                    List<ProjectObj> fetchedProjects = _ProjectDBHandler.GetProjectByID(task.ProjectID);
                    if (fetchedProjects.Count > 0)
                    {
                        task.Project = fetchedProjects.First();
                    }

                    List<MilestoneObj> fetchedMilestones = _MilestoneDBHandler.GetMilestones(MilestonesType.Particular, task.MilestoneID, -1);
                    if (fetchedMilestones.Count > 0)
                    {
                        task.Milestone = fetchedMilestones.First();
                    }

                    List<ZUser> fetchedOwners = _UserDBHandler.GetUser(task.OwnerID);
                    if (fetchedOwners.Count > 0)
                    {
                        task.Owner = fetchedOwners.First();
                    }

                    List<ZUser> fetchedCreatedUsers = _UserDBHandler.GetUser(task.CreatedUserID);
                    if (fetchedCreatedUsers.Count > 0)
                    {
                        task.CreatedUser = fetchedOwners.First();
                    }
                    else
                    {
                        throw new Exception("Assigned User not fetched for Task Id " + task.ID);
                    }
                    foreach (ZSubTask subTask in _SubTaskDBHandler.GetTaskSubTasks(task.ID))
                    {
                        task.SubTaskCollection.Add(subTask);
                    }

                }
                FilterTasksUseCaseResponse filterTasksUseCaseResponse = new FilterTasksUseCaseResponse(fetchedTasks);
                filterTasksUseCaseCallBack.OnSuccess(filterTasksUseCaseResponse);
            }
            catch (Exception e)
            {
                filterTasksUseCaseCallBack.OnError(e.Message);
            }
        }
    }
}
