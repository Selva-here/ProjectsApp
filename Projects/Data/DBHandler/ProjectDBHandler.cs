using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DataBaseAdapter;
using Projects.DatabaseConnector;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Presentation.ViewModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Projects.Data.DBHandler
{
    public interface IProjectDBHandler
    {
        void AddProject(ProjectObj project);
        void EditProjectProperty(IEditProjectPropertyUseCaseRequest editProjectPropertyUseCaseRequest);
        List<ProjectObj> GetProjects(ProjectsType projectsType, object value);
        List<ProjectObj> GetProjectByID(int ProjectID);
        void DeleteProject(int projectID);
    }
    public class ProjectDBHandler : IProjectDBHandler
    {
       
        IDBAdapter _DBAdapter;
        public ProjectDBHandler(IDBAdapter dBAdapter)
        {
            _DBAdapter = dBAdapter;
        }
        public void AddProject(ProjectObj projectObj)
        {
            _DBAdapter.ExecuteQuery<Project>(QueryStrings.InsertProjectQuery, projectObj.Name, projectObj.OwnerID, projectObj.CreatedUserID, projectObj.StartDate, projectObj.EndDate, projectObj.Description);

        }
        
        public List<ProjectObj> GetProjects(ProjectsType projectsType,object value)
        {
            switch (projectsType)
            {
                case ProjectsType.All:
                    return _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.GetProjectsBaseQuery+QueryStrings.WhereQuery+ QueryStrings.UserProjectCheckQuery,App.AppUserID);
                case ProjectsType.Active:
                    return _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.GetProjectsBaseQuery + QueryStrings.WhereQuery + QueryStrings.NotQuery +QueryStrings.OpenParenthesisQuery + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + QueryStrings.CloseParenthesisQuery+QueryStrings.AndUserProjectCheckQuery, ProjectStatus.Completed, ProjectStatus.Cancelled,App.AppUserID);
                case ProjectsType.Completed:
                    return _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.GetProjectsBaseQuery + QueryStrings.WhereQuery + QueryStrings.OpenParenthesisQuery + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + QueryStrings.CloseParenthesisQuery + QueryStrings.AndUserProjectCheckQuery, ProjectStatus.Completed, ProjectStatus.Cancelled, App.AppUserID);
                case ProjectsType.Particular:
                    return _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.GetProjectsBaseQuery + QueryStrings.IDCheckQuery + QueryStrings.AndUserProjectCheckQuery, (int)value, App.AppUserID);
                case ProjectsType.NameSearch:
                    return _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.GetProjectsBaseQuery + "WHERE Name LIKE  '%" + (string)value + "%' " + QueryStrings.AndUserProjectCheckQuery, App.AppUserID);
                case ProjectsType.LastProject:
                    return _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.GetProjectsBaseQuery + QueryStrings.WhereQuery + QueryStrings.LastProjectIDQuery);
                default:
                    throw new Exception("ProjectsType Enum value Not found.");
            }
        }
        public List<ProjectObj> GetProjectByID(int ProjectID)
        {
            return _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.GetProjectsBaseQuery + QueryStrings.IDCheckQuery,ProjectID);
        }

        public void EditProjectProperty(IEditProjectPropertyUseCaseRequest editProjectPropertyUseCaseRequest)
        {
            switch (editProjectPropertyUseCaseRequest.ProjectPropertyType)
            {
                case ProjectPropertyEditType.Name:
                    _DBAdapter.ExecuteQuery<Project>(QueryStrings.UpdateProjectBaseQuery + QueryStrings.NameQuery + QueryStrings.IDCheckQuery, editProjectPropertyUseCaseRequest.Value, editProjectPropertyUseCaseRequest.ProjectID);
                    break;
                case ProjectPropertyEditType.Status:
                    _DBAdapter.ExecuteQuery<Project>(QueryStrings.UpdateProjectBaseQuery + QueryStrings.StatusQuery + QueryStrings.IDCheckQuery, editProjectPropertyUseCaseRequest.Value, editProjectPropertyUseCaseRequest.ProjectID);
                    break;
                case ProjectPropertyEditType.StartDate:
                    _DBAdapter.ExecuteQuery<Project>(QueryStrings.UpdateProjectBaseQuery + QueryStrings.StartDateQuery + QueryStrings.IDCheckQuery, editProjectPropertyUseCaseRequest.Value, editProjectPropertyUseCaseRequest.ProjectID);
                    break;
                case ProjectPropertyEditType.EndDate:
                    _DBAdapter.ExecuteQuery<Project>(QueryStrings.UpdateProjectBaseQuery + QueryStrings.EndDateQuery + QueryStrings.IDCheckQuery, editProjectPropertyUseCaseRequest.Value, editProjectPropertyUseCaseRequest.ProjectID);
                    break;
                case ProjectPropertyEditType.OwnerID:
                    _DBAdapter.ExecuteQuery<Project>(QueryStrings.UpdateProjectBaseQuery + QueryStrings.OwnerIDQuery + QueryStrings.IDCheckQuery, editProjectPropertyUseCaseRequest.Value, editProjectPropertyUseCaseRequest.ProjectID);
                    break;
                case ProjectPropertyEditType.Description:
                    _DBAdapter.ExecuteQuery<Project>(QueryStrings.UpdateProjectBaseQuery + QueryStrings.DescriptionQuery + QueryStrings.IDCheckQuery, editProjectPropertyUseCaseRequest.Value, editProjectPropertyUseCaseRequest.ProjectID);
                    break;
                default:
                    throw new Exception("ProjectPropertyEditType Enum value Not found.");
            }
        }
        public void DeleteProject(int projectID)
        {
            _DBAdapter.ExecuteQuery<ProjectObj>(QueryStrings.DeleteProjectQuery, projectID);
        }
       
    }
}
