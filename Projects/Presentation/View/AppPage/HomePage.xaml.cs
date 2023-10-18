using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Core.Helper;
using Projects.Notification;
using Projects.Presentation.View.AppUserControl;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ZohoProjects;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page,IGetTasks,IGetMilestones,IGetProjects,IEditMilstone,IEditProject
    {
        public string CurrentTimeString
        {
            get { return (string)GetValue(CurrentTimeStringProperty); }
            set { SetValue(CurrentTimeStringProperty, value); }
        }

        public CoreDispatcher ZCoreDispatcher { get; }

        public static readonly DependencyProperty CurrentTimeStringProperty = DependencyProperty.Register("CurrentTimeString", typeof(string),typeof(HomePage),new PropertyMetadata(string.Empty));

        EventNotification _EventNotification=EventNotification.GetInstance();
        List<ZTaskObj> _TaskList = new List<ZTaskObj>();
        List<MilestoneObj> _MilestoneList = new List<MilestoneObj>();
        List<ProjectObj> _ProjectList = new List<ProjectObj>();
        bool _IsTasksLoaded = false;
        bool _IsMilestonesLoaded = false;
        bool _IsProjectsLoaded = false;
        List<Grid> _DashboardSummaryPanelList;
        GetTasksVM _GetTasksVM;
        GetMilestonesVM _GetMilestonesVM;
        GetProjectsVM _GetProjectsVM;
        EditMilstoneVM _EditMilstoneVM;
        EditProjectVM _EditProjectVM;
        public HomePage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetTasksVM = new GetTasksVM(this);
            _GetMilestonesVM = new GetMilestonesVM(this);
            _GetProjectsVM = new GetProjectsVM(this);
            _EditMilstoneVM = new EditMilstoneVM(this);
            _EditProjectVM = new EditProjectVM(this);
            StartDispatcherTimer();
            SetCurrentTime();
            _GetTasksVM.GetTasks(TasksType.All, null);
            _GetMilestonesVM.GetMilestones(MilestonesType.All, null, -1,null);
            _GetProjectsVM.GetProjects(ProjectsType.All, null, null);

        }
        public void LoadTasks(TasksType tasksType, IEnumerable<ZTaskObj> tasks)
        {
            _TaskList.Clear();
            foreach (var task in tasks)
            {
                _TaskList.Add(task);
            }
            DataLoadingProgressRing.IsActive = false;
            _IsTasksLoaded = true;
            LoadData();
           
        }
        public void LoadMilestones(MilestonesType milestonesType, IEnumerable<MilestoneObj> milestones)
        {
            _MilestoneList.Clear();
            foreach (var item in milestones)
            {
                _MilestoneList.Add(item);
            }
            _IsMilestonesLoaded = true;
            LoadData();
          
        }
       
        public void LoadProjects(ProjectsType projectsType, IEnumerable<ProjectObj> projects)
        {
            _ProjectList.Clear();
            foreach (var task in projects)
            {
                _ProjectList.Add(task);
            }
            _IsProjectsLoaded = true;
            LoadData();
           
        }
       
        public void ShowNotification(string message)
        {
            //_EventNotification.InvokeShowNotification(message);
        }
        void LoadData()
        {
            if (_IsTasksLoaded && _IsMilestonesLoaded && _IsProjectsLoaded)
            {
                int completedTasksCount = 0;
                int allTasksCount = 0;
                foreach (Milestone milestone in _MilestoneList)
                {
                    completedTasksCount = _TaskList.Where(t => t.MilestoneID == milestone.ID && (t.Status == ZTaskStatus.Closed || t.Status == ZTaskStatus.Cancelled)).Count();
                    allTasksCount = _TaskList.Where(t => t.MilestoneID == milestone.ID).Count();
                    if (milestone.CompletedTasksCount != completedTasksCount)
                    {
                        milestone.CompletedTasksCount = completedTasksCount;
                        _EditMilstoneVM.EditMilestoneProperty(milestone.ID,MilestonePropertyEditType.CompletedTasksCount, completedTasksCount);
                    }
                    if (milestone.AllTasksCount != allTasksCount)
                    {
                        milestone.AllTasksCount = allTasksCount;
                        _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.AllTasksCount, allTasksCount);
                    }
                    if (milestone.CompletedPercentage == 100)
                    {
                        milestone.Status = MilestoneStatus.Completed; 
                        _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.Status, milestone.Status);
                    }
                }
                foreach (Project project in _ProjectList)
                {
                    
                    project.CompletedTasksCount = _MilestoneList.Where(t => t.ProjectID == project.ID && (t.Status == MilestoneStatus.Completed || t.Status == MilestoneStatus.Cancelled)).Count() ;
                    project.AllTasksCount =_TaskList.Where(t => t.MilestoneID == project.ID).Count();
                    if (project.CompletedTasksCount != completedTasksCount)
                    {
                        project.CompletedTasksCount = completedTasksCount;
                        _EditProjectVM.EditProjectProperty(project.ID, ProjectPropertyEditType.CompletedTasksCount, completedTasksCount);
                    }
                    if (project.AllTasksCount != allTasksCount)
                    {
                        project.AllTasksCount = allTasksCount;
                        _EditProjectVM.EditProjectProperty(project.ID, ProjectPropertyEditType.AllTasksCount, allTasksCount);
                    }
                    if (project.CompletedPercentage == 100)
                    {
                        project.Status = ProjectStatus.Completed;
                        _EditProjectVM.EditProjectProperty(project.ID, ProjectPropertyEditType.Status, project.Status);
                    }
                    if (project.CompletedPercentage == 100)
                    {
                        project.Status = ProjectStatus.Completed;
                    }
                }

                DashboardSummaryOpenTasksCountTextBlock.Text = _TaskList.Where(t => t.Status != ZTaskStatus.Closed && t.Status != ZTaskStatus.Cancelled).Count().ToString();
                DashboardSummaryClosedTasksCountTextBlock.Text = _TaskList.Where(t => t.Status == ZTaskStatus.Closed || t.Status == ZTaskStatus.Cancelled).Count().ToString();

                DashboardSummaryActiveMilestonesCountTextBlock.Text = _MilestoneList.Where(t => t.Status != MilestoneStatus.Cancelled && t.Status != MilestoneStatus.Completed).Count().ToString();
                DashboardSummaryCompletedMilestonesCountTextBlock.Text = _MilestoneList.Where(t => t.Status == MilestoneStatus.Cancelled || t.Status == MilestoneStatus.Completed).Count().ToString();

                DashboardSummaryActiveProjectsCountTextBlock.Text = _ProjectList.Where(t => t.Status != ProjectStatus.Cancelled && t.Status != ProjectStatus.Completed).Count().ToString();
                DashboardSummaryClosedProjectsCountTextBlock.Text = _ProjectList.Where(t => t.Status == ProjectStatus.Cancelled || t.Status == ProjectStatus.Completed).Count().ToString();

                LoadWidgetsData();
            }
        }
        void LoadWidgetsData()
        {

            ListWidgetControl overdueTasksListWidgetControl = new ListWidgetControl();
            overdueTasksListWidgetControl.WidgetTitle = "Overdue Tasks";
            overdueTasksListWidgetControl._Tasks =new ObservableCollection<ZTaskObj>( _TaskList.Where(t => t.EndDate.Date < DateTime.Today.Date && t.EndDate.Date != DateTime.MinValue.Date));

            ListWidgetControl dueTodayTasksListWidgetControl = new ListWidgetControl();
            dueTodayTasksListWidgetControl.WidgetTitle = "Due Today Tasks";
            dueTodayTasksListWidgetControl._Tasks = new ObservableCollection<ZTaskObj>(_TaskList.Where(t => t.EndDate.Date == DateTime.Today.Date).ToList());

            ChartWidgetControl taskPercentageChartWidgetControl = new ChartWidgetControl();
            taskPercentageChartWidgetControl.WidgetTitle = "Task Percentage";
            List<KeyValuePair<string, int>> taskPercentageKeyValuePair = new List<KeyValuePair<string, int>>();
            for (int i = 0; i <= 100; i = i + 10)
            {
                int count = _TaskList.Where(task => task.CompletedPercentage == i).Count();
                taskPercentageKeyValuePair.Add(new KeyValuePair<string, int>(i.ToString() + "%", count));
            }
            foreach (var pair in taskPercentageKeyValuePair)
            {
                taskPercentageChartWidgetControl.ValueCollection.Add(pair);
            }

            ChartWidgetControl taskStatusChartWidgetControl = new ChartWidgetControl();
            taskStatusChartWidgetControl.WidgetTitle = "Task Status";
            List<KeyValuePair<string, int>> taskStatusKeyValuePair = new List<KeyValuePair<string, int>>();
            foreach (ZTaskStatus status in Enum.GetValues(typeof(ZTaskStatus)))
            {
                int count = _TaskList.Where(task => task.Status == status).Count();
                taskStatusKeyValuePair.Add(new KeyValuePair<string, int>(ZTaskHelper.ConvertTaskStatusToString(status), count));
            }
            foreach (var pair in taskStatusKeyValuePair)
            {
                taskStatusChartWidgetControl.ValueCollection.Add(pair);

            }

            ChartWidgetControl milestonePercentageChartWidgetControl = new ChartWidgetControl();
            milestonePercentageChartWidgetControl.WidgetTitle = "Milestone Percentage";
            List<KeyValuePair<string, int>> milestonePercentageKeyValuePair = new List<KeyValuePair<string, int>>();
            for (int i = 0; i <= 100; i = i + 10)
            {
                int count = _MilestoneList.Where(milestone => milestone.CompletedPercentage == i).Count();
                milestonePercentageKeyValuePair.Add(new KeyValuePair<string, int>(i.ToString() + "%", count));
            }
            foreach (var pair in milestonePercentageKeyValuePair)
            {
                milestonePercentageChartWidgetControl.ValueCollection.Add(pair);
            }


            ChartWidgetControl milestoneStatusChartWidgetControl = new ChartWidgetControl();
            milestoneStatusChartWidgetControl.WidgetTitle = "Milestone Status";
            List<KeyValuePair<string, int>> milestoneStatusKeyValuePair = new List<KeyValuePair<string, int>>();
            foreach (MilestoneStatus status in Enum.GetValues(typeof(MilestoneStatus)))
            {
                int count = _MilestoneList.Where(milestone => milestone.Status == status).Count();
                milestoneStatusKeyValuePair.Add(new KeyValuePair<string, int>(MilestoneHelper.ConvertMilestoneStatusToString(status), count));
            }
            foreach (var pair in milestoneStatusKeyValuePair)
            {
                milestoneStatusChartWidgetControl.ValueCollection.Add(pair);
            }


            ChartWidgetControl projectPercentageChartWidgetControl = new ChartWidgetControl();
            projectPercentageChartWidgetControl.WidgetTitle = "Project Percentage";
            List<KeyValuePair<string, int>> projectPercentageKeyValuePair = new List<KeyValuePair<string, int>>();
            for (int i = 0; i <= 100; i = i + 10)
            {
                int count = _ProjectList.Where(project => project.CompletedPercentage == i).Count();
                projectPercentageKeyValuePair.Add(new KeyValuePair<string, int>(i.ToString() + "%", count));
            }
            foreach (var pair in projectPercentageKeyValuePair)
            {
                projectPercentageChartWidgetControl.ValueCollection.Add(pair);
            }


            ChartWidgetControl projectStatusChartWidgetControl = new ChartWidgetControl();
            projectStatusChartWidgetControl.WidgetTitle = "Project Status";
            List<KeyValuePair<string, int>> projectStatusKeyValuePair = new List<KeyValuePair<string, int>>();
            foreach (ProjectStatus status in Enum.GetValues(typeof(ProjectStatus)))
            {
                int count = _ProjectList.Where(project => project.Status == status).Count();
                projectStatusKeyValuePair.Add(new KeyValuePair<string, int>(ProjectHelper.ConvertProjectStatusToString(status), count));
            }
            foreach (var pair in projectStatusKeyValuePair)
            {
                projectStatusChartWidgetControl.ValueCollection.Add(pair);
            }

            WidgetAdaptiveGridView.Items.Add(overdueTasksListWidgetControl);
            WidgetAdaptiveGridView.Items.Add(dueTodayTasksListWidgetControl);
            WidgetAdaptiveGridView.Items.Add(taskPercentageChartWidgetControl);
            WidgetAdaptiveGridView.Items.Add(taskStatusChartWidgetControl);

            WidgetAdaptiveGridView.Items.Add(milestonePercentageChartWidgetControl);
            WidgetAdaptiveGridView.Items.Add(milestoneStatusChartWidgetControl);
            WidgetAdaptiveGridView.Items.Add(projectPercentageChartWidgetControl);
            WidgetAdaptiveGridView.Items.Add(projectStatusChartWidgetControl);
        }
        string GetAppUserName()
        {
            return "Hi "+App.AppUser.Name;
        }
        string GetAppUserMail()
        {
            return "( "+App.AppUser.MailID+" )";
        }

        BitmapImage GetAppUserBitmapImage()
        {
            try
            {
                Uri imageUri = new Uri(App.AppUser.ImagePath);
                if (imageUri != null)
                {
                    return new BitmapImage(imageUri);
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }
        ZUser GetAppUser()
        {
            return App.AppUser;
        }
        
        void SetCurrentTime()
        {
            CurrentTimeString = GetCurrentTimeString(DateTime.Now);
            if (DateTime.Now.Hour <= 17)
            {
                SunBackgroundImage.Visibility = Visibility.Visible;
                MoonBackgroundImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                SunBackgroundImage.Visibility = Visibility.Collapsed;
                MoonBackgroundImage.Visibility = Visibility.Visible;
            }
        }
        void StartDispatcherTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherTimer.Tick += (sender, e) =>
            {
                SetCurrentTime();
            };
            dispatcherTimer.Start();
        }
        string GetCurrentTimeString(DateTime dateTime)
        {
            return dateTime.ToString("MMM dd") + ", " + dateTime.ToString("hh:mm tt");
        }
        
       
        private void DashboardSummaryAdaptiveGridView_Loaded(object sender, RoutedEventArgs e)
        {
            _DashboardSummaryPanelList = new List<Grid>() { DashboardSummaryPanel1, DashboardSummaryPanel2, DashboardSummaryPanel3,
            DashboardSummaryPanel4,DashboardSummaryPanel5,DashboardSummaryPanel6};
            foreach (Grid grid in _DashboardSummaryPanelList)
            {
                grid.PointerEntered += DashboardSummaryPanel_PointerEntered;
                grid.PointerExited += DashboardSummaryPanel_PointerExited;
            }
        }

        private void DashboardSummaryPanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Background = (SolidColorBrush)Application.Current.Resources["AppContentBackgroundBrush"];
        }
        private void DashboardSummaryPanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Background = (SolidColorBrush)Application.Current.Resources["AppMainBackgroundBrush"];
        }
        private void DashboardSummaryPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid grid=sender as Grid;
            DashboardSummaryType dashboardSummaryType = (DashboardSummaryType)Enum.Parse(typeof(DashboardSummaryType),grid.Tag.ToString());
            _EventNotification.InvokeDashboardSummaryPanelTappedEvent(dashboardSummaryType);
        }
        public void LoadTask(ZTaskObj task)
        {
            throw new NotImplementedException();
        }

        public void LoadMilestone(MilestoneObj milestone)
        {
            throw new NotImplementedException();
        }

        public void LoadProject(ProjectObj project)
        {
            throw new NotImplementedException();
        }
        public void AutoSuggestionBoxMilestoneSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<MilestoneObj> milestones)
        {
            throw new NotImplementedException();
        }

        public void AutoSuggestionBoxProjectSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ProjectObj> projects)
        {
            throw new NotImplementedException();
        }

       
    }
}

