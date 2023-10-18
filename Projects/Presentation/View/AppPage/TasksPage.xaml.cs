using Microsoft.UI.Xaml.Controls;
using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.View.AppContentDialog;
using Projects.Presentation.View.AppUserControl;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using ZohoProjects;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TasksPage : Page,IGetTasks,IEditTask,IDeleteTasks,INotifyPropertyChanged
    {

        public CoreDispatcher ZCoreDispatcher { get; }
        
        EventNotification _EventNotification = EventNotification.GetInstance();
        List<ZTaskObj> _TaskList= new List<ZTaskObj>();
        List<ZUser> _UserList = new List<ZUser>();
        ObservableCollection<ZTaskObj> _TaskCollection = new ObservableCollection<ZTaskObj>();
        ObservableCollection<ZTaskObj> _CheckedTaskCollection= new ObservableCollection<ZTaskObj> { };
        ObservableCollection<string> _AppliedFilterCollection = new ObservableCollection<string>();
        bool _IsSearchFilterApplied = false;
        GetTasksVM _GetTasksVM;
        EditTaskVM _EditTaskVM;
        DeleteTasksVM _DeleteTaskVM;
       
        TasksType SelectedTasksType = TasksType.All;
        public TasksPage()
        {
            this.InitializeComponent();

            ZCoreDispatcher = Dispatcher;
            _GetTasksVM = new GetTasksVM(this);
            _EditTaskVM = new EditTaskVM(this);
            _DeleteTaskVM=new DeleteTasksVM(this);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
                SelectedTasksType = (TasksType)e.Parameter;
            LoadTasksType();
        }
        void LoadTasksType()
        {
            HeaderTextBlock.Text = GetTasksTypeString(SelectedTasksType);
            _GetTasksVM.GetTasks(SelectedTasksType, null);
        }
        void LoadDataToCollection(IEnumerable<ZTaskObj> tasks)
        {
            _TaskCollection.Clear();
            foreach (ZTaskObj task in tasks)
            {
                _TaskCollection.Add(task);
            }
            if (ClassicViewControl != null)
                ClassicViewControl.LoadTaskGroups();
            if (KanbanViewControl != null)
                KanbanViewControl.LoadKanbanTaskCollections();
        }
       
        public void LoadTasks(TasksType tasksType, IEnumerable<ZTaskObj> tasks)
        {
            PlainViewControl.IsTasksLoaded = true;
            _TaskList.Clear();
            _TaskCollection.Clear();
            TasksFilterControl.TaskList.Clear();
            foreach (ZTaskObj task in tasks)
            {
                TasksFilterControl.TaskList.Add(task);
                _TaskList.Add(task);
                _TaskCollection.Add(task);
            }
            
            if (ClassicViewControl != null)
                ClassicViewControl.LoadTaskGroups();
            if (KanbanViewControl != null)
                KanbanViewControl.LoadKanbanTaskCollections();
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
            _UserList.Clear();
            foreach(ZUser user in users)
            {
                _UserList.Add(user);
            }
        }
        public void LoadFilterdTasks(IEnumerable<ZTaskObj> tasks)
        {
            _TaskList.Clear();
            _TaskCollection.Clear();
            foreach (ZTaskObj task in tasks)
            {
                _TaskList.Add(task);
                _TaskCollection.Add(task);
            }
        }
        void UpdateTasksFilteredEvent(IEnumerable<ZTaskObj> tasks)
        {
            LoadFilterdTasks(tasks);
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        private void PagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            _CheckedTaskCollection.CollectionChanged += CheckedTaskCollectionChanged;
            _EventNotification.NewTaskAdded += UpdateNewTaskAddedEvent;
            _EventNotification.TaskChecked += UpdateTaskCheckedEvent;
            _EventNotification.TaskUnchecked += UpdateTaskUncheckedEvent;
            TasksFilterControl.TasksFilteredEvent += UpdateTasksFilteredEvent;
            _EventNotification.CancelFilter += UpdateCancelFilterEvent;
            _EventNotification.CloseFilter += UpdateCloseFilterEvent;
            _EventNotification.AppliedFilters += UpdateAppliedFiltersEvent;
            _EventNotification.TasksDeleted += UpdateTasksDeleted;
            StartDateCalendarDatePicker.Date = DateTime.Today.Date;
            DueDateCalendarDatePicker.Date = DateTime.Today.Date.AddDays(1);
        }
        void UpdateNewTaskAddedEvent(ZTaskObj task)
        {
            TasksFilterControl.TaskList.Add(task);
            _TaskList.Add(task);
            _TaskCollection.Add(task);

            OpenTasksContentPage();
        }

        private void HeaderMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            SelectedTasksType = (TasksType)Enum.Parse(typeof(TasksType), menuFlyout.Tag.ToString());
            LoadTasksType();
        }
        string GetTasksTypeString(TasksType tasksType)
        {
            switch (tasksType)
            {
                case TasksType.DueToday:
                    return "Due Today";
                case TasksType.MyOpen:
                    return "My Open";
                case TasksType.MyClosed:
                    return "My Closed";
                case TasksType.MyOverdue:
                    return "My Overdue";
                case TasksType.TodayAssigned:
                    return "Today Assigned";
                case TasksType.CreatedByMe:
                    return "Created By Me";
                default:
                    return tasksType.ToString();
            }

        }
        private void FilterPropertyCancelMethod_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            _AppliedFilterCollection.Remove(button.DataContext.ToString());
            TasksFilterControl.CancelFilterProperty((FilterProperty)Enum.Parse(typeof(FilterProperty), button.DataContext.ToString()));
        }
        private void CloseFilterPanelButton_Click(object sender, RoutedEventArgs e)
        {
            TasksPageSpiltView.IsPaneOpen = false;
        }
        void UpdateAppliedFiltersEvent(List<FilterProperty> appliedFilter)
        {
            _AppliedFilterCollection.Clear();
            foreach (var filter in appliedFilter)
            {
                _AppliedFilterCollection.Add(filter.ToString());
            }
        }
        void UpdateCancelFilterEvent()
        {
            CancelFilter();
        }
        void UpdateCloseFilterEvent()
        {
            TasksPageSpiltView.IsPaneOpen = false;
        }
        private void CancelFilterPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CancelFilter();
        }
        void CancelFilter()
        {
            CancelFilterPanel.Visibility = Visibility.Collapsed;
            SearchAutoSuggestBox.Text = string.Empty;
            TasksPageSpiltView.IsPaneOpen = false;
            _AppliedFilterCollection.Clear();
            if (TasksFilterControl.GetIsFilterApplied())
            {
                TasksFilterControl.RefreahFilters();
                _GetTasksVM.GetTasks(SelectedTasksType, null);
            }
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksPageSpiltView.IsPaneOpen)
            {
                TasksPageSpiltView.IsPaneOpen = false;
            }
            else
            {
                if (_IsSearchFilterApplied)
                {
                    SearchAutoSuggestBox.Text = string.Empty;
                }
                TasksPageSpiltView.IsPaneOpen = true;
            }
        }

      
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsAddNewTaskPageOpen == false)
            {
                if (NewTaskPanel == null)
                {
                    this.FindName("NewTaskPanel");
                }
                NewTaskPanel.Visibility = Visibility.Visible;
                TasksPageContentPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                _EventNotification.InvokeShowNotificationWithSeverity("Add Task page has already been opened", InfoBarSeverity.Informational);
            }
        }
       
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            
            Button clickedButton = (Button)sender;
            foreach (Button button in ViewStyleButtonPanel.Children)
            {
                if (button == clickedButton)
                    button.Style = (Style)this.Resources["PageSymbolAccentButtonStyle"];
                else
                    button.Style = (Style)this.Resources["PageSymbolWindows11ButtonStyle"];
            }
            ChangeView(clickedButton);
        }
        void ChangeView(Button clickedButton)
        {
            TaskViewType changedViewType = (TaskViewType)Enum.Parse(typeof(TaskViewType),clickedButton.Tag.ToString());

            if (clickedButton.Tag.ToString() == "Plain")
            {
                this.FindName("PlainViewPanel");
                //this.UnloadObject(ClassicViewPanel);
                //this.UnloadObject(KanbanViewPanel);
            }
            else if (clickedButton.Tag.ToString() == "Classic")
            {
                this.FindName("ClassicViewPanel");
                //this.UnloadObject(PlainViewPanel);
                //this.UnloadObject(KanbanViewPanel);
            }
            else if (clickedButton.Tag.ToString() == "Kanban")
            {
                this.FindName("KanbanViewPanel");
                //this.UnloadObject(ClassicViewPanel);
                //this.UnloadObject(PlainViewPanel);
            }
            foreach (Grid grid in PanelsGrid.Children)
            {
                if (grid != null)
                {
                    if (changedViewType == (TaskViewType)Enum.Parse(typeof(TaskViewType), grid.Tag.ToString()))
                    {
                        grid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grid.Visibility = Visibility.Collapsed;
                    }
                }
            }

        }
        private void SearchAutoSuggestBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void SearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                SearchTasks(SearchAutoSuggestBox.Text);
               
            }
        }
        void SearchTasks(string searchedText)
        {
            if (String.IsNullOrEmpty(searchedText))
            {
                LoadDataToCollection(_TaskList);
                _IsSearchFilterApplied = false;
                return;
            }
            var searchResultTasks = new List<ZTaskObj>();
            var splitText = searchedText.ToLower().Split(" ");
            foreach (var task in _TaskList)
            {
                var found = splitText.All((key) =>
                {
                    return task.Name.ToLower().Contains(key);
                });
                if (found)
                {
                    searchResultTasks.Add(task);
                }
            }
            LoadDataToCollection(searchResultTasks);
            _IsSearchFilterApplied = true;
        }
        void CheckedTaskCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_CheckedTaskCollection.Count < 1)
            {
                MultiSelectPopup.IsOpen= false;
            }
            else
            {
                MultiSelectPopup.IsOpen = true;
            }
        }
        
        void UpdateTaskCheckedEvent(ZTaskObj task)
        {
            if (!_CheckedTaskCollection.Contains(task))
                _CheckedTaskCollection.Add(task);
        }
        void UpdateTaskUncheckedEvent(ZTaskObj task)
        {
            if (_CheckedTaskCollection.Contains(task))
                _CheckedTaskCollection.Remove(task);
        }

        string GetTaskCheckedCount(int count)
        {
            if (count < 2)
            {
                return count + " Task Selected";
            }
            return count + " Tasks Selected";
        }
       
        private void MultiSelectPopupCancelButton(object sender, RoutedEventArgs e)
        {
            MultiSelectPopup.IsOpen = false;
            _EventNotification.InvokeUncheckAllTasksEvent();
        }
        private async void MultiSelectDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteConformationContentDialog contentDialog = new DeleteConformationContentDialog();
            contentDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            int checkedTasksCount = _CheckedTaskCollection.Count;
            if (checkedTasksCount < 2)
            {
                contentDialog.Title = "Are you sure about Delete this Task ?";
            }
            else
            {
                contentDialog.Title = "Are you sure about Delete these " + checkedTasksCount + " Tasks ?";
            }
            await contentDialog.ShowAsync();
            ContentDialogResult result = contentDialog.Result;
            if (result == ContentDialogResult.Primary)
            {
                _DeleteTaskVM.DeleteTask(_CheckedTaskCollection);
               
                _EventNotification.InvokeTasksDeletedEvent(_CheckedTaskCollection);
                MultiSelectPopup.IsOpen = false;
            }
        }
        void UpdateTasksDeleted(IList<ZTaskObj> tasks)
        {
            foreach (ZTaskObj task in _CheckedTaskCollection)
            {
                _TaskCollection.Remove(task);
                _TaskList.Remove(task);
                TasksFilterControl.TaskList.Remove(task);
            }
        }
        private void PriorityMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = (MenuFlyoutItem)sender;
            Priority priority = (Priority)Enum.Parse(typeof(Priority), menuFlyoutItem.Text);
            foreach (ZTaskObj task in _CheckedTaskCollection)
            {
                task.Priority = priority;
                _EditTaskVM.EditTaskProperty(task.ID, TaskPropertyEditType.Priority, task.Priority);
            }
        }

        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = (MenuFlyoutItem)sender;
            ZTaskStatus status = (ZTaskStatus)Enum.Parse(typeof(ZTaskStatus), menuFlyoutItem.Text.Replace(" ", ""));

            foreach (ZTaskObj task in _CheckedTaskCollection)
            {
                task.Status = status;
                _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.Status,task);
                _EditTaskVM.EditTaskProperty(task.ID, TaskPropertyEditType.Status, task.Status);
            }
        }
        private void MultiSelectStartDateRadioButtonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Panel panel = sender as Panel;
            RadioButton radioButton = panel.Children[0] as RadioButton;
            radioButton.IsChecked = true;
            StartDateCalendarDatePicker.Visibility = Visibility.Collapsed;
            StartDateNumberBox.Visibility = Visibility.Visible;
        }

        private void MultiSelectStartDateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Content.ToString() == "Date")
            {
                StartDateCalendarDatePicker.Visibility = Visibility.Visible;
                StartDateNumberBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                StartDateCalendarDatePicker.Visibility = Visibility.Collapsed;
                StartDateNumberBox.Visibility = Visibility.Visible;
            }
            MultiSelectStartDateApplyButton.Tag = radioButton.Content.ToString();
        }
        private void MultiSelectEndDateRadioButtonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Panel panel = sender as Panel;
            RadioButton radioButton = panel.Children[0] as RadioButton;
            radioButton.IsChecked = true;
            DueDateCalendarDatePicker.Visibility = Visibility.Collapsed;
            DueDateNumberBox.Visibility = Visibility.Visible;
        }
        private void MultiSelectDueDateRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Content.ToString() == "Date")
            {
                DueDateCalendarDatePicker.Visibility = Visibility.Visible;
                DueDateNumberBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                DueDateCalendarDatePicker.Visibility = Visibility.Collapsed;
                DueDateNumberBox.Visibility = Visibility.Visible;
            }
            MultiSelectDueDateApplyButton.Tag = radioButton.Content.ToString();
        }

        private void MultiSelectStartDateApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string method = button.Tag.ToString();
            DateTime newDate;
            switch (method)
            {
                case "Prepone":
                    foreach (ZTask task in _CheckedTaskCollection)
                    {
                        if (task.StartDate != DateTime.MinValue)
                        {
                            newDate = task.StartDate.AddDays(-(StartDateNumberBox.Value));
                            UpdateStartDate(task, newDate);
                        }
                    }
                    break;
                case "Postpone":
                    foreach (ZTask task in _CheckedTaskCollection)
                    {
                        if (task.StartDate != DateTime.MinValue)
                        {
                            newDate = task.StartDate.AddDays((StartDateNumberBox.Value));
                            UpdateStartDate(task, newDate);
                        }
                    }
                    break;
                case "Date":
                    foreach (ZTaskObj task in _CheckedTaskCollection)
                    {
                        if (StartDateCalendarDatePicker.Date != null)
                        {
                            DateTimeOffset dateTimeOffset = (DateTimeOffset)StartDateCalendarDatePicker.Date;
                            newDate = dateTimeOffset.DateTime.Date;
                            UpdateStartDate(task,newDate);
                        }
                       
                        
                    }
                    break;
            }
            MultiSelectStartDateFlyout.Hide();
        }
        void UpdateStartDate(ZTask task, DateTime newDate)
        {
            if (task.StartDate != newDate && newDate <= task.EndDate)
            {
                task.StartDate = newDate;
                _EditTaskVM.EditTaskProperty(task.ID, TaskPropertyEditType.StartDate, task.StartDate);
            }
        }
        private void MultiSelectDueDateApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string method = button.Tag.ToString();
            DateTime newDate;
            switch (method)
            {
                case "Prepone":
                    foreach (ZTask task in _CheckedTaskCollection)
                    {
                        if (task.EndDate != DateTime.MinValue)
                        {
                            newDate = task.EndDate.AddDays(-(DueDateNumberBox.Value));
                            UpdateDueDate(task, newDate);
                        }
                    }
                    break;
                case "Postpone":
                    foreach (ZTask task in _CheckedTaskCollection)
                    {
                        if (task.StartDate != DateTime.MinValue)
                        {
                            newDate = task.EndDate.AddDays((DueDateNumberBox.Value));
                            UpdateDueDate(task, newDate);
                        }
                    }
                    break;
                case "Date":
                    foreach (ZTask task in _CheckedTaskCollection)
                    {
                        if (DueDateCalendarDatePicker.Date != null)
                        {
                            DateTimeOffset dateTimeOffset = (DateTimeOffset)DueDateCalendarDatePicker.Date;
                            newDate = dateTimeOffset.Date;
                            UpdateDueDate(task,newDate);
                        }
                       
                    }
                    break;

            }
            MultiSelectDueDateFlyout.Hide();
        }
        void UpdateDueDate(ZTask task, DateTime newDate)
        {
            if (task.EndDate != newDate && newDate >= task.StartDate)
            {
                task.EndDate = newDate;
                _EditTaskVM.EditTaskProperty(task.ID, TaskPropertyEditType.EndDate, task.EndDate);
            }
        }
        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate == null)
                sender.Date = args.OldDate;
        }

        private void DueDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate == null)
                sender.Date = args.OldDate;
        }
        private void MultiSelectOwnerFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {

        }

        private void MultiSelectOwnerRadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            string radioButtonContent = radioButton.Content.ToString();
            if (radioButtonContent == "Replace")
            {
                ReplacePanel.Visibility = Visibility.Visible;
                AddPanel.Visibility = Visibility.Collapsed;
            }
            else if (radioButtonContent == "Add")
            {
                ReplacePanel.Visibility = Visibility.Collapsed;
                AddPanel.Visibility = Visibility.Visible;
            }
            else if (radioButtonContent == "Unassign")
            {
                ReplacePanel.Visibility = Visibility.Collapsed;
                AddPanel.Visibility = Visibility.Collapsed;
            }
            MultiSelectOwnerApplyButton.Tag = radioButtonContent;
        }
        private void MultiSelectOwnerRadioButtonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Panel panel = sender as Panel;
            RadioButton radioButton = panel.Children[0] as RadioButton;
            radioButton.IsChecked = true;
        }
        private void MultiSelectOwnerApplyButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Replace":
                    if (CurrentOwner != null && ReplaceOwner != null && CurrentOwner.Id != ReplaceOwner.Id)
                    {
                        foreach (ZTaskObj task in _CheckedTaskCollection)
                        {
                            if (task.OwnerID == CurrentOwner.Id)
                            {
                                task.OwnerID = ReplaceOwner.Id;
                                task.Owner = ReplaceOwner;
                                _EditTaskVM.EditTaskProperty(task.ID, TaskPropertyEditType.OwnerID, task.OwnerID);
                            }
                        }
                    }
                    break;
                case "Add":
                    if (AddOwner != null)
                    {
                        foreach (ZTaskObj task in _CheckedTaskCollection)
                        {
                            task.OwnerID = AddOwner.Id;
                            task.Owner = AddOwner;
                            _EditTaskVM.EditTaskProperty(task.ID, TaskPropertyEditType.OwnerID, task.OwnerID);
                        }
                    }
                    break;
            }

        }

        ZUser CurrentOwner;
        ZUser AddOwner;
        ZUser ReplaceOwner;

        private void OwnerSearchAutoSuggestionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<ZUser>(3);

                if (sender.Text.ToString() != "")
                {

                    var splitText = sender.Text.ToLower().Split(" ");
                    foreach (var user in _UserList)
                    {
                        var found = splitText.All((key) =>
                        {
                            return user.Name.ToLower().Contains(key);
                        });
                        if (found)
                        {
                            suitableItems.Add(user);
                        }
                        if (suitableItems.Count > 3)
                            break;
                    }

                }

                else if (sender.Text.ToString() == "")
                {
                    sender.ItemsSource = null;
                }
                sender.ItemsSource = suitableItems;
            }
        }

        private void OwnerSearchAutoSuggestionBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            string selectedUserName = args.QueryText.ToString();
            if (selectedUserName != "")
            {
                foreach (ZUser user in _UserList)
                {
                    if (user.Name == selectedUserName)
                    {
                        if (autoSuggestBox.Tag.ToString() == "CurrentOwner")
                        {
                            CurrentOwner = user;
                            CurrentOwnerControl.Visibility = Visibility.Visible;
                            CurrentOwnerAutoSuggestionBox.Visibility = Visibility.Collapsed;
                            OnPropertyChanged("CurrentOwner");
                        }
                        else if (autoSuggestBox.Tag.ToString() == "ReplaceOwner")
                        {
                            ReplaceOwner = user;
                            ReplaceOwnerControl.Visibility = Visibility.Visible;
                            ReplaceOwnerAutoSuggestionBox.Visibility = Visibility.Collapsed;
                            OnPropertyChanged("ReplaceOwner");
                        }
                        else if (autoSuggestBox.Tag.ToString() == "AddOwner")
                        {
                            AddOwner = user;
                            AddOwnerControl.Visibility = Visibility.Visible;
                            AddOwnerAutoSuggestionBox.Visibility = Visibility.Collapsed;
                            OnPropertyChanged("AddOwner");
                        }


                        break;
                    }
                }

                autoSuggestBox.Text = String.Empty;
                autoSuggestBox.ItemsSource = null;
                autoSuggestBox.IsFocusEngaged = false;
            }
        }

        private void OwnerSearchAutoSuggestionBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            ZUser selectedUser = (ZUser)args.SelectedItem;

            if (selectedUser != null)
            {
                autoSuggestBox.Text = (string)selectedUser.Name;
            }
            else if (selectedUser == null)
            {
                autoSuggestBox.Text = "";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void AddOwnerControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            AddOwnerControl.Visibility = Visibility.Collapsed;
            AddOwnerAutoSuggestionBox.Visibility = Visibility.Visible;
        }

        private void CurrentOwnerControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CurrentOwnerControl.Visibility = Visibility.Collapsed;
            CurrentOwnerAutoSuggestionBox.Visibility = Visibility.Visible;
        }

        private void ReplaceOwnerControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ReplaceOwnerControl.Visibility = Visibility.Collapsed;
            ReplaceOwnerAutoSuggestionBox.Visibility = Visibility.Visible;
        }
      

        private void CheckAllTasksButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Tag.ToString() == "Check")
            {
                _EventNotification.InvokeCheckAllTasksEvent();
                button.Content = "Uncheck All";
                button.Tag= "Uncheck";
            }
            else
            {
                _EventNotification.InvokeUncheckAllTasksEvent();
                button.Content = "Check All";
                button.Tag = "Check";
            }
        }
        void OpenTasksContentPage()
        {
            NewTaskPanel.Visibility = Visibility.Collapsed;
            PageAddNewTaskContentControl.Refresh();
            TasksPageContentPanel.Visibility = Visibility.Visible;
            App.IsAddNewMilestonePageOpen = false;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OpenTasksContentPage();
        }

        private async void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {

            AppWindow addNewTaskWindow = await AppWindow.TryCreateAsync();
            addNewTaskWindow.Closed += AddNewTaskWindow_Closed;
            Frame frame = new Frame();
            frame.Navigate(typeof(AddNewTaskInSeparateWindowPage), addNewTaskWindow);
            addNewTaskWindow.Title = "Add Task";
            addNewTaskWindow.TitleBar.BackgroundColor = Colors.Transparent;
            addNewTaskWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            addNewTaskWindow.TitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            addNewTaskWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Transparent;
            addNewTaskWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            addNewTaskWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            ElementCompositionPreview.SetAppWindowContent(addNewTaskWindow, frame);
            await addNewTaskWindow.TryShowAsync();
            App.IsAddNewTaskPageOpen = true;
            OpenTasksContentPage();
        }

        private void AddNewTaskWindow_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
            App.IsAddNewTaskPageOpen=false;
        }

        private void PagePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _CheckedTaskCollection.CollectionChanged -= CheckedTaskCollectionChanged;
            _EventNotification.NewTaskAdded -= UpdateNewTaskAddedEvent;
            _EventNotification.TaskChecked -= UpdateTaskCheckedEvent;
            _EventNotification.TaskUnchecked -= UpdateTaskUncheckedEvent;
            TasksFilterControl.TasksFilteredEvent -= UpdateTasksFilteredEvent;
            _EventNotification.CancelFilter -= UpdateCancelFilterEvent;
            _EventNotification.CloseFilter -= UpdateCloseFilterEvent;
            _EventNotification.AppliedFilters -= UpdateAppliedFiltersEvent;
            _EventNotification.TasksDeleted -= UpdateTasksDeleted;
        }
        public void LoadTask(ZTaskObj task)
        {
            throw new NotImplementedException();
        }

       
    }

    public enum TaskViewType {
        Plain,
        Classic,
        Kanban
    }

  
}
