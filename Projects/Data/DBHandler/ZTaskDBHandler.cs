using Projects.Core.Entity;
using Projects.Core.AppEnum;
using System;
using System.Collections.Generic;
using Projects.Core.EntityObj;
using Projects.Data.DataBaseAdapter;
using Projects.Domain;
using Projects.Domain.UseCase.ZTaskUseCase;
using System.Collections;
using System.Data.Common;
using ZohoProjects;
using System.Threading.Tasks;
using Projects.Presentation.View.AppUserControl;
using Windows.UI.Xaml.Media;

namespace Projects.Data.DBHandler
{
    public interface ITaskDBHandler
    {
        void AddTask(ZTask task);
        void EditTaskProperty(IEditTaskPropertyUseCaseRequest editTaskPropertyUseCaseRequest);
        List<ZTaskObj> GetTasks(TasksType tasksType, object value);

        void DeleteTask(int taskID);
        void DeleteMilestoneTasks(int milestoneID);
        void DeleteProjectTasks(int projectID);

        List<ZTaskObj> GetFilteredTasks(FilterMethod filterMethod, List<FilterPropertyAndValue> filterPropertyAndValueList);
        void UpdateTasks(List<ZTaskObj> fetchedTasksFromServer);
    }
    public class ZTaskDBHandler : ITaskDBHandler
    {

        IDBAdapter _DBAdapter;
        public ZTaskDBHandler(IDBAdapter dBAdapter)
        {
            _DBAdapter = dBAdapter;
        }
        public void AddTask(ZTask task)
        {
            _DBAdapter.ExecuteQuery<ZTask>(QueryStrings.InsertTaskQuery, task.Name, task.MilestoneID, task.ProjectID, task.OwnerID, task.CreatedUserID, task.StartDate, task.EndDate, task.Description);
        }
       
        public List<ZTaskObj> GetTasks(TasksType tasksType,object value)
        {
            switch (tasksType)
            {

                case TasksType.All:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.WhereQuery + QueryStrings.UserProjectIDCheckQuery, App.AppUserID);
                case TasksType.Open:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.WhereQuery + QueryStrings.NotQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) "+QueryStrings.AndUserProjectIDCheckQuery, ZTaskStatus.Cancelled, ZTaskStatus.Closed, App.AppUserID);
                case TasksType.Closed:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.WhereQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) " + QueryStrings.AndUserProjectIDCheckQuery, ZTaskStatus.Cancelled, ZTaskStatus.Closed, App.AppUserID);
                case TasksType.Overdue:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.DueDateOverDueCheckQuery + QueryStrings.AndUserProjectIDCheckQuery, DateTime.Today.Date.Ticks, App.AppUserID);
                case TasksType.DueToday:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.DueDateCheckQuery + QueryStrings.AndUserProjectIDCheckQuery, DateTime.Today.Date.Ticks, App.AppUserID);
               
                case TasksType.MyOpen:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.WhereQuery + QueryStrings.NotQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) " + QueryStrings.AndQuery + QueryStrings.OwnerIDQuery, ZTaskStatus.Cancelled, ZTaskStatus.Closed, App.AppUserID);
                case TasksType.MyClosed:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.WhereQuery + " ( " + QueryStrings.StatusQuery + QueryStrings.OrQuery + QueryStrings.StatusQuery + " ) " + QueryStrings.AndQuery + QueryStrings.OwnerIDQuery, ZTaskStatus.Cancelled, ZTaskStatus.Closed, App.AppUserID);
                case TasksType.MyOverdue:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.DueDateOverDueCheckQuery + QueryStrings.AndQuery + QueryStrings.IDQuery, DateTime.Today.Date.Ticks, App.AppUserID);
                case TasksType.TodayAssigned:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.StartDateCheckQuery + QueryStrings.AndUserProjectIDCheckQuery, DateTime.Today.Date.Ticks, App.AppUserID);
                case TasksType.CreatedByMe:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.CreatedUserIDCheckQuery, App.AppUserID);

                case TasksType.Project:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.ProjectIDCheckQuery, (int)value);
                case TasksType.Milestone:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetTasksBaseQuery + QueryStrings.MilestoneIDCheckQuery, (int)value);
                case TasksType.LastTask:
                    return _DBAdapter.ExecuteQuery<ZTaskObj>(QueryStrings.GetLastTaskQuery);
                default:
                    throw new Exception("Invalid TasksType is given in TaskDBHandler");
            }
        }
        public void EditTaskProperty(IEditTaskPropertyUseCaseRequest editTaskPropertyUseCaseRequest)
        {
            _DBAdapter.ExecuteQuery<ZTask>(GetEditTaskQuery(editTaskPropertyUseCaseRequest.TaskPropertyEditType), editTaskPropertyUseCaseRequest.Value, editTaskPropertyUseCaseRequest.TaskID);
        }
        string GetEditTaskQuery(TaskPropertyEditType propertyType)
        {
            switch (propertyType)
            {
                case TaskPropertyEditType.Name:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.NameQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.Status:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.StatusQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.Priority:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.PriorityQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.MilestoneID:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.MilestoneIDQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.ProjectID:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.ProjectIDQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.StartDate:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.StartDateQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.Percentage:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.PercentageQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.EndDate:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.DueDateQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.OwnerID:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.OwnerIDQuery + QueryStrings.IDCheckQuery;
                case TaskPropertyEditType.Description:
                    return QueryStrings.UpdateTaskBaseQuery + QueryStrings.DescriptionQuery + QueryStrings.IDCheckQuery;
                default:
                    throw new NotImplementedException();
            }
        }
        public void UpdateTasks(List<ZTaskObj> fetchedTasksFromServer)
        {

        }
        public void DeleteTask(int taskID)
        {
            _DBAdapter.ExecuteQuery<ZTask>(QueryStrings.DeleteTaskQuery, taskID);
        }
       
        public void DeleteMilestoneTasks(int milestoneID)
        {
            _DBAdapter.ExecuteQuery<ZTask>(QueryStrings.DeleteMilestoneTaskQuery, milestoneID);
        }
        public void DeleteProjectTasks(int projectID)
        {
            _DBAdapter.ExecuteQuery<ZTask>(QueryStrings.DeleteProjectTaskQuery, projectID);
        }
       
        public List<ZTaskObj> GetFilteredTasks(FilterMethod filterMethod, List<FilterPropertyAndValue> filterPropertyAndValueList)
        {
            string query = QueryStrings.GetTasksQuery + QueryStrings.AndQuery + QueryStrings.OpenParenthesisQuery;

            string filterMethodQuery = " "+ QueryStrings.OrQuery+ " ";
            if (filterMethod == FilterMethod.All)
            {
                filterMethodQuery = " " + QueryStrings.AndQuery + " ";
            }
            bool isFirstFilterQueryApplied = false;
            foreach (FilterPropertyAndValue filterPropertyAndValue in filterPropertyAndValueList)
            {
                switch (filterPropertyAndValue.Property)
                {
                    case FilterProperty.Owner:
                        if (filterPropertyAndValue.Value != null)
                        {
                            string ownerIDQuery= $" OwnerID = {filterPropertyAndValue.Value} ";
                            if ((OwnerFilterMethod)filterPropertyAndValue.Method == OwnerFilterMethod.Is)
                                query += ownerIDQuery;
                            else if ((OwnerFilterMethod)filterPropertyAndValue.Method == OwnerFilterMethod.IsNot)
                                query += QueryStrings.NotQuery + ownerIDQuery;

                            isFirstFilterQueryApplied = true;
                        }
                        break;
                    case FilterProperty.Priority:
                        if ( filterPropertyAndValue.Value != null)
                        {
                            string priorityQuery= $" Priority = {(int)(Priority)filterPropertyAndValue.Value} "; 
                            if (isFirstFilterQueryApplied == false)
                            {
                                query += priorityQuery;
                                isFirstFilterQueryApplied = true;
                            }
                            else
                            {
                                query += filterMethodQuery + priorityQuery;
                            }
                        }
                        break;
                    case FilterProperty.Status:
                        if (filterPropertyAndValue.Method != null && filterPropertyAndValue.Value != null)
                        {
                            string statusQuery= $" Status = {(int)(ZTaskStatus)filterPropertyAndValue.Value}";
                            if (isFirstFilterQueryApplied == false)
                            {
                                query += statusQuery;
                                isFirstFilterQueryApplied = true;
                            }
                            else
                            {
                                query += filterMethodQuery + statusQuery;
                            }
                        }
                        break;
                    case FilterProperty.Percentage:
                        if (filterPropertyAndValue.Method != null && filterPropertyAndValue.Value != null)
                        {
                            string percentageQuery = "";
                            PercentageFilterMethod selectedPercentageFilterMethod=(PercentageFilterMethod)filterPropertyAndValue.Method;
                            switch (selectedPercentageFilterMethod)
                            {
                                case PercentageFilterMethod.LessThan:
                                    percentageQuery = $" Percentage < {(int)filterPropertyAndValue.Value}";
                                    break;
                                case PercentageFilterMethod.GreaterThan:
                                    percentageQuery = $" Percentage > {(int)filterPropertyAndValue.Value}";
                                    break;
                                case PercentageFilterMethod.Equal:
                                    percentageQuery = $" Percentage = {(int)filterPropertyAndValue.Value}";
                                    break;
                            }

                            if (isFirstFilterQueryApplied == false)
                            {
                                query += percentageQuery;
                                isFirstFilterQueryApplied = true;
                            }
                            else
                            {
                                query += filterMethodQuery + percentageQuery;
                            }
                        }
                        break;
                    case FilterProperty.StartDate:
                        if (filterPropertyAndValue.Method != null )
                        {
                            string startDateQuery = "";
                            DateFilterMethod selectedDateFilterMethod = (DateFilterMethod)filterPropertyAndValue.Method;
                            DateTime date=(DateTime) filterPropertyAndValue.Value;
                            switch (selectedDateFilterMethod)
                            {
                                case DateFilterMethod.Today:
                                    startDateQuery = $" StartDate = {DateTime.Today.Date.Ticks}";
                                    break;
                                case DateFilterMethod.TillYesterday:
                                    startDateQuery = $" StartDate <= {DateTime.Today.Date.AddDays(-1).Ticks}";
                                    break;
                               case DateFilterMethod.Tomorrow:
                                    startDateQuery = $" StartDate = {DateTime.Today.Date.AddDays(1).Ticks}";
                                    break;
                                case DateFilterMethod.Yesterday:
                                    startDateQuery = $" StartDate < {DateTime.Today.Date.AddDays(-1).Ticks}";
                                    break;
                                case DateFilterMethod.GreaterThan:
                                    startDateQuery = $" StartDate > {date.Ticks}";
                                    break;
                                case DateFilterMethod.LessThan:
                                    startDateQuery = $" StartDate < {date.Ticks}";
                                    break;
                                case DateFilterMethod.GreaterThanOrEqual:
                                    startDateQuery = $" StartDate >= {date.Ticks}";
                                    break;
                                case DateFilterMethod.LessThanOrEqual:
                                    startDateQuery = $" StartDate <= {date.Ticks}";
                                    break;
                            }

                            if (isFirstFilterQueryApplied == false)
                            {
                                query += startDateQuery;
                                isFirstFilterQueryApplied = true;
                            }
                            else
                            {
                                query += filterMethodQuery + startDateQuery;
                            }
                        }
                        break;
                    case FilterProperty.EndDate:
                        if (filterPropertyAndValue.Method != null )
                        {
                            string dueDateQuery = "";
                            DateFilterMethod selectedDateFilterMethod = (DateFilterMethod)filterPropertyAndValue.Method;
                            DateTime date = (DateTime)filterPropertyAndValue.Value;
                            switch (selectedDateFilterMethod)
                            {
                                case DateFilterMethod.Today:
                                    dueDateQuery = $" DueDate = {DateTime.Today.Date.Ticks}";
                                    break;
                                case DateFilterMethod.TillYesterday:
                                    dueDateQuery = $" DueDate <= {DateTime.Today.Date.AddDays(-1).Ticks}";
                                    break;
                                case DateFilterMethod.Tomorrow:
                                    dueDateQuery = $" DueDate = {DateTime.Today.Date.AddDays(1).Ticks}";
                                    break;
                                case DateFilterMethod.Yesterday:
                                    dueDateQuery = $" DueDate < {DateTime.Today.Date.AddDays(-1).Ticks}";
                                    break;
                                case DateFilterMethod.GreaterThan:
                                    dueDateQuery = $" DueDate > {date.Ticks}";
                                    break;
                                case DateFilterMethod.LessThan:
                                    dueDateQuery = $" DueDate < {date.Ticks}";
                                    break;
                                case DateFilterMethod.GreaterThanOrEqual:
                                    dueDateQuery = $" DueDate >= {date.Ticks}";
                                    break;
                                case DateFilterMethod.LessThanOrEqual:
                                    dueDateQuery = $" DueDate <= {date.Ticks}";
                                    break;
                            }

                            if (isFirstFilterQueryApplied == false)
                            {
                                query += dueDateQuery;
                                isFirstFilterQueryApplied = true;
                            }
                            else
                            {
                                query += filterMethodQuery + dueDateQuery;
                            }
                        }
                        break;
                }
            }
            query += QueryStrings.CloseParenthesisQuery;

            return _DBAdapter.ExecuteQuery<ZTaskObj>(query,App.AppUserID);
            
        }
    }
}
