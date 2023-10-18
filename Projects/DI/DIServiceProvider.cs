using Microsoft.Extensions.DependencyInjection;
using Milestones.Domain.UseCase.MilestoneUseCase;
using Projects.Data.DataBaseAdapter;
using Projects.Data.DataManager.MilestoneDataManager;
using Projects.Data.DataManager.ProjectDataManager;
using Projects.Data.DataManager.ZTaskDataManager;
using Projects.Data.DataManager.ZTaskDataManagerAddTask;
using Projects.Data.DataManager.ZUserDataManager;
using Projects.Data.DBHandler;
using Projects.Data.FileHandler;
using Projects.Domain;
using Projects.Domain.UseCase.MilestoneUseCase;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Domain.UseCase.ZTaskUseCase;
using Projects.Domain.UseCase.ZUserUseCase;

namespace Projects.DI
{
    public class DIServiceProvider
    {
        DIServiceProvider()
        {
            Initialize();
        }
        static DIServiceProvider instance;
        public static DIServiceProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DIServiceProvider();
                }
                return instance;
            }
        }
        public ServiceProvider _ServiceProvider;
        private void Initialize()
        {
            var serviceCollection = new ServiceCollection();

           
            serviceCollection.AddSingleton<IAddTaskDataManger, AddTaskDataManager>();
            serviceCollection.AddSingleton<IGetTasksDataManger, GetTasksDataManager>();
            serviceCollection.AddSingleton<IFilterTasksDataManger, FilterTasksDataManager>();
            serviceCollection.AddSingleton<IEditTaskPropertyDataManger, EditTaskPropertyDataManager>(); 
            serviceCollection.AddSingleton<IDeleteTasksDataManger, DeleteTasksDataManager>();
            serviceCollection.AddSingleton<ITasksFileHandler, TasksFileHandler>();
            serviceCollection.AddSingleton<ITaskDBHandler, ZTaskDBHandler>();

            serviceCollection.AddSingleton<ISubTaskDBHandler, ZSubTaskDBHandler>();

            serviceCollection.AddSingleton<IGetUsersDataManger, GetUsersDataManager>();
            serviceCollection.AddSingleton<IAddUserDataManger, AddUserDataManager>();
            serviceCollection.AddSingleton<IUserDBHandler, ZUserDBHandler>();

            serviceCollection.AddSingleton<IAddMilestoneDataManger, AddMilestoneDataManager>();
            serviceCollection.AddSingleton<IEditMilestonePropertyDataManger, EditMilestonePropertyDataManager>();
            serviceCollection.AddSingleton<IGetMilestonesDataManger, GetMilestonesDataManager>();
            serviceCollection.AddSingleton<IDeleteMilestonesDataManger, DeleteMilestonesDataManager>();
            serviceCollection.AddSingleton<IMilestoneDBHandler, MilestoneDBHandler>();

            serviceCollection.AddSingleton<IGetProjectsDataManger, GetProjectsDataManager>();
            serviceCollection.AddSingleton<IAddProjectDataManger, AddProjectDataManager>();
            serviceCollection.AddSingleton<IEditProjectPropertyDataManger, EditProjectPropertyDataManager>();
            serviceCollection.AddSingleton<IDeleteProjectsDataManger, DeleteProjectsDataManager>();
            serviceCollection.AddSingleton<IProjectDBHandler, ProjectDBHandler>();

            serviceCollection.AddSingleton<IProjectAndUserConnectionDBHandler, ProjectAndUserConnectionDBHandler>();

            serviceCollection.AddSingleton<IDBAdapter, DBAdapter>();
            _ServiceProvider = serviceCollection.BuildServiceProvider();

        }
    }
}
