using Microsoft.UI.Xaml.Controls;
using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Presentation.View.AppUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Projects.Notification
{
    public class EventNotification
    {
       static EventNotification _EventNotificationInstance;
      

        public static EventNotification GetInstance()
        {
            if(_EventNotificationInstance==null)
                _EventNotificationInstance=new EventNotification();
            return _EventNotificationInstance;
        }

        public event Action<ZTaskObj> TaskChecked;
        public event Action<ZTaskObj> TaskUnchecked;
        public event Action UncheckAllTasks;
        public event Action CheckAllTasks;
        public event Action<IList<ZTaskObj>> TasksDeleted;
        public event Action<IList<ZSubTask>> SubTasksDeleted;
        public event Action<TaskPropertyEditType, ZTaskObj> TaskPropertyEdited;

        public event Action<MilestoneObj> MilestoneChecked;
        public event Action<MilestoneObj> MilestoneUnchecked;
        public event Action CheckAllMilestones;
        public event Action UncheckAllMilestones;
        public event Action<IList<MilestoneObj>> MilestonesDeleted;

        public event Action<ProjectObj> ProjectChecked;
        public event Action<ProjectObj> ProjectUnchecked;
        public event Action CheckAllProjects;
        public event Action UncheckAllProjects;
        public event Action<IList<ProjectObj>> CheckedProjectsDeleted;

        public event Action<ProjectObj> NewProjectAdded;
        public event Action<MilestoneObj> NewMilestoneAdded;
        public event Action<ZTaskObj> NewTaskAdded;

        public event Action<int> ProjectTappedForDetailView;
        public event Action<int> MilestoneTappedForDetailView;
        public event Action<ZTaskObj, IEnumerable<ZTaskObj>> TaskTappedForDetailView;

        public event Action<List<FilterProperty>> AppliedFilters;
        public event Action CancelFilter;
        public event Action CloseFilter;

        public event Action<DashboardSummaryType> DashboardSummaryPanelTapped;

        public event Action SignedOut;
        public event Action SignedIn;
        public event Action ShowSignUpPageEvent;

        public event Action<string> ShowNotification;
        public event Action<string, InfoBarSeverity> ShowNotificationWithSeverity;
        //public event Action<Type> OpenSplitPanelEvent;
        //public event Action<Type> CloseSplitPanelEvent;

        //public event Action NavigatePlainViewPage_Event;
        //public event Action NavigateAccountsPage_Event;



        public event Action OpenTaskDetailsPageEvent;
        public event Action<IEnumerable<ZTaskObj>> PassDataToTaskDetailsPageEvent;

        public event Action<int> KanbanViewBoardWidthChanged;
        public event Action<ZTaskObj> KanbanViewDragStartedEvent;
        public event Action<ListView> KanbanViewTargetListViewDragStartedEvent;
        public event Action<ZTaskObj> KanbanViewItemPoinerEnteredEvent;
       
       
        public event Action<ElementTheme> ThemeModeChanged;
        public event Action<Color> AccentColorChanged;
        public event Action<int> BackgroundImageChanged;


        public void InvokeTaskCheckedEvent(ZTaskObj task)
        {
            TaskChecked?.Invoke(task);
        }
        public void InvokeTaskUncheckedEvent(ZTaskObj task)
        {
            TaskUnchecked?.Invoke(task);
        }
        public void InvokeUncheckAllTasksEvent()
        {
            UncheckAllTasks?.Invoke();
        }
        public void InvokeCheckAllTasksEvent()
        {
            CheckAllTasks?.Invoke();
        }
         public void InvokeTasksDeletedEvent(IList<ZTaskObj> tasks)
        {
            TasksDeleted?.Invoke(tasks);
        }
        public void InvokSubTasksDeletedEvent(IList<ZSubTask> subTasks)
        {
            SubTasksDeleted?.Invoke(subTasks);
        }

        public void InvokeTaskPropertyEditedEvent(TaskPropertyEditType taskProperty,ZTaskObj task)
        {
            TaskPropertyEdited?.Invoke(taskProperty,task);
        }

        public void InvokeMilestoneCheckedEvent(MilestoneObj milestone)
        {
            MilestoneChecked?.Invoke(milestone);
        }
        public void InvokeMilestoneUncheckedEvent(MilestoneObj milestone)
        {
            MilestoneUnchecked?.Invoke(milestone);
        }
        public void InvokeCheckAllMilestonesEvent()
        {
            CheckAllMilestones?.Invoke();
        }
        public void InvokeUncheckAllMilestonesEvent()
        {
            UncheckAllMilestones?.Invoke();
        }
        public void InvokeMilestonesDeletedEvent(IList<MilestoneObj> milestones)
        {
            MilestonesDeleted?.Invoke(milestones);
        }

        public void InvokeProjectCheckedEvent(ProjectObj projects)
        {
            ProjectChecked?.Invoke(projects);
        }
        public void InvokeProjectUncheckedEvent(ProjectObj projects)
        {
            ProjectUnchecked?.Invoke(projects);
        }
        public void InvokeCheckAllProjectsEvent()
        {
            CheckAllProjects?.Invoke();
        }
        public void InvokeUncheckAllProjectsEvent()
        {
            UncheckAllProjects?.Invoke();
        }
        public void InvokeCheckedProjectsDeletedEvent(IList<ProjectObj> projects)
        {
            CheckedProjectsDeleted?.Invoke(projects);
        }


        public void InvokeShowSignUpPageEvent()
        {
            ShowSignUpPageEvent?.Invoke();
        }
       
        public void InvokeShowNotification(string message)
        {
            ShowNotification?.Invoke(message);
        }
        
        public void InvokeShowNotificationWithSeverity(string message,InfoBarSeverity infoBarSeverity)
        {
            ShowNotificationWithSeverity?.Invoke(message,infoBarSeverity);
        }

        public void InvokeNewTaskAddedEvent(ZTaskObj task)
        {
            NewTaskAdded?.Invoke(task);
        }
        public void InvokeNewProjectAddedEvent(ProjectObj project)
        {
            NewProjectAdded?.Invoke(project);
        }
        public void InvokeNewMilestoneAddedEvent(MilestoneObj milestone)
        {
            NewMilestoneAdded?.Invoke(milestone);
        }
        public void InvokeTaskTappedForDetailViewEvent(ZTaskObj task,IEnumerable<ZTaskObj> tasks)
        {
            TaskTappedForDetailView?.Invoke(task,tasks);
        }

        public void InvokeAppliedFiltersEvent(List<FilterProperty> appliedFilterProperties)
        {
            AppliedFilters?.Invoke(appliedFilterProperties);
        }
        public void InvokeCancelFilterEvent()
        {
            CancelFilter?.Invoke();
        }
        public void InvokeCloseFilterEvent()
        {
            CloseFilter?.Invoke();
        }
        public void InvokeDashboardSummaryPanelTappedEvent(DashboardSummaryType dashboardSummaryType)
        {
            DashboardSummaryPanelTapped?.Invoke(dashboardSummaryType);
        }
        public void InvokeProjectTappedForDetailViewEvent(int projectID)
        {
            ProjectTappedForDetailView?.Invoke(projectID);
        }
        public void InvokeMilestoneTappedForDetailViewEvent(int milestoneID)
        {
            MilestoneTappedForDetailView?.Invoke(milestoneID);
        }



        public void InvokeOpenTaskDetailsPageEvent()
        {
            OpenTaskDetailsPageEvent?.Invoke();
        }
        public void InvokePassDataToTaskDetailsPageEvent(IEnumerable<ZTaskObj> tasks)
        {
            PassDataToTaskDetailsPageEvent?.Invoke(tasks);
        }
        public void InvokeKanbanViewDragStartedEvent(ZTaskObj task)
        {
            KanbanViewDragStartedEvent?.Invoke(task);
        }

        public void InvokeKanbanViewBoardidthChangedEvent(int boardWidth)
        {
            KanbanViewBoardWidthChanged?.Invoke(boardWidth);
        }
        public void InvokeKanbanViewItemPoinerEnteredEvent(ZTaskObj task)
        {
            KanbanViewItemPoinerEnteredEvent?.Invoke(task);
        }
       public void InvokeKanbanViewTargetListViewDragStartedEvent(ListView listView)
        {
            KanbanViewTargetListViewDragStartedEvent?.Invoke(listView);
        }
       
        public void InvokeThemeModeChangedEvent(ElementTheme elementTheme)
        {
            ThemeModeChanged?.Invoke(elementTheme);
        }
        public void InvokeAccentColorChangedEvent(Color changedColor)
        {
            AccentColorChanged?.Invoke(changedColor);
        }
        public void InvokeBackgroundImageChangedEvent(int bgID)
        {
            BackgroundImageChanged?.Invoke(bgID);
        }
        public void InvokeSignedInEvent()
        {
            SignedIn?.Invoke();
        }
        public void InvokeSignedOutEvent()
        {
            SignedOut?.Invoke();
        }
    }
}
