using Microsoft.Toolkit.Collections;
using Microsoft.UI.Xaml.Controls;
using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Core.Helper;
using Projects.Notification;
using Projects.Presentation.View.AppPage;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ZohoProjects;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TaskDetailViewPage : Page,INotifyPropertyChanged,IGetUsers,IEditTask
    {
        public CoreDispatcher ZCoreDispatcher { get; }
        ObservableCollection<ZTaskObj> _TaskCollection { get; set; }
       ObservableCollection<ZUser> _UserCollection=new ObservableCollection<ZUser> { };
        EventNotification _EventNotification=EventNotification.GetInstance();
       ZTaskObj _ZTask { 
            get { return this.DataContext as ZTaskObj; } 
            set { this.DataContext = value; }
        }
        ZUser SelectedOwner { get; set; }
        GetUsersVM _GetUsersVM;
        EditTaskVM _EditTaskVM;
       public TaskDetailViewPage()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            _EventNotification.PassDataToTaskDetailsPageEvent += UpdatePassDataToTaskDetailsPageEvent;
            ZCoreDispatcher = Dispatcher;
            _GetUsersVM=new GetUsersVM(this);
            _EditTaskVM=new EditTaskVM(this);
            SetSplitView("No");
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            _ZTask = (ZTaskObj)e.Parameter;
            TaskDetailDescriptionControl.DesciptionText = _ZTask.Description;
           SelectedOwner = _ZTask.Owner;
        }
        public void LoadUser(ZUser user)
        {
            throw new NotImplementedException();
        }

        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }

        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        private void TaskDetailViewPanel_Loaded(object sender, RoutedEventArgs e)
        {
            TaskDetailDescriptionControl.DescriptionRichEditBoxLostFocus += UpdateDescriptionTextoxLostFocusEvent;
            _EventNotification.SubTasksDeleted += UpdateSubTasksDeletedEvent;
        }

        private void UpdatePassDataToTaskDetailsPageEvent(IEnumerable<ZTaskObj> tasks)
        {
            if (tasks.Count() > 1)
            {
                _TaskCollection = (ObservableCollection<ZTaskObj>)tasks;
                TasksListView.SelectedItem = _ZTask;
            }
            else
            {
                SetSplitView("No");
            }
        }
        private void TasksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ZTask = (ZTaskObj)e.AddedItems.FirstOrDefault();
            OnPropertyChanged("_ZTask");
        }
        private void TasksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            _ZTask = (ZTaskObj)e.ClickedItem;
            OnPropertyChanged("_ZTask");
        }
        private void TaskDetailViewPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 920)
            {
                SetSplitView("No");
            }
        }
        void SetSplitView(string spiltViewOption)
        {
            if (spiltViewOption == "No")
            {
                TasksListColumn.Width = new GridLength(1, GridUnitType.Star);
                TasksListColumn.MaxWidth = 2000;

                GridSplitter.Visibility = Visibility.Collapsed;
                GridSplitterColumn.Width = new GridLength(0);

                
                TasksListColumn.MinWidth = 0;
                TasksListColumn.Width = new GridLength(0, GridUnitType.Pixel);

                SplitViewButton.Tag = "Vertical";
                SplitViewButton.Content = "\uE784";
            }
            else if (spiltViewOption == "Vertical")
            {
                TasksListColumn.Width = new GridLength(400);
                TasksListColumn.MaxWidth = 500;

                GridSplitter.Visibility = Visibility.Visible;
                GridSplitterColumn.Width = new GridLength(5);

                TasksListColumn.MinWidth = 200;
                TasksListColumn.Width = new GridLength(400, GridUnitType.Pixel);

                SplitViewButton.Tag = "No";
                SplitViewButton.Content = "\uEA37";
            }
        }

        private void SplitViewButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string splitViewOption = (string)button.Tag;
            SetSplitView(splitViewOption);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(TasksPage));
        }
       
        private void NameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            NameTextBlock.Visibility = Visibility.Collapsed;
            NameTextBox.Visibility = Visibility.Visible;
        }
        bool _IsNameTextChanged=false;
        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _IsNameTextChanged=true;
        }
        private void NameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
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
            if (_IsNameTextChanged)
            {
                _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Name, NameTextBox.Text);
            }
            
        }
        private void CompletionPercentageSlider_LostFocus(object sender, RoutedEventArgs e)
        {
            CompletionPercentageTextBlock.Visibility = Visibility.Visible;
            CompletionPercentageSlider.Visibility = Visibility.Collapsed;
            _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Percentage, _ZTask.CompletedPercentage);
            if (_ZTask.CompletedPercentage == 100)
            {
                _ZTask.Status = ZTaskStatus.Closed;
                _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Status, _ZTask.Status);
            }
        }
        private void CompletionPercentageTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {

            if (_ZTask.Status == ZTaskStatus.Closed || _ZTask.Status == ZTaskStatus.Cancelled)
            {
                _EventNotification.InvokeShowNotificationWithSeverity("Can't change the percentage of Closed or Completed Task",InfoBarSeverity.Informational);
            }
            else
            {
                CompletionPercentageTextBlock.Visibility = Visibility.Collapsed;
                CompletionPercentageSlider.Visibility = Visibility.Visible;
            }
        }
        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            _ZTask.Status = (ZTaskStatus)Enum.Parse(typeof(ZTaskStatus), menuFlyout.Text.Replace(" ", ""));
            if(_ZTask.Status==ZTaskStatus.Closed || _ZTask.Status == ZTaskStatus.Closed)
            {
                _ZTask.CompletedPercentage = 100;
                _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Percentage, _ZTask.CompletedPercentage);
            }
            if (_ZTask.Status == ZTaskStatus.Open)
            {
                _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Percentage, _ZTask.CompletedPercentage);
            }
            _EditTaskVM.EditTaskProperty(_ZTask.ID,TaskPropertyEditType.Status,_ZTask.Status);
        }
        private void PriorityMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            _ZTask.Priority = (Priority)Enum.Parse(typeof(Priority), menuFlyout.Text);
            _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Priority, _ZTask.Priority);
        }
        private void StartDatePanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StartDateCalendarDatePicker.IsCalendarOpen= true;
        }
        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;
                if (dateTimeOffset.DateTime > _ZTask.EndDate)
                {
                    StartDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Start Date should be lesser than Due Date.");
                    return;
                }
                if (_ZTask.StartDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZTask.StartDate = dateTimeOffset.DateTime;
                    _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.StartDate, _ZTask.StartDate);
                }
            }
            else
            {
                StartDateCalendarDatePicker.Date= args.OldDate;
                _EventNotification.InvokeShowNotification("Start Date Cant be null");
            }
        }
       
        private void DueDatePanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DueDateCalendarDatePicker.IsCalendarOpen =true;
        }
        private void DueDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;

                if (dateTimeOffset.DateTime < _ZTask.StartDate)
                {
                    DueDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("End Date should be greater than Start Date.");
                    return;
                }
                if (_ZTask.EndDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZTask.EndDate = dateTimeOffset.DateTime;
                    _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.EndDate, _ZTask.EndDate);
                }
            }
            else
            {
                DueDateCalendarDatePicker.Date=args.OldDate;
                _EventNotification.InvokeShowNotification("End Date Cant be null");
            }
        }
        
        private void SelectedOwnerNameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CollapseOwnerItemPanel();
        }
        void CollapseOwnerItemPanel()
        {
            OwnerSearchAutoSuggestBox.Visibility = Visibility.Visible;
            SelectedOwnerPanel.Visibility = Visibility.Collapsed;
        }
        void CollapseOwnerSearchAutoSuggestBox()
        {
            OwnerSearchAutoSuggestBox.Visibility = Visibility.Collapsed;
            OwnerSearchAutoSuggestBox.Text = string.Empty;
            SelectedOwnerPanel.Visibility = Visibility.Visible;
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
                    if (_ZTask.Owner.Id != user.Id)
                    {
                        _ZTask.OwnerID = user.Id;
                        _ZTask.Owner = user;
                        SelectedOwner=user;
                        OnPropertyChanged(nameof(SelectedOwner));
                        _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.OwnerID, _ZTask.OwnerID);
                    }
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
                    _GetUsersVM.GetUsers(UsersType.ProjectNameSearch,sender.Text,_ZTask.ProjectID,sender);
                }
                else if (sender.Text.ToString() == "")
                {
                    sender.ItemsSource = null;
                }
            }
        }



        private void UpdateDescriptionTextoxLostFocusEvent(string descriptionText)
        {
            _ZTask.Description = descriptionText;
            _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.Description, _ZTask.Description);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
      
        private void SubTaskPanel_Loaded(object sender, RoutedEventArgs e)
        {
            SubTaskStartDateCalendarDatePicker.Date = DateTime.Today;
            SubTaskDueDateCalendarDatePicker.Date = DateTime.Today.AddDays(1);
        }
        Priority SubTaskPriority=Priority.None;
        private void SubTaskPriorityMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            SubTaskPriority = (Priority)Enum.Parse(typeof(Priority), menuFlyout.Text);
            OnPropertyChanged(nameof(SubTaskPriority));
        }
        private void SubTaskStartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;
                if (dateTimeOffset.DateTime > SubTaskDueDateCalendarDatePicker.Date)
                {
                    SubTaskStartDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Start Date should be lesser than Due Date.");
                    return;
                }

            }
            else
            {
                SubTaskStartDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("Start Date cant be null");
            }
        }

        private void SubTaskDueDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;

                if (dateTimeOffset.DateTime < SubTaskStartDateCalendarDatePicker.Date)
                {
                    SubTaskDueDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Due Date should be greater than Start Date.");
                    return;
                }

            }
            else
            {
                SubTaskDueDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("Due Date cant be null");
            }
        }
        void UpdateSubTasksDeletedEvent(IList<ZSubTask> subTasks)
        {
            foreach (ZSubTask subTask in subTasks)
            {
                _ZTask.SubTaskCollection.Remove(subTask);
            }
        }
        private void SubTaskTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (SubTaskTextBox.Text.Count() > 1)
                {
                    ZSubTask subTask = new ZSubTask(_ZTask.ID, SubTaskTextBox.Text, SubTaskPriority,((DateTimeOffset)SubTaskStartDateCalendarDatePicker.Date).Date, ((DateTimeOffset)SubTaskDueDateCalendarDatePicker.Date).Date);
                    _ZTask.SubTaskCollection.Add(subTask);
                }
               
            }
        }
        private void TaskDetailViewPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.SubTasksDeleted -= UpdateSubTasksDeletedEvent;
            _EventNotification.PassDataToTaskDetailsPageEvent -= UpdatePassDataToTaskDetailsPageEvent;
            TaskDetailDescriptionControl.DescriptionRichEditBoxLostFocus -= UpdateDescriptionTextoxLostFocusEvent;
        }
    }
}
