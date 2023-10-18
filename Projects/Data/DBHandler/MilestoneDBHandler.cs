using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DataBaseAdapter;
using Projects.DatabaseConnector;
using Projects.Domain.UseCase.MilestoneUseCase;
using Projects.Presentation.ViewModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Projects.Data.DBHandler
{
    public interface IMilestoneDBHandler
    {
        void AddMilestone(MilestoneObj milestone);
        List<MilestoneObj> GetMilestones(MilestonesType milestonesType, Object value, int projectID);
        void EditMilestoneProperty(IEditMilestonePropertyUseCaseRequest editMilestonePropertyUseCaseRequest);
        void DeleteMilestone(int milestoneID);
        void DeleteProjectMilestones(int projectID);
    }

    public class MilestoneDBHandler:IMilestoneDBHandler
    {
       
        IDBAdapter _DBAdapter;
        public MilestoneDBHandler(IDBAdapter dBAdapter)
        {
            _DBAdapter = dBAdapter;
        }

        public void AddMilestone(MilestoneObj milestone)
        {
            _DBAdapter.ExecuteQuery<Milestone>(QueryStrings.InsertMilestoneQuery, milestone.Name, milestone.ProjectID, milestone.OwnerID, milestone.CreatedUserID, milestone.StartDate, milestone.EndDate, milestone.Description);
        }
       
        public List<MilestoneObj> GetMilestones(MilestonesType milestoneType, Object value,int projectID)
        {
            switch (milestoneType)
            {
                case MilestonesType.All:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery +QueryStrings.WhereQuery+QueryStrings.UserProjectIDCheckQuery,App.AppUserID);
                case MilestonesType.Active:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.WhereQuery + QueryStrings.NotQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) "+QueryStrings.AndUserProjectIDCheckQuery, MilestoneStatus.Cancelled, MilestoneStatus.Completed, App.AppUserID);
                case MilestonesType.Completed:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.WhereQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) " + QueryStrings.AndUserProjectIDCheckQuery, MilestoneStatus.Cancelled, MilestoneStatus.Completed, App.AppUserID);
                case MilestonesType.Overdue:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.EndDateOverDueCheckQuery + QueryStrings.AndUserProjectIDCheckQuery, DateTime.Today.Date.Ticks, DateTime.MinValue.Ticks, App.AppUserID);
                case MilestonesType.DueThisWeek:
                    var lastDayOfWeek = DateTime.Today.AddDays(+(6 - (int)DateTime.Today.DayOfWeek));
                    var firstDayOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.EndDateIntervalCheckQuery + QueryStrings.AndUserProjectIDCheckQuery, firstDayOfWeek, lastDayOfWeek, App.AppUserID);
                case MilestonesType.DueThisMonth:
                    DateTime date = DateTime.Today;
                    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.EndDateIntervalCheckQuery + QueryStrings.AndUserProjectIDCheckQuery, firstDayOfMonth, lastDayOfMonth, App.AppUserID);
                case MilestonesType.MyActive:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.WhereQuery + QueryStrings.NotQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) " + QueryStrings.AndQuery + QueryStrings.OwnerIDQuery + QueryStrings.AndUserProjectIDCheckQuery, MilestoneStatus.Cancelled, MilestoneStatus.Completed, App.AppUser.Id);
                case MilestonesType.MyCompleted:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.WhereQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) " + QueryStrings.AndQuery + QueryStrings.OwnerIDQuery + QueryStrings.AndUserProjectIDCheckQuery, MilestoneStatus.Cancelled, MilestoneStatus.Completed, App.AppUser.Id);
                case MilestonesType.CreatedByMe:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.CreatedUserIDCheckQuery, App.AppUser.Id);

                case MilestonesType.Particular:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.IDCheckQuery, (int)value);
                case MilestonesType.Project:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.ProjectIDCheckQuery, (int)value);
                case MilestonesType.ProjectNameSearch:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + "WHERE Name LIKE  '%" + (string)value + "%' " + QueryStrings.AndUserProjectIDCheckQuery + QueryStrings.AndQuery + QueryStrings.ProjectIDQuery,  App.AppUserID,projectID);
                case MilestonesType.NameSearch:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + "WHERE Name LIKE  '%" + (string)value + "%' " + QueryStrings.AndUserProjectIDCheckQuery, App.AppUserID);
                case MilestonesType.LastMilestone:
                    return _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.GetMilestonesBaseQuery + QueryStrings.WhereQuery + QueryStrings.LastMilestoneIDQuery);
                default:
                    throw new NotImplementedException("Milestone Type Not Found");
            }
        }
      
        public void EditMilestoneProperty(IEditMilestonePropertyUseCaseRequest editMilestonePropertyUseCaseRequest)
        {
            _DBAdapter.ExecuteQuery<MilestoneObj>(GetEditMilestoneQuery(editMilestonePropertyUseCaseRequest.MilestonePropertyType),editMilestonePropertyUseCaseRequest.Value,editMilestonePropertyUseCaseRequest.MilestoneID);
        }
        string GetEditMilestoneQuery(MilestonePropertyEditType milestonePropertyEditType)
        {
            switch (milestonePropertyEditType)
            {
                case MilestonePropertyEditType.Name:
                    return QueryStrings.UpdateMilestoneBaseQuery + QueryStrings.NameQuery + QueryStrings.IDCheckQuery;
                case MilestonePropertyEditType.Status:
                    return QueryStrings.UpdateMilestoneBaseQuery + QueryStrings.StatusQuery + QueryStrings.IDCheckQuery;
                case MilestonePropertyEditType.StartDate:
                    return QueryStrings.UpdateMilestoneBaseQuery + QueryStrings.StartDateQuery + QueryStrings.IDCheckQuery;
                case MilestonePropertyEditType.EndDate:
                    return QueryStrings.UpdateMilestoneBaseQuery + QueryStrings.EndDateQuery + QueryStrings.IDCheckQuery;
                case MilestonePropertyEditType.OwnerID:
                    return QueryStrings.UpdateMilestoneBaseQuery + QueryStrings.OwnerIDQuery + QueryStrings.IDCheckQuery;
                case MilestonePropertyEditType.Description:
                    return QueryStrings.UpdateMilestoneBaseQuery + QueryStrings.DescriptionQuery + QueryStrings.IDCheckQuery;
                default:
                    throw new NotImplementedException("MilestonePropertyEditType Not Found");
            }
        }

        public void DeleteMilestone(int milestoneID)
        {
            _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.DeleteMilestoneQuery, milestoneID);
        }
        public void DeleteProjectMilestones(int projectID)
        {
            _DBAdapter.ExecuteQuery<MilestoneObj>(QueryStrings.DeleteProjectMilestonesQuery,projectID);
        }
    }
}
