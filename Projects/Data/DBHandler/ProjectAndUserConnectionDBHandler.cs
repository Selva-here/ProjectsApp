using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DataBaseAdapter;
using Projects.DatabaseConnector;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Projects.Data.DBHandler
{
    public interface IProjectAndUserConnectionDBHandler
    {
        void AddProjectUsers(ProjectObj project);
        void AddProjectUser(int projectID,int userID);
        void DeleteProjectUser(int projectID, int userID);
        void DeleteProjectUsers(int projectID);
    }
    public class ProjectAndUserConnectionDBHandler : IProjectAndUserConnectionDBHandler
    {
        IDBAdapter _DBAdapter;
        public ProjectAndUserConnectionDBHandler(IDBAdapter dBAdapter)
        {
            _DBAdapter = dBAdapter;
        }

        public void AddProjectUsers(ProjectObj project)
        {
            foreach (ZUser user in project.UserCollection)
            {
                _DBAdapter.ExecuteQuery<ProjectAndUserConnection>(QueryStrings.InsertProjectUserQuery, project.ID, user.Id);
            }
        }
        public void DeleteProjectUsers(int projectID)
        {
            _DBAdapter.ExecuteQuery<ProjectAndUserConnection>(QueryStrings.DeleteProjectUsersQuery, projectID);
        }
        public void AddProjectUser(int projectID, int userID)
        {
            _DBAdapter.ExecuteQuery<ProjectAndUserConnection>(QueryStrings.InsertProjectUserQuery, projectID, userID);
        }
        public void DeleteProjectUser(int projectID, int userID)
        {
            _DBAdapter.ExecuteQuery<ProjectAndUserConnection>(QueryStrings.DeleteProjectUserQuery, projectID, userID);
        }
    }
}
