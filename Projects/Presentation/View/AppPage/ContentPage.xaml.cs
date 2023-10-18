using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.View.AppPage;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContentPage : Page,IGetProjects,IGetMilestones
    {
        public CoreDispatcher ZCoreDispatcher { get; }
        EventNotification _EventNotification = EventNotification.GetInstance();
        DispatcherTimer _DispatcherTimer;
        GetProjectsVM _GetProjectsVM;
        GetMilestonesVM _GetMilestonesVM;
        int _DispatcherTimesTicked = 0;
        int _DispatcherTimesToTick = 3;
        public ContentPage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetProjectsVM = new GetProjectsVM(this);
            _GetMilestonesVM= new GetMilestonesVM(this);
        }
        public void LoadProject(ProjectObj project)
        {
            ProjectsNavigationViewItem.IsSelected = true;
            MainFrame.Navigate(typeof(ProjectDetailPage), project);
        }
       
        public void LoadMilestone(MilestoneObj milestone)
        {
           MilestonesNavigationViewItem.IsSelected = true;
           MainFrame.Navigate(typeof(MilestoneDetailPage), milestone);
        }
        public void LoadProjects(ProjectsType projectsType, IEnumerable<ProjectObj> projects)
        {
            throw new NotImplementedException();
        }
        public void LoadMilestones(MilestonesType milestonesType, IEnumerable<MilestoneObj> milestones)
        {
            throw new NotImplementedException();
        }
        public void AutoSuggestionBoxProjectSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ProjectObj> projects)
        {
            throw new NotImplementedException();
        }

        public void AutoSuggestionBoxMilestoneSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<MilestoneObj> milestones)
        {
            throw new NotImplementedException();
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        private void ContentPagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.ProjectTappedForDetailView += UpdateProjectTappedForDetailViewEvent;
            _EventNotification.MilestoneTappedForDetailView += UpdateMilestoneTappedForDetailViewEvent;
            _EventNotification.TaskTappedForDetailView += UpdateTaskTappedForDetailViewEvent;
            _EventNotification.DashboardSummaryPanelTapped += UpdateDashboardSummaryPanelTappedEvent;
        }
        private void ContentPagePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 640)
            {
                MainNavigationView.IsPaneToggleButtonVisible = false;
                MainNavigationViewHeaderToggleButton.Visibility = Visibility.Visible;
            }
            else
            {
                MainNavigationView.IsPaneToggleButtonVisible = true;
                MainNavigationViewHeaderToggleButton.Visibility = Visibility.Collapsed;
            }
        }
        private void MainNavigationViewHeaderToggleButton_Click(object sender, RoutedEventArgs e)
        {
            MainNavigationView.IsPaneOpen = !MainNavigationView.IsPaneOpen;
        }
        void UpdateNavigateAccountsPageEvent()
        {
            SettingsNavigationViewItem.IsSelected = true;
        }
        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            NavigatePage();
        }

        void UpdateProjectTappedForDetailViewEvent(int projectID)
        {
            _GetProjectsVM.GetProjects(ProjectsType.Particular, projectID,null);
        }
        void UpdateMilestoneTappedForDetailViewEvent(int milestoneID)
        {
            _GetMilestonesVM.GetMilestones(MilestonesType.Particular, milestoneID, -1,null);
        }
        
        void UpdateTaskTappedForDetailViewEvent(ZTask task,IEnumerable<ZTaskObj> tasks)
        {
            TasksNavigationViewItem.IsSelected = true;
            MainFrame.Navigate(typeof(TaskDetailViewPage),task);
            _EventNotification.InvokePassDataToTaskDetailsPageEvent(tasks);
        }
        void UpdateDashboardSummaryPanelTappedEvent(DashboardSummaryType dashboardSummaryType)
        {
            switch (dashboardSummaryType)
            {
                case DashboardSummaryType.OpenTasks:
                    TasksNavigationViewItem.IsSelected = true;
                    MainFrame.Navigate(typeof(TasksPage),TasksType.Open);
                    break;
                case DashboardSummaryType.ClosedTasks:
                    TasksNavigationViewItem.IsSelected = true;
                    MainFrame.Navigate(typeof(TasksPage), TasksType.Closed);
                    break;
                case DashboardSummaryType.ActiveMilestones:
                    MilestonesNavigationViewItem.IsSelected = true;
                    MainFrame.Navigate(typeof(MilestonesPage), MilestonesType.Active);
                    break;
                case DashboardSummaryType.CompletedMilestones:
                    MilestonesNavigationViewItem.IsSelected = true;
                    MainFrame.Navigate(typeof(MilestonesPage), MilestonesType.Completed);
                    break;
                case DashboardSummaryType.ActiveProjects:
                    ProjectsNavigationViewItem.IsSelected=true;
                    MainFrame.Navigate(typeof(ProjectsPage), ProjectsType.Active);
                    break;
                case DashboardSummaryType.CompletedProjects:
                    ProjectsNavigationViewItem.IsSelected = true;
                    MainFrame.Navigate(typeof(ProjectsPage), ProjectsType.Completed);
                    break;

            }
        }

        void NavigatePage()
        {
            if (MainNavigationView != null && HomeNavigationViewItem != null)
            {

                if (HomeNavigationViewItem.IsSelected)
                {
                    MainFrame.Navigate(typeof(HomePage));

                }
                else if (ProjectsNavigationViewItem.IsSelected)
                {
                    MainFrame.Navigate(typeof(ProjectsPage));

                }
                else if (MilestonesNavigationViewItem.IsSelected)
                {
                    MainFrame.Navigate(typeof(MilestonesPage));

                }
                else if (TasksNavigationViewItem.IsSelected)
                {
                    MainFrame.Navigate(typeof(TasksPage));

                }

                else if (UsersNavigationViewItem.IsSelected)
                {
                    MainFrame.Navigate(typeof(UsersPage));

                }
                else if (SettingsNavigationViewItem.IsSelected)
                {
                    MainFrame.Navigate(typeof(SettingsPage));
                }
            }
        }

        private void ContentPagePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.ProjectTappedForDetailView -= UpdateProjectTappedForDetailViewEvent;
            _EventNotification.MilestoneTappedForDetailView -= UpdateMilestoneTappedForDetailViewEvent;
            _EventNotification.TaskTappedForDetailView -= UpdateTaskTappedForDetailViewEvent;
            _EventNotification.DashboardSummaryPanelTapped -= UpdateDashboardSummaryPanelTappedEvent;
            
        }

       

        
    }
}
