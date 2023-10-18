using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DataManager.ZTaskDataManager;
using Projects.Data.DataParser;
using Projects.Data.DBHandler;
using Projects.Data.FileHandler;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Projects.Data.DataManager.ZTaskDataManager
{
    public class GetTasksDataManager : IGetTasksDataManger
    {
        ITaskDBHandler _TaskDBHandler;
        ISubTaskDBHandler _SubTaskDBHandler;
        IProjectDBHandler _ProjectDBHandler;
        IMilestoneDBHandler _MilestoneDBHandler;
        IUserDBHandler _UserDBHandler;
        ITasksFileHandler _TasksFileHandler;
        public GetTasksDataManager(ITaskDBHandler taskDBHandler, ISubTaskDBHandler subTaskDBHandler, IProjectDBHandler projectDBHandler,IMilestoneDBHandler milestoneDBHandler,IUserDBHandler userDBHandler, ITasksFileHandler tasksFileHandler)
        {
            _TaskDBHandler = taskDBHandler;
            _ProjectDBHandler = projectDBHandler;
            _MilestoneDBHandler= milestoneDBHandler;
            _UserDBHandler= userDBHandler;
            _SubTaskDBHandler = subTaskDBHandler;
            _TasksFileHandler = tasksFileHandler;
        }
        public async void GetTasks(IGetTasksUseCaseRequest getTasksUseCaseRequest, IUseCaseCallBackBase<GetTasksUseCaseResponse> getTasksUseCaseCallBack)
        {
            try
            {
                TasksLoaded( _TaskDBHandler.GetTasks(getTasksUseCaseRequest.TasksType,getTasksUseCaseRequest.Value));

                string jsonString = await _TasksFileHandler.GetDataFromFile();
                List<ZTaskObj> fetchedTasksFromServer = TaskDataParser.ParseForumPostData(jsonString);
                _TaskDBHandler.UpdateTasks(fetchedTasksFromServer);
                TasksLoaded(fetchedTasksFromServer);

                void TasksLoaded(List<ZTaskObj> fetchedTasks)
                {
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
                    GetTasksUseCaseResponse getTasksUseCaseResponse = new GetTasksUseCaseResponse(fetchedTasks, getTasksUseCaseRequest.TasksType);
                    getTasksUseCaseCallBack.OnSuccess(getTasksUseCaseResponse);
                }
            }
            catch(Exception e)
            {
                getTasksUseCaseCallBack.OnError(e.Message);
            }
        }
    }
}
