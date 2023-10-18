using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Core.Helper;
using Projects.Notification;
using Projects.Presentation.View.AppUserControl;
using Projects.Presentation.View.AppUserControl.MilestoneUserControl;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ZohoProjects;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProjectDetailPage : Page,IEditProject,IGetUsers,IGetTasks,IGetMilestones,INotifyPropertyChanged
    {
        
        public ProjectObj _Project { get; set; }
        public CoreDispatcher ZCoreDispatcher { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        EventNotification _EventNotification = EventNotification.GetInstance();
        ObservableCollection<MilestoneObj> _MilestoneCollection=new ObservableCollection<MilestoneObj>();
        ObservableCollection<ZTaskObj> _TaskCollection=new ObservableCollection<ZTaskObj>();
        bool _IsProjectNameTextBoxTextChanged = false;
        bool _IsTasksLoaded=false;
        bool _IsMilestonesLoaded=false;
        ZUser SelectedOwner;
        EditProjectVM _EditProjectVM;
        GetUsersVM _GetUsersVM;
        GetMilestonesVM _GetMilestonesVM;
        GetTasksVM _GetTasksVM;
        public ProjectDetailPage()
        {
            this.InitializeComponent();

            ZCoreDispatcher = Dispatcher;
           
            _EditProjectVM = new EditProjectVM(this);
            _GetUsersVM = new GetUsersVM(this);
            _GetMilestonesVM = new GetMilestonesVM(this);
            _GetTasksVM = new GetTasksVM(this);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _Project = (ProjectObj)e.Parameter;
            ProjectsDetailDescriptionControl.DesciptionText =_Project.Description;
            SelectedOwner = _Project.Owner;
        }
        private void ProjectDetailPagePanel_Loaded(object sender, RoutedEventArgs e)
        {
           ProjectsDetailDescriptionControl.DescriptionRichEditBoxLostFocus += UpdateDescriptionTextBlockLostFocusEvent;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProjectsPage));
        }

        public void LoadMilestones(MilestonesType milestonesType, IEnumerable<MilestoneObj> milestones)
        {
            _IsMilestonesLoaded = true;
            _MilestoneCollection.Clear();
            foreach (MilestoneObj milestone in milestones)
            {
                _MilestoneCollection.Add(milestone);
            }
            if (MilestonesPanel != null)
            {
                MilestonePlainViewControl.IsMilestonesLoaded = true;
            }

            ChartWidgetControl milestonePercentageChartWidgetControl = new ChartWidgetControl();
            milestonePercentageChartWidgetControl.WidgetTitle = "Milestone Percentage";
            List<KeyValuePair<string, int>> milestonePercentageKeyValuePair = new List<KeyValuePair<string, int>>();
            for (int i = 0; i <= 100; i = i + 10)
            {
                int count = _MilestoneCollection.Where(milestone => milestone.CompletedPercentage == i).Count();
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
                int count = _MilestoneCollection.Where(milestone => milestone.Status == status).Count();
                milestoneStatusKeyValuePair.Add(new KeyValuePair<string, int>(MilestoneHelper.ConvertMilestoneStatusToString(status), count));
            }
            foreach (var pair in milestoneStatusKeyValuePair)
            {
                milestoneStatusChartWidgetControl.ValueCollection.Add(pair);
            }
            DashboardAdaptiveGridView.Items.Add(milestonePercentageChartWidgetControl);
            DashboardAdaptiveGridView.Items.Add(milestoneStatusChartWidgetControl);
        }

        public void LoadTasks(TasksType tasksType, IEnumerable<ZTaskObj> tasks)
        {
            _IsTasksLoaded = true;
            _TaskCollection.Clear();
            foreach (ZTaskObj task in tasks)
            {
                _TaskCollection.Add(task);
            }
            if (TasksPanel != null)
            {
                TaskPlainViewControl.IsTasksLoaded = true;
            }

            ListWidgetControl overdueTasksListWidgetControl = new ListWidgetControl();
            overdueTasksListWidgetControl.WidgetTitle = "Overdue Tasks";
            overdueTasksListWidgetControl._Tasks = new ObservableCollection<ZTaskObj>(_TaskCollection.Where(t => t.EndDate.Date < DateTime.Today.Date).ToList());

            ListWidgetControl dueTodayTasksListWidgetControl = new ListWidgetControl();
            dueTodayTasksListWidgetControl.WidgetTitle = "Due Today Tasks";
            dueTodayTasksListWidgetControl._Tasks = new ObservableCollection<ZTaskObj>(_TaskCollection.Where(t => t.EndDate.Date.Date == DateTime.Today.Date).ToList());

            ChartWidgetControl taskPercentageChartWidgetControl = new ChartWidgetControl();
            taskPercentageChartWidgetControl.WidgetTitle = "Task Percentage";
            List<KeyValuePair<string, int>> taskPercentageKeyValuePair = new List<KeyValuePair<string, int>>();
            for (int i = 0; i <= 100; i = i + 10)
            {
                int count = _TaskCollection.Where(task => task.CompletedPercentage == i).Count();
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
                int count = _TaskCollection.Where(task => task.Status == status).Count();
                taskStatusKeyValuePair.Add(new KeyValuePair<string, int>(ZTaskHelper.ConvertTaskStatusToString(status), count));
            }
            foreach (var pair in taskStatusKeyValuePair)
            {
                taskStatusChartWidgetControl.ValueCollection.Add(pair);

            }

            DashboardAdaptiveGridView.Items.Add(dueTodayTasksListWidgetControl);
            DashboardAdaptiveGridView.Items.Add(overdueTasksListWidgetControl);
            DashboardAdaptiveGridView.Items.Add(taskPercentageChartWidgetControl);
            DashboardAdaptiveGridView.Items.Add(taskStatusChartWidgetControl);
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }
        public void LoadUser(ZUser user)
        {
            throw new NotImplementedException();
        }

        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }

        public void LoadTask(ZTaskObj task)
        {
            throw new NotImplementedException();
        }

        public void LoadMilestone(MilestoneObj milestone)
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

        private void ProjectDetailPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                PivotItem deselectedpivotItem = (PivotItem)e.RemovedItems[0];
                
                if (deselectedpivotItem.Name == "Tasks")
                {
                    TaskViewOptionButtonsPanel.Visibility = Visibility.Collapsed;
                }

            }

            PivotItem selectedpivotItem = (PivotItem)e.AddedItems[0];

            if (selectedpivotItem.Name == "Tasks")
            {
                TaskViewOptionButtonsPanel.Visibility = Visibility.Visible;
                if (TasksPanel == null)
                {
                    this.FindName("TasksPanel");
                    if (_IsTasksLoaded == false)
                    {
                        _GetTasksVM.GetTasks(TasksType.Project, _Project.ID);
                    }
                    else
                    {
                        TaskPlainViewControl.IsTasksLoaded = true;
                    }
                }
               
            }
            else if (selectedpivotItem.Name == "Milestones")
            {
                if (MilestonesPanel == null)
                {
                    this.FindName("MilestonesPanel");
                    if (_IsMilestonesLoaded == false)
                    {
                        _GetMilestonesVM.GetMilestones(MilestonesType.Project, _Project.ID, -1, null);
                    }
                    else
                    {
                        MilestonePlainViewControl.IsMilestonesLoaded = true;
                    }
                }
            }
            else if (selectedpivotItem.Name == "Dashboard")
            {
                if (_IsTasksLoaded == false)
                {
                    _GetTasksVM.GetTasks(TasksType.Project, _Project.ID);
                }
                if (_IsMilestonesLoaded == false)
                {
                    _GetMilestonesVM.GetMilestones(MilestonesType.Project, _Project.ID, -1, null);
                }
            }
        }
        private void DashboardPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void TaskViewButton_Click(object sender, RoutedEventArgs e)
        {

            Button clickedButton = (Button)sender;
            foreach (Button button in TaskViewOptionButtonsPanel.Children)
            {
                if (button == clickedButton)
                    button.Style = (Style)this.Resources["PageSymbolAccentButtonStyle"];
                else
                    button.Style = (Style)this.Resources["PageSymbolWindows11ButtonStyle"];
            }
            ChangeTaskView(clickedButton);
        }
        void ChangeTaskView(Button clickedButton)
        {
            TaskViewType changedViewType = (TaskViewType)Enum.Parse(typeof(TaskViewType), clickedButton.Tag.ToString());

            if (clickedButton.Tag.ToString() == "Plain")
            {
                this.FindName("TaskPlainViewControl");
            }
            else if (clickedButton.Tag.ToString() == "Classic")
            {
                this.FindName("TaskClassicViewControl");
            }
            else if (clickedButton.Tag.ToString() == "Kanban")
            {
                this.FindName("TaskKanbanViewControl");
            }
            foreach (Control control in TasksPanel.Children)
            {
                if (control != null)
                {
                    if (changedViewType == (TaskViewType)Enum.Parse(typeof(TaskViewType), control.Tag.ToString()))
                    {
                        control.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        control.Visibility = Visibility.Collapsed;
                    }
                }
            }

        }

        private void UserCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Button button=sender as Button;
            ZUser user=button.DataContext as ZUser;
            if (user.Id != _Project.OwnerID)
            {
                _Project.UserCollection.Remove(user);
                _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.RemoveUser, user.Id);
            }
        }

        private void DetailsPanelProjectNameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DetailsPanelProjectNameTextBlock.Visibility = Visibility.Collapsed;
            DetailsPanelProjectNameTextBox.Visibility=Visibility.Visible;
        }

        private void DetailsPanelProjectNameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CollapseDetailsPanelProjectNameTextBox();
        }
        private void DetailsPanelProjectNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _IsProjectNameTextBoxTextChanged = true;
        }
        private void DetailsPanelProjectNameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                CollapseDetailsPanelProjectNameTextBox();
            }
        }
       void CollapseDetailsPanelProjectNameTextBox()
        {
            if (_IsProjectNameTextBoxTextChanged)
            {
                _Project.Name = DetailsPanelProjectNameTextBox.Text;
                _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.Name, _Project.Name);
                _IsProjectNameTextBoxTextChanged=false;
            }
            DetailsPanelProjectNameTextBox.Visibility = Visibility.Collapsed;
            DetailsPanelProjectNameTextBlock.Visibility = Visibility.Visible;
        }
        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            _Project.Status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), menuFlyoutItem.Tag.ToString());
            _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.Status, _Project.Status);
        }
       
        void CollapseOwnerSearchAutoSuggestBox()
        {
            MilestoneOwnerPanel.Visibility = Visibility.Visible;
            OwnerSearchAutoSuggestBox.Visibility = Visibility.Collapsed;
            OwnerSearchAutoSuggestBox.Text= string.Empty;
        }
        
        void CollapseUserSearchAutoSuggestBox()
        {
            UsersAutoSuggestBox.Visibility = Visibility.Collapsed;
            UsersAutoSuggestBox.Text = string.Empty;
        }
        private void MilestoneOwnerNameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MilestoneOwnerPanel.Visibility= Visibility.Collapsed;
            OwnerSearchAutoSuggestBox.Visibility = Visibility.Visible;
        }
        private void OwnerSearchAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CollapseOwnerSearchAutoSuggestBox();
        }

        private void OwnerSearchAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            string selectedUserName = args.QueryText.ToString();
            if (selectedUserName != "")
            {
                ZUser user = args.ChosenSuggestion as ZUser;
                if (user == null)
                    return;
                if (autoSuggestBox.Tag.ToString() == "Owner")
                {
                    _Project.OwnerID = user.Id;
                    _Project.Owner = user;
                    SelectedOwner = user;
                    OnPropertyChanged(nameof(SelectedOwner));
                    _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.OwnerID, _Project.OwnerID);
                    if (!_Project.UserCollection.Any(u => u.Id == user.Id))
                    {
                        _Project.UserCollection.Add(user);
                        _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.AddUser, user.Id);
                    }
                    CollapseOwnerSearchAutoSuggestBox();
                }
                else if (autoSuggestBox.Tag.ToString() == "Users")
                {
                    if (!_Project.UserCollection.Any(u => u.Id == user.Id))
                    {
                        _Project.UserCollection.Add(user);
                        _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.AddUser, user.Id);
                        CollapseUserSearchAutoSuggestBox();
                    }
                }
                
                autoSuggestBox.Text = String.Empty;
                autoSuggestBox.ItemsSource = null;
                autoSuggestBox.IsFocusEngaged = false;
            }
        }

        private void OwnerSearchAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ZUser selectedUser = (ZUser)args.SelectedItem;

            if (selectedUser != null)
            {
                sender.Text = (string)selectedUser.Name;
            }
            else if (selectedUser == null)
            {
                sender.Text = "";
            }
        }

        private void OwnerSearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.ToString() != "")
                {
                    if (autoSuggestBox.Tag.ToString() == "Owner")
                    {
                        _GetUsersVM.GetUsers(UsersType.ProjectNameSearch, sender.Text, _Project.ID, sender);
                    }
                    else if (autoSuggestBox.Tag.ToString() == "Users")
                    {
                        _GetUsersVM.GetUsers(UsersType.NameSearch, sender.Text, -1, sender);
                    }
                }
            }
        }
       
        private void StartDateTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StartDateCalendarDatePicker.IsCalendarOpen = true;
        }
        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;
                if (dateTimeOffset.DateTime > _Project.EndDate && _Project.EndDate!=DateTime.MinValue)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("End Date should be greater than Start Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                if (_Project.StartDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _Project.StartDate = dateTimeOffset.DateTime;
                    _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.StartDate, _Project.StartDate);
                }
            }
            else
            {
                _Project.StartDate = DateTime.MinValue;
                _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.StartDate, _Project.StartDate);
            }
        }
        private void EndDateTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            EndDateCalendarDatePicker.IsCalendarOpen = true;
        }
        private void EndDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;

                if (dateTimeOffset.DateTime < _Project.StartDate)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("End Date should be greater than Start Date",Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                if (_Project.EndDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _Project.EndDate = dateTimeOffset.DateTime;
                    _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.EndDate, _Project.EndDate);
                }
            }
            else
            {
                _Project.EndDate = DateTime.MinValue;
                _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.EndDate, _Project.EndDate);
            }
        }

        private void UpdateDescriptionTextBlockLostFocusEvent(string descriptionText)
        {
            _Project.Description=descriptionText;
            _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.Description, _Project.Description);
        }

        private void ProjectDetailPagePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _MilestoneCollection.Clear();
            _TaskCollection.Clear();
            ProjectsDetailDescriptionControl.DescriptionRichEditBoxLostFocus -= UpdateDescriptionTextBlockLostFocusEvent;
        }
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

       
    }

    
}
