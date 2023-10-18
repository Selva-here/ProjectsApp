using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Core.Helper;
using Projects.Notification;
using Projects.Presentation.View.AppUserControl;
using Projects.Presentation.View.AppUserControl.TaskUserControl;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
    public sealed partial class MilestoneDetailPage : Page,INotifyPropertyChanged,IEditMilstone,IGetUsers,IGetTasks
    {
        public MilestoneObj _Milestone { get; set; }
        public CoreDispatcher ZCoreDispatcher { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        EventNotification _EventNotification=EventNotification.GetInstance();
        ObservableCollection<ZTaskObj> _TaskCollection = new ObservableCollection<ZTaskObj>();
        bool _IsTasksLoaded = false;
        EditMilstoneVM _EditMilstoneVM;
        GetUsersVM _GetUsersVM;
        GetTasksVM _GetTasksVM;
        ZUser _SelectedOwner;

        public MilestoneDetailPage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _EditMilstoneVM = new EditMilstoneVM(this);
            _GetUsersVM = new GetUsersVM(this);
            _GetTasksVM = new GetTasksVM(this);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _Milestone = (MilestoneObj)e.Parameter;
           MilestoneDetailDescriptionControl.DesciptionText = _Milestone.Description;
            _SelectedOwner =_Milestone.Owner;
            
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
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
            Debug.WriteLine(taskPercentageKeyValuePair.Count());
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


            Debug.WriteLine(taskStatusKeyValuePair.Count());
            foreach (var pair in taskStatusKeyValuePair)
            {
                taskStatusChartWidgetControl.ValueCollection.Add(pair);
            }

            DashboardAdaptiveGridView.Items.Add(overdueTasksListWidgetControl);
            DashboardAdaptiveGridView.Items.Add(dueTodayTasksListWidgetControl);
            DashboardAdaptiveGridView.Items.Add(taskPercentageChartWidgetControl);
            DashboardAdaptiveGridView.Items.Add(taskStatusChartWidgetControl);
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
           throw new NotImplementedException();
        }
        private void MilestoneDetailPagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            MilestoneDetailDescriptionControl.DescriptionRichEditBoxLostFocus += UpdateDescriptionTextoxLostFocusEvent;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MilestonesPage));
        }
      
        private void MilestoneDetailPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                this.FindName("TasksPanel");
                TaskViewOptionButtonsPanel.Visibility = Visibility.Visible;
                if (_IsTasksLoaded == true)
                    TaskPlainViewControl.IsTasksLoaded = true;
                else
                {
                    _GetTasksVM.GetTasks(TasksType.Milestone, _Milestone.ID);
                }
            }
            else if (selectedpivotItem.Name == "Dashboard")
            {
                this.FindName("DashboardPanel");
                if (_IsTasksLoaded == false)
                {
                    _GetTasksVM.GetTasks(TasksType.Milestone, _Milestone.ID);
                }
            }
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
            
            if (clickedButton.Tag.ToString() == "Plain")
            {
                this.FindName("TaskPlainViewControl");
                TaskPlainViewControl.Visibility = Visibility.Visible;
                TaskKanbanViewControl.Visibility = Visibility.Collapsed;
            }
            else if (clickedButton.Tag.ToString() == "Kanban")
            {
                this.FindName("TaskKanbanViewControl");
                TaskPlainViewControl.Visibility = Visibility.Collapsed;
                TaskKanbanViewControl.Visibility = Visibility.Visible;
            }
        }
       

        private void NameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NameTextBlock.Visibility = Visibility.Collapsed;
            NameTextBox.Visibility = Visibility.Visible;
        }
        bool IsNameTextBlock = false;
        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IsNameTextBlock = true;
        }
        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsNameTextBlock)
            {
                _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.Name, _Milestone.Project.Name);
            }
            CollapseNameTextBox();
        }
        private void NameTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                CollapseNameTextBox();
            }
        }
        void CollapseNameTextBox()
        {
           NameTextBox.Visibility = Visibility.Collapsed;
           NameTextBlock.Visibility = Visibility.Visible;
        }
     
        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            _Milestone.Status = (MilestoneStatus)Enum.Parse(typeof(MilestoneStatus), menuFlyoutItem.Text.ToString().Replace(" ",""));
            _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.Status, _Milestone.Status);
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
                if (dateTimeOffset.DateTime.Date > _Milestone.EndDate && _Milestone.EndDate != DateTime.MinValue)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("Start Date should be lesser than End Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                else if (_Milestone.StartDate != dateTimeOffset.DateTime.Date)
                {
                    _Milestone.StartDate = dateTimeOffset.DateTime.Date;
                    _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.StartDate, _Milestone.StartDate);
                }
            }
            else
            {
                _Milestone.StartDate = DateTime.MinValue;
                _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.StartDate, _Milestone.StartDate);
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
                if (dateTimeOffset.DateTime.Date < _Milestone.StartDate)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("End Date should be greater than Start Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                else
                {
                    _Milestone.EndDate = dateTimeOffset.DateTime.Date;
                    _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.EndDate, _Milestone.EndDate);
                }
            }
            else
            {
                _Milestone.EndDate = DateTime.MinValue;
                _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.EndDate, _Milestone.EndDate);
            }

            
        }
        private void OwnerTexBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CollapseOwnerItemPanel();
        }
        
        void CollapseOwnerItemPanel()
        {
            OwnerSearchAutoSuggestBox.Visibility = Visibility.Visible;
            OwnerItemPanel.Visibility = Visibility.Collapsed;
        }
        void CollapseOwnerSearchAutoSuggestBox()
        {
            OwnerSearchAutoSuggestBox.Visibility = Visibility.Collapsed;
            OwnerSearchAutoSuggestBox.Text = string.Empty;
            OwnerItemPanel.Visibility = Visibility.Visible;
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
                ZUser user=args.ChosenSuggestion as ZUser;
                if (user != null)
                {
                    _Milestone.Owner = user;
                    _SelectedOwner = user;
                    OnPropertyChanged(nameof(_SelectedOwner));
                    _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.OwnerID, _Milestone.OwnerID);
                    CollapseOwnerSearchAutoSuggestBox();
                    autoSuggestBox.Text = String.Empty;
                    autoSuggestBox.ItemsSource = null;
                    autoSuggestBox.IsFocusEngaged = false;
                }
            }
        }

        private void OwnerSearchAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ZUser selectedUser = (ZUser)args.SelectedItem;

            if (selectedUser != null)
            {
                OwnerSearchAutoSuggestBox.Text = (string)selectedUser.Name;
            }
            else if (selectedUser == null)
            {
                OwnerSearchAutoSuggestBox.Text = "";
            }
        }

        private void OwnerSearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {

                if (sender.Text.ToString() != "")
                {
                    _GetUsersVM.GetUsers(UsersType.ProjectNameSearch, sender.Text, _Milestone.ProjectID, sender);
                }
                else if (sender.Text.ToString() == "")
                {
                    sender.ItemsSource = null;
                }
                
            }
        }
       
        private void UpdateDescriptionTextoxLostFocusEvent(string descriptionText)
        {
            _Milestone.Description = descriptionText;
            _EditMilstoneVM.EditMilestoneProperty(_Milestone.ID, MilestonePropertyEditType.Description, _Milestone.Description);
        }
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void MilestoneDetailPagePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            MilestoneDetailDescriptionControl.DescriptionRichEditBoxLostFocus -= UpdateDescriptionTextoxLostFocusEvent;
        }

     
    }
}
