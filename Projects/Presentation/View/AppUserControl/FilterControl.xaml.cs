using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ZohoProjects;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl
{
    public sealed partial class FilterControl : UserControl,IGetUsers,INotifyPropertyChanged,IFilterTasks
    {
        public FilterItem CurrentFilterItem { get; set; }
        public List<ProjectObj> _ProjectList { get; set; } = new List<ProjectObj>();
        public List<MilestoneObj> _MilestoneList { get; set; } = new List<MilestoneObj>();
        public List<ZTaskObj> TaskList { get; set; } = new List<ZTaskObj>();
        public List<ProjectObj> _FilteredProjectList { get; set; } = new List<ProjectObj>();
        public List<MilestoneObj> _FilteredMilestoneList { get; set; } = new List<MilestoneObj>();
        public List<ZTaskObj> _FilteredTaskList { get; set; } = new List<ZTaskObj>();
        public bool IsFilterApplied { get; set; }=false;
        public CoreDispatcher ZCoreDispatcher { get; }
        public event Action<IEnumerable<ProjectObj>> ProjectsFilteredEvent;
        public event Action<IEnumerable<MilestoneObj>> MilestonesFilteredEvent;
        public event Action<IEnumerable<ZTaskObj>> TasksFilteredEvent;
        public event PropertyChangedEventHandler PropertyChanged;
        List<FilterProperty> _AppliedFilters=new List<FilterProperty>();
      
        List<FilterPropertyAndValue> _FilterPropertyAndValueList = new List<FilterPropertyAndValue>();
        ZUser SelectedOwner;
       
        bool IsOwnerFilterApplied = false;
        bool IsPriorityFilterApplied = false;
        bool IsStatusFilterApplied = false;
        bool IsPercentageFilterApplied = false;
        bool IsStartDateFilterApplied = false;
        bool IsEndDateFilterApplied = false;
        FilterMethod SelectedFilterMethod = FilterMethod.Any;
        GetUsersVM _GetUsersVM;
        FilterTasksVM _FilterTasksVM;
        public FilterControl()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetUsersVM = new GetUsersVM(this);
            _FilterTasksVM = new FilterTasksVM(this);
            foreach (FilterProperty property in Enum.GetValues(typeof(FilterProperty)))
            {
                _FilterPropertyAndValueList.Add(new FilterPropertyAndValue(property));
            }
        }
        private void FilterControlPanel_Loaded(object sender, RoutedEventArgs e)
        {

        }
      
        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }
        public void LoadTasks(IEnumerable<ZTaskObj> tasks)
        {
            TasksFilteredEvent?.Invoke(tasks);
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        public void LoadUser(ZUser user)
        {
           throw new NotImplementedException();
        }
        public void CancelFilterProperty(FilterProperty filterProperty)
        {
            switch (filterProperty) {
                case FilterProperty.Owner:
                    CancelOwnerFilter();
                    break;
                case FilterProperty.Priority:
                    CancelPriorityFilter();
                    break;
                case FilterProperty.Status:
                    CancelStatusFilter();
                    break;
                case FilterProperty.Percentage:
                    CancelPercentageFilter();
                    break;
                case FilterProperty.StartDate:
                    CancelStartDateFilter();
                    break;
                case FilterProperty.EndDate:
                    CancelEndDateFilter();
                    break;
            }
            Filter();
        }
        public void RefreahFilters()
        {
            CancelOwnerFilter();
            CancelPriorityFilter();
            CancelStatusFilter();
            CancelPercentageFilter();
            CancelStartDateFilter();
            CancelEndDateFilter();
        }
        public bool GetIsFilterApplied()
        {
            if (IsOwnerFilterApplied || IsPriorityFilterApplied || IsStatusFilterApplied || IsPercentageFilterApplied || IsStartDateFilterApplied || IsEndDateFilterApplied)
            {
                return true;
            }
            return false;
        }
        void UpdateIsFilterApplied()
        {
            IsFilterApplied = GetIsFilterApplied();
            OnPropertyChanged(nameof(IsFilterApplied));
        }
        private void FilterRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreahFilters();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeCloseFilterEvent();
        }
        Visibility GetVisibility(object filterItemString)
        {
            FilterItem filterItem = (FilterItem)Enum.Parse(typeof(FilterItem), filterItemString.ToString());
            if (filterItem == CurrentFilterItem)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        private void OwnerFilterAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {

                if (sender.Text.ToString() != "")
                {
                    _GetUsersVM.GetUsers(UsersType.NameSearch,sender.Text,-1, sender);
                }
                else if (sender.Text.ToString() == "")
                {
                    sender.ItemsSource = null;
                }
            }
        }

        private void OwnerFilterAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ZUser selectedUser = (ZUser)args.SelectedItem;

            if (selectedUser != null)
            {
                OwnerFilterAutoSuggestBox.Text = (string)selectedUser.Name;
            }
            else if (selectedUser == null)
            {
                OwnerFilterAutoSuggestBox.Text = "";
            }
        }

        private void OwnerFilterAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string selectedUserName = args.QueryText.ToString();
            if (selectedUserName != "")
            {
                ZUser user=args.ChosenSuggestion as ZUser;
                if (user != null)
                {
                    SelectedOwner = user;
                    OnPropertyChanged(nameof(SelectedOwner));
                    IsOwnerFilterApplied = true;
                    CollapseOwnerFilterAutoSuggestBox();
                    OwnerFilterAutoSuggestBox.Text = String.Empty;
                    OwnerFilterAutoSuggestBox.ItemsSource = null;
                    OwnerFilterAutoSuggestBox.IsFocusEngaged = false;
                }
            }
        }
        private void SelectedOwnerCancelButton_Click(object sender, RoutedEventArgs e)
        {
            VisibleOwnerFilterAutoSuggestBox();
        }
        void VisibleOwnerFilterAutoSuggestBox()
        {
            OwnerFilterAutoSuggestBox.Visibility = Visibility.Visible;
            SelectedOwnerPanel.Visibility = Visibility.Collapsed;
            SelectedOwner = null;
            IsOwnerFilterApplied = false;
            OnPropertyChanged(nameof(SelectedOwner));
        }
        void CollapseOwnerFilterAutoSuggestBox()
        {
            SelectedOwnerPanel.Visibility = Visibility.Visible;
            OwnerFilterAutoSuggestBox.Visibility = Visibility.Collapsed;
        }
        private void OwnerFilterCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelOwnerFilter();
        }
        void CancelOwnerFilter()
        {
            IsOwnerFilterApplied = false;
            OwnerFilterComboBox.SelectedItem = null;
            OwnerFilterAutoSuggestBox.Text = string.Empty;
            OwnerFilterAutoSuggestBox.Visibility = Visibility.Collapsed;
            SelectedOwnerPanel.Visibility = Visibility.Collapsed;
            OwnerFilterCancelButton.Visibility = Visibility.Collapsed;
        }
        private void PriorityFilterCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelPriorityFilter();
        }
        void CancelPriorityFilter()
        {
            IsPriorityFilterApplied = false;
            PriorityFilterComboBox.SelectedItem = null;
            PriorityFilterCancelButton.Visibility = Visibility.Collapsed;
        }

        private void StatusFilterCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelStatusFilter();
        }
        void CancelStatusFilter()
        {
            IsStatusFilterApplied = false;
            StatusFilterProjectComboBox.SelectedItem = null;
            StatusFilterMilestoneComboBox.SelectedItem = null;
            StatusFilterTaskComboBox.SelectedItem = null;
            StatusFilterCancelButton.Visibility = Visibility.Collapsed;
        }
        private void PercenatgeFilterCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelPercentageFilter();
        }
        void CancelPercentageFilter()
        {
            IsPercentageFilterApplied = false;
            PercentageFilterComboBox.SelectedItem = null;
            PercentageFilterNumberBox.Value = PercentageFilterSecondNumberBox.Value = 0;
            PercentageFilterNumberBox.Visibility = PercentageFilterSecondNumberBox.Visibility = Visibility.Collapsed;
            PercentageFilterCancelButton.Visibility = Visibility.Collapsed;
        }
        private void StartDateFilterCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelStartDateFilter();
        }
        void CancelStartDateFilter()
        {
            IsStartDateFilterApplied = false;
            StartDateFilterComboBox.SelectedItem = null;
            StartDateFilterCalendarDatePicker.Date = StartDateFilterSecondCalendarDatePicker.Date = null;
            StartDateFilterCalendarDatePicker.Visibility = StartDateFilterSecondCalendarDatePicker.Visibility = Visibility.Collapsed;
            StartDateFilterCancelButton.Visibility = Visibility.Collapsed;
        }
        private void EndDateFilterCancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsEndDateFilterApplied = false;
            EndDateFilterComboBox.SelectedItem = null;
            EndDateFilterCalendarDatePicker.Date = EndDateFilterSecondCalendarDatePicker.Date = null;
            EndDateFilterCalendarDatePicker.Visibility = EndDateFilterSecondCalendarDatePicker.Visibility = Visibility.Collapsed;
            EndDateFilterCancelButton.Visibility = Visibility.Collapsed;
        }
        void CancelEndDateFilter()
        {
            IsEndDateFilterApplied = false;
            EndDateFilterComboBox.SelectedItem = null;
            EndDateFilterCalendarDatePicker.Date = EndDateFilterSecondCalendarDatePicker.Date = null;
            EndDateFilterCalendarDatePicker.Visibility = EndDateFilterSecondCalendarDatePicker.Visibility = Visibility.Collapsed;
            EndDateFilterCancelButton.Visibility = Visibility.Collapsed;
        }

        private void FilterMethodRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton.Tag != null)
                SelectedFilterMethod = (FilterMethod)Enum.Parse(typeof(FilterMethod), radioButton.Tag.ToString());
        }

        private void OwnerFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OwnerFilterComboBox.SelectedItem != null)
            {
                IsOwnerFilterApplied = true;
                OwnerFilterCancelButton.Visibility = Visibility.Visible;
                if (SelectedOwnerPanel.Visibility == Visibility.Collapsed)
                {
                    OwnerFilterAutoSuggestBox.Visibility = Visibility.Visible;
                }
            }
        }
        private void PriorityFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PriorityFilterComboBox.SelectedItem != null)
            {
                IsPriorityFilterApplied = true;
                PriorityFilterCancelButton.Visibility = Visibility.Visible;
                ComboBoxItem comboBoxItem = PriorityFilterComboBox.SelectedItem as ComboBoxItem;
                Priority priority = (Priority)Enum.Parse(typeof(Priority), comboBoxItem.Content.ToString());
            }
        }
        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusFilterProjectComboBox.SelectedItem != null)
            {
                IsStatusFilterApplied = true;
                StatusFilterCancelButton.Visibility = Visibility.Visible;
            }
            else if (StatusFilterMilestoneComboBox.SelectedItem != null)
            {
                IsStatusFilterApplied = true;
                StatusFilterCancelButton.Visibility = Visibility.Visible;
            }
            else if (StatusFilterTaskComboBox.SelectedItem != null)
            {
                IsStatusFilterApplied = true;
                StatusFilterCancelButton.Visibility = Visibility.Visible;
            }
        }

        private void PercentageFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PercentageFilterComboBox.SelectedItem != null)
            {
                ComboBoxItem comboBoxItem = (ComboBoxItem)PercentageFilterComboBox.SelectedItem;
                string content = comboBoxItem.Content.ToString();
                IsPercentageFilterApplied = true;
                PercentageFilterCancelButton.Visibility = Visibility.Visible;
                if (content == "Between")
                {
                    PercentageFilterNumberBox.Visibility = Visibility.Visible;
                    PercentageFilterSecondNumberBox.Visibility = Visibility.Visible;

                }
                else
                {
                    PercentageFilterNumberBox.Visibility = Visibility.Visible;
                    PercentageFilterSecondNumberBox.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void StartDateFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDateFilterComboBox.SelectedItem != null)
            {
                ComboBoxItem comboBoxItem = StartDateFilterComboBox.SelectedItem as ComboBoxItem;
                string datePickerCount = comboBoxItem.Tag.ToString();
                IsStartDateFilterApplied = true;
                StartDateFilterCancelButton.Visibility = Visibility.Visible;
                if (datePickerCount == "No")
                {
                    StartDateFilterCalendarDatePicker.Visibility = Visibility.Collapsed;
                    StartDateFilterSecondCalendarDatePicker.Visibility = Visibility.Collapsed;
                }
                else if (datePickerCount == "One")
                {
                    StartDateFilterCalendarDatePicker.Visibility = Visibility.Visible;
                    StartDateFilterSecondCalendarDatePicker.Visibility = Visibility.Collapsed;
                }
                else
                {
                    StartDateFilterCalendarDatePicker.Visibility = Visibility.Visible;
                    StartDateFilterSecondCalendarDatePicker.Visibility = Visibility.Visible;
                }

            }
        }

        private void EndDateFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EndDateFilterComboBox.SelectedItem != null)
            {
                ComboBoxItem comboBoxItem = EndDateFilterComboBox.SelectedItem as ComboBoxItem;
                string datePickerCount = comboBoxItem.Tag.ToString();
                IsEndDateFilterApplied = true;
                EndDateFilterCancelButton.Visibility = Visibility.Visible;
                if (datePickerCount == "No")
                {
                    EndDateFilterCalendarDatePicker.Visibility = Visibility.Collapsed;
                    EndDateFilterSecondCalendarDatePicker.Visibility = Visibility.Collapsed;
                }
                else if (datePickerCount == "One")
                {
                    EndDateFilterCalendarDatePicker.Visibility = Visibility.Visible;
                    EndDateFilterSecondCalendarDatePicker.Visibility = Visibility.Collapsed;
                }
                else
                {
                    EndDateFilterCalendarDatePicker.Visibility = Visibility.Visible;
                    EndDateFilterSecondCalendarDatePicker.Visibility = Visibility.Visible;
                }
            }
        }




        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            Filter();
        }
        void Filter()
        {
            if (!IsOwnerFilterApplied && !IsPriorityFilterApplied && !IsStatusFilterApplied && !IsPercentageFilterApplied && !IsStartDateFilterApplied && !IsEndDateFilterApplied)
            {
                if (CurrentFilterItem == FilterItem.Task)
                    TasksFilteredEvent?.Invoke(TaskList);
                IsFilterApplied = false;
                OnPropertyChanged(nameof(IsFilterApplied));
                _EventNotification.InvokeCancelFilterEvent();

                return;
            }
            else
            {
                foreach (var obj in _FilterPropertyAndValueList)
                {
                    obj.Method = null;
                    obj.Value = null;
                }
            }

          
            if (IsOwnerFilterApplied)
            {
                if (SelectedOwner == null)
                {
                    IsOwnerFilterApplied = false;
                }
                else
                {
                    FilterPropertyAndValue ownerFilterPropertyAndValue = GetFilterPropertyAndValue(FilterProperty.Owner);
                    _AppliedFilters.Add(FilterProperty.Owner);
                    ComboBoxItem comboBoxItem = OwnerFilterComboBox.SelectedItem as ComboBoxItem;
                    OwnerFilterMethod ownerFilterMethod = (OwnerFilterMethod)Enum.Parse(typeof(OwnerFilterMethod), comboBoxItem.Content.ToString().Replace(" ", ""));

                    ownerFilterPropertyAndValue.Method = ownerFilterMethod;
                    ownerFilterPropertyAndValue.Value = SelectedOwner.Id;
                }
            }
            if (IsPriorityFilterApplied)
            {
                _AppliedFilters.Add(FilterProperty.Priority);
                ComboBoxItem comboBoxItem = PriorityFilterComboBox.SelectedItem as ComboBoxItem;
                Priority priority = (Priority)Enum.Parse(typeof(Priority), comboBoxItem.Content.ToString());
                FilterPropertyAndValue priorityFilterPropertyAndValue = GetFilterPropertyAndValue(FilterProperty.Priority);
                priorityFilterPropertyAndValue.Method = priority;
                priorityFilterPropertyAndValue.Value = priority;
            }
            if (IsStatusFilterApplied)
            {
                _AppliedFilters.Add(FilterProperty.Status);
                ComboBoxItem comboBoxItem = StatusFilterTaskComboBox.SelectedItem as ComboBoxItem;
                ZTaskStatus status = (ZTaskStatus)Enum.Parse(typeof(ZTaskStatus), comboBoxItem.Content.ToString().Replace(" ", ""));
                FilterPropertyAndValue statusFilterPropertyAndValue = GetFilterPropertyAndValue(FilterProperty.Status);
                statusFilterPropertyAndValue.Method = status;
                statusFilterPropertyAndValue.Value = status;
            }

            if (IsPercentageFilterApplied)
            {
                _AppliedFilters.Add(FilterProperty.Percentage);
                ComboBoxItem comboBoxItem = PercentageFilterComboBox.SelectedItem as ComboBoxItem;
                PercentageFilterMethod filterMethod = (PercentageFilterMethod)Enum.Parse(typeof(PercentageFilterMethod), comboBoxItem.Content.ToString().Replace(" ", ""));
                int percentage = (int)PercentageFilterNumberBox.Value;
                FilterPropertyAndValue percentageFilterPropertyAndValue = GetFilterPropertyAndValue(FilterProperty.Percentage);
                percentageFilterPropertyAndValue.Method = percentage;
                percentageFilterPropertyAndValue.Value = percentage;
            }

            if (IsStartDateFilterApplied)
            {
                _AppliedFilters.Add(FilterProperty.StartDate);
                ComboBoxItem comboBoxItem = StartDateFilterComboBox.SelectedItem as ComboBoxItem;
                DateFilterMethod filterMethod = (DateFilterMethod)Enum.Parse(typeof(DateFilterMethod), comboBoxItem.Content.ToString().Replace(" ", ""));
                DateTime date=DateTime.MinValue;
                if (StartDateFilterCalendarDatePicker.Date != null)
                {
                    DateTimeOffset dateTimeOffset = (DateTimeOffset)StartDateFilterCalendarDatePicker.Date;
                    date = dateTimeOffset.Date;
                }

                FilterPropertyAndValue startDateFilterPropertyAndValue = GetFilterPropertyAndValue(FilterProperty.StartDate);
                startDateFilterPropertyAndValue.Method = filterMethod;
                startDateFilterPropertyAndValue.Value = date;
            }
            if (IsEndDateFilterApplied)
            {
                _AppliedFilters.Add(FilterProperty.EndDate);
                ComboBoxItem comboBoxItem = EndDateFilterComboBox.SelectedItem as ComboBoxItem;
                DateFilterMethod filterMethod = (DateFilterMethod)Enum.Parse(typeof(DateFilterMethod), comboBoxItem.Content.ToString().Replace(" ", ""));
                DateTime date=DateTime.MinValue;

                if (EndDateFilterCalendarDatePicker != null)
                {
                    DateTimeOffset dateTimeOffset = (DateTimeOffset)EndDateFilterCalendarDatePicker.Date;
                    date = dateTimeOffset.Date;
                }

                FilterPropertyAndValue endDateFilterPropertyAndValue = GetFilterPropertyAndValue(FilterProperty.EndDate);
                endDateFilterPropertyAndValue.Method = filterMethod;
                endDateFilterPropertyAndValue.Value = date;
            }
            IsFilterApplied = true;
            OnPropertyChanged(nameof(IsFilterApplied));
            _EventNotification.InvokeAppliedFiltersEvent(_AppliedFilters);
            _AppliedFilters.Clear();

            _FilterTasksVM.FilterTasks(SelectedFilterMethod, _FilterPropertyAndValueList);
        }

        FilterPropertyAndValue GetFilterPropertyAndValue(FilterProperty property)
        {
            foreach (var filterPropertyAndValue in _FilterPropertyAndValueList)
            {
                if (filterPropertyAndValue.Property == property)
                {
                    return filterPropertyAndValue;
                }
            }
            throw new NotImplementedException();
        }
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        EventNotification _EventNotification=EventNotification.GetInstance();
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeCancelFilterEvent();
        }
        private void FilterControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {

        }

       
    }
    public enum FilterProperty {
        Owner,
        Priority,
        Status,
        Percentage,
        StartDate,
        EndDate
    }

    public enum FilterItem {
        Project,
        Milestone,
        Task
    }

    public enum OwnerFilterMethod
    {
        Is,
       IsNot
    }
    public enum PercentageFilterMethod {
        LessThan,
        GreaterThan,
        Equal,
        Between
    }
  
    public enum DateFilterMethod
    {
        Today,
        TillYesterday,
        Unscheduled, 
        Tomorrow,
        Yesterday,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Between
    }

    public enum FilterMethod
    {
        Any,
        All
    }

    public class FilterPropertyAndValue { 
        public readonly FilterProperty Property;
        public object Method;
        public object Value;
        public FilterPropertyAndValue(FilterProperty property)
        {
            Property = property;
        }
        public FilterPropertyAndValue(FilterProperty property,object method,object value)
        {
            Property=property; 
            Method=method; 
            Value=value;
        }
    }
    

}
