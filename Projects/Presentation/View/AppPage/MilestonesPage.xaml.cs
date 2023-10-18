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
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MilestonesPage : Page, INotifyPropertyChanged, IGetUsers, IGetMilestones, IEditMilstone,IDeleteMilestones
    {
        public CoreDispatcher ZCoreDispatcher { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        EventNotification _EventNotification = EventNotification.GetInstance();
        List<MilestoneObj> _MilestoneList = new List<MilestoneObj> { };
        ObservableCollection<MilestoneObj> _MilestoneCollection = new ObservableCollection<MilestoneObj>();
        ObservableCollection<MilestoneObj> _CheckedMilestoneCollection = new ObservableCollection<MilestoneObj> { };
        ObservableCollection<string> _AppliedFilterCollection = new ObservableCollection<string>();
        bool _IsSearchFilterApplied = false;
        GetMilestonesVM _GetMilestonesVM;
        GetUsersVM _GetUsersVM;
        EditMilstoneVM _EditMilstoneVM;
        DeleteMilestoneVM _DeleteMilestoneVM;
        MilestonesType SelectedMilestonesType = MilestonesType.All;
        public MilestonesPage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetMilestonesVM = new GetMilestonesVM(this);
            _GetUsersVM = new GetUsersVM(this);
            _EditMilstoneVM = new EditMilstoneVM(this);
            _DeleteMilestoneVM = new DeleteMilestoneVM(this);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
                SelectedMilestonesType = (MilestonesType)e.Parameter;
            LoadMilestonesType();
        }
        void LoadDataToCollection(IEnumerable<MilestoneObj> projects)
        {
            _MilestoneCollection.Clear();
            foreach (MilestoneObj milestone in projects)
            {
                _MilestoneCollection.Add(milestone);
            }
            if (ClassicViewControl != null)
            {
                ClassicViewControl.LoadMilestoneGroups();
            }
        }
        public void LoadMilestones(MilestonesType milestonesType, IEnumerable<MilestoneObj> projects)
        {
            _MilestoneList.Clear();
            _MilestoneCollection.Clear();
            MilestonesFilterControl._MilestoneList.Clear();
            foreach (MilestoneObj milestone in projects)
            {
                _MilestoneList.Add(milestone);
                _MilestoneCollection.Add(milestone);
                MilestonesFilterControl._MilestoneList.Add(milestone);
            }
            PlainViewControl.IsMilestonesLoaded = true;
        }
        public void LoadFilterdMilestones(IEnumerable<MilestoneObj> projects)
        {
            MilestonesPageSpiltView.IsPaneOpen = false;
            _MilestoneList.Clear();
            _MilestoneCollection.Clear();
            foreach (MilestoneObj milestone in projects)
            {
                _MilestoneList.Add(milestone);
                _MilestoneCollection.Add(milestone);
            }
        }
        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }
      
        void UpdateMilestonesFilteredEvent(IEnumerable<MilestoneObj> milestones)
        {
            LoadFilterdMilestones(milestones);
        }

        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }

        private void MilestonesPagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            _CheckedMilestoneCollection.CollectionChanged += CheckedMilestoneCollectionChanged;
            _EventNotification.NewMilestoneAdded += UpdateNewMilestoneAddedEvent;
            _EventNotification.MilestoneChecked += UpdateMilestoneCheckedEvent;
            _EventNotification.MilestoneUnchecked += UpdateMilestoneUncheckedEvent;
            MilestonesFilterControl.MilestonesFilteredEvent += UpdateMilestonesFilteredEvent;
            _EventNotification.CancelFilter += UpdateCancelFilterEvent;
            _EventNotification.CloseFilter += UpdateCloseFilterEvent;
            _EventNotification.AppliedFilters += UpdateAppliedFiltersEvent;
            _EventNotification.MilestonesDeleted += UpdateMilestonesDeletedEvent;
            _EventNotification.NewMilestoneAdded += UpdateNewMilestoneAddedEvent;
        }
        private void MilestonesPagePanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 900)
            {
                CloseFilterPanel();
            }
        }
        private void HeaderMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            SelectedMilestonesType = (MilestonesType)Enum.Parse(typeof(MilestonesType), menuFlyoutItem.Text.ToString().Replace(" ", ""));
            LoadMilestonesType();
        }
        void LoadMilestonesType()
        {
            PageHeaderTextBlock.Text = GetMilestonesTypeString();
            _GetMilestonesVM.GetMilestones(SelectedMilestonesType, null, -1, null);
        }
        string GetMilestonesTypeString()
        {
            switch (SelectedMilestonesType) {
                case MilestonesType.DueToday:
                    return "Due Today";
                case MilestonesType.DueThisWeek:
                    return "Due This Week";
                case MilestonesType.DueThisMonth:
                    return "Due This Month";
                case MilestonesType.MyActive:
                    return "My Active";
                case MilestonesType.MyCompleted:
                    return "My Completed";
                case MilestonesType.CreatedByMe:
                    return "Created By Me";
                default:
                    return SelectedMilestonesType.ToString();
            }

        }
        private void SearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                SearchMilestones(SearchAutoSuggestBox.Text);
            }
        }
        void SearchMilestones(string searchedText)
        {
            if (String.IsNullOrEmpty(searchedText))
            {
                LoadDataToCollection(_MilestoneList);
                _IsSearchFilterApplied = false;
                return;
            }
            var searchResultMilestones = new List<MilestoneObj>();
            var splitText = searchedText.ToLower().Split(" ");
            foreach (var milestone in _MilestoneList)
            {
                var found = splitText.All((key) =>
                {
                    return milestone.Name.ToLower().Contains(key);
                });
                if (found)
                {
                    searchResultMilestones.Add(milestone);
                }
            }
            LoadDataToCollection(searchResultMilestones);
            _IsSearchFilterApplied = true;
        }
        private void SearchAutoSuggestBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {

        }
      
        private void FilterPropertyCancelMethod_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            _AppliedFilterCollection.Remove(button.DataContext.ToString());
            MilestonesFilterControl.CancelFilterProperty((FilterProperty)Enum.Parse(typeof(FilterProperty), button.DataContext.ToString()));
        }
        private void CloseFilterPanelButton_Click(object sender, RoutedEventArgs e)
        {
            MilestonesPageSpiltView.IsPaneOpen = false;
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
            MilestonesPageSpiltView.IsPaneOpen = false;
        }
        private void CancelFilterPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CancelFilter();
        }
        void CancelFilter()
        {
            CancelFilterPanel.Visibility = Visibility.Collapsed;
            SearchAutoSuggestBox.Text = string.Empty;
            MilestonesPageSpiltView.IsPaneOpen = false;
            _AppliedFilterCollection.Clear();
            if (MilestonesFilterControl.GetIsFilterApplied())
            {
                MilestonesFilterControl.RefreahFilters();
                _GetMilestonesVM.GetMilestones(SelectedMilestonesType, null, -1, null);
            }
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
           if (MilestonesPageSpiltView.IsPaneOpen)
            {
                MilestonesPageSpiltView.IsPaneOpen = false;
            }
            else
            {
                if (_IsSearchFilterApplied)
                {
                    SearchAutoSuggestBox.Text = string.Empty;
                }
                MilestonesPageSpiltView.IsPaneOpen = true;
            }
        }
       
        void CloseFilterPanel()
        {
            if (MilestonesPageSpiltView.IsPaneOpen)
            {

                SearchAutoSuggestBox.Text = string.Empty;
                MilestonesPageSpiltView.IsPaneOpen = false;
                MilestonesFilterControl.RefreahFilters();
                _GetMilestonesVM.GetMilestones(MilestonesType.All, null, -1,null);
            }
        }
        void OpenMilestonesContentPage()
        {
            NewMilestonePanel.Visibility = Visibility.Collapsed;
            MilestonesPageContentPanel.Visibility = Visibility.Visible;
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsAddNewMilestonePageOpen == false)
            {
                if (NewMilestonePanel == null)
                {
                    this.FindName("NewMilestonePanel");
                }
                NewMilestonePanel.Visibility = Visibility.Visible;
                PageAddNewMilestoneContentControl.Refresh();
                MilestonesPageContentPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                _EventNotification.InvokeShowNotificationWithSeverity("Add Milestone page has already been opened",InfoBarSeverity.Informational);
            }
        }
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton clickedButton = (ToggleButton)sender;
            foreach (ToggleButton button in ViewStyleButtonsPanel.Children)
            {
                if (button == clickedButton)
                    button.IsChecked = true;
                else
                    button.IsChecked = false;
            }
            ChangeView(clickedButton);
        }
        bool IsClassicViewControlLoaded = false;
        void ChangeView(ToggleButton clickedButton)
        {
            if (clickedButton.Tag.ToString() == "Plain")
            {
                PlainViewControl.Visibility = Visibility.Visible;
                ClassicViewControl.Visibility = Visibility.Collapsed;
            }
            else
            {
                PlainViewControl.Visibility = Visibility.Collapsed;
                if (IsClassicViewControlLoaded == false)
                {
                    this.FindName("ClassicViewControl");
                    IsClassicViewControlLoaded = true;
                }
                ClassicViewControl.Visibility = Visibility.Visible;

            }
        }
        private void CheckAllMilestonesButton_Click(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeCheckAllMilestonesEvent();
        }
        void CheckedMilestoneCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_CheckedMilestoneCollection.Count < 1)
            {
                MultiSelectPopup.IsOpen = false;
            }
            else
            {
                MultiSelectPopup.IsOpen = true;
            }
        }
        void UpdateNewMilestoneAddedEvent(MilestoneObj milestone)
        {
            _MilestoneList.Add(milestone);
            _MilestoneCollection.Add(milestone);
            MilestonesFilterControl._MilestoneList.Add(milestone);

            OpenMilestonesContentPage();
        }
        void UpdateMilestoneCheckedEvent(MilestoneObj milestone)
        {
            _CheckedMilestoneCollection.Add(milestone);
        }
        void UpdateMilestoneUncheckedEvent(MilestoneObj milestone)
        {
            _CheckedMilestoneCollection.Remove(milestone);
        }
        string GetMilestoneCheckedCount(int count)
        {
            if (count < 2)
            {
                return count + " Milestone Selected";
            }
            return count + " Milestones Selected";
        }
        private void MultiSelectPopupCancelButton(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeUncheckAllMilestonesEvent();
        }


        private void MultiSelectStatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = (MenuFlyoutItem)sender;
            MilestoneStatus status = (MilestoneStatus)Enum.Parse(typeof(MilestoneStatus), menuFlyoutItem.Text.Replace(" ", ""));

            foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
            {
                milestone.Status = status;
                _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.Status, milestone.Status);
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
                    foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
                    {
                        if (milestone.StartDate != DateTime.MinValue)
                        {
                            newDate = milestone.StartDate.AddDays(-(StartDateNumberBox.Value));
                            UpdateStartDate(milestone, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't increment unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Postpone":
                    foreach (Milestone milestone in _CheckedMilestoneCollection)
                    {
                        if (milestone.StartDate != DateTime.MinValue)
                        {
                            newDate = milestone.StartDate.AddDays((StartDateNumberBox.Value));
                            UpdateStartDate(milestone, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't decrement unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Date":
                    foreach (Milestone milestone in _CheckedMilestoneCollection)
                    {
                        if (StartDateCalendarDatePicker.Date != null)
                        {
                            DateTimeOffset dateTimeOffset = (DateTimeOffset)StartDateCalendarDatePicker.Date;
                            newDate = dateTimeOffset.DateTime.Date;
                        }
                        else
                        {
                            newDate = DateTime.MinValue;
                        }
                        UpdateStartDate(milestone, newDate);
                    }
                    break;
            }
        }
        void UpdateStartDate(Milestone milestone, DateTime newDate)
        {

            if (milestone.StartDate != newDate)
            {
                if (newDate <= milestone.EndDate || milestone.EndDate == DateTime.MinValue)
                {
                    milestone.StartDate = newDate;
                    _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.StartDate, milestone.StartDate);
                }
                if (newDate > milestone.EndDate)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("StartDate should be less than EndDate", InfoBarSeverity.Informational);
                }

              
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
                    foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
                    {
                        if (milestone.EndDate != DateTime.MinValue)
                        {
                            newDate = milestone.EndDate.AddDays(-(DueDateNumberBox.Value));
                            UpdateEndDate(milestone, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't increment unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Postpone":
                    foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
                    {
                        if (milestone.EndDate != DateTime.MinValue)
                        {
                            newDate = milestone.EndDate.AddDays((DueDateNumberBox.Value));
                            UpdateEndDate(milestone, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't decrement unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Date":
                    foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
                    {
                        if (DueDateCalendarDatePicker.Date != null)
                        {
                            DateTimeOffset dateTimeOffset = (DateTimeOffset)DueDateCalendarDatePicker.Date;
                            newDate = dateTimeOffset.Date;
                        }
                        else
                        {
                            newDate = DateTime.MinValue;
                        }
                        UpdateEndDate(milestone, newDate);
                    }
                    break;
            }

        }
       void UpdateEndDate(Milestone milestone, DateTime newDate)
        {

            if (milestone.EndDate != newDate)
            {
                if (newDate >= milestone.StartDate || newDate == DateTime.MinValue)
                {
                    milestone.EndDate = newDate;
                    _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.EndDate, milestone.EndDate);
                }
                else
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("End Date should be greater than Start Date", InfoBarSeverity.Informational);
                }
            }
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
                        foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
                        {
                            if (milestone.OwnerID == CurrentOwner.Id)
                            {
                                milestone.OwnerID = ReplaceOwner.Id;
                                milestone.Owner = ReplaceOwner;
                                _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.OwnerID, milestone.OwnerID);
                            }
                        }
                    }
                    break;
                case "Add":
                    if (AddOwner != null)
                    {
                        foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
                        {
                            milestone.OwnerID = AddOwner.Id;
                            milestone.Owner = AddOwner;
                            _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.OwnerID, milestone.OwnerID);
                        }
                    }
                    break;
                case "Unassign":
                    foreach (MilestoneObj milestone in _CheckedMilestoneCollection)
                    {
                        milestone.OwnerID = 0;
                        milestone.Owner = null;
                        _EditMilstoneVM.EditMilestoneProperty(milestone.ID, MilestonePropertyEditType.OwnerID, milestone.OwnerID);
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

                if (sender.Text.ToString() != "")
                {
                    _GetUsersVM.GetUsers(UsersType.NameSearch, sender.Text, -1, sender);
                }

                else if (sender.Text.ToString() == "")
                {
                    sender.ItemsSource = null;
                }

            }
        }

        private void OwnerSearchAutoSuggestionBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            AutoSuggestBox autoSuggestBox = sender as AutoSuggestBox;
            string selectedUserName = args.QueryText.ToString();
            if (selectedUserName != "")
            {
                ZUser user = args.ChosenSuggestion as ZUser;

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
       
        private async void MultiSelectDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteConformationContentDialog contentDialog = new DeleteConformationContentDialog();
            contentDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            int checkedMilestonesCount = _CheckedMilestoneCollection.Count;
            if (checkedMilestonesCount < 2)
            {
                contentDialog.Title = "Are you sure about Delete this Milestone ?";
            }
            else
            {
                contentDialog.Title = "Are you sure about Delete these " + checkedMilestonesCount + " Milestones ?";
            }
            await contentDialog.ShowAsync();
            ContentDialogResult result = contentDialog.Result;
            if (result == ContentDialogResult.Primary)
            {
                _DeleteMilestoneVM.DeleteMilestone(_CheckedMilestoneCollection);
               _EventNotification.InvokeMilestonesDeletedEvent(_CheckedMilestoneCollection);
                MultiSelectPopup.IsOpen = false;
            }
            
        }
        void UpdateMilestonesDeletedEvent(IList<MilestoneObj> milestones)
        {
            foreach (MilestoneObj milestone in milestones)
            {
                _MilestoneCollection.Remove(milestone);
                _MilestoneList.Remove(milestone);
                MilestonesFilterControl._MilestoneList.Remove(milestone);
            }
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.IsAddNewMilestonePageOpen = false;
            OpenMilestonesContentPage();
        }

        private async void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {

            AppWindow addNewMilestoneWindow = await AppWindow.TryCreateAsync();
            addNewMilestoneWindow.Closed += AddNewMilestoneWindow_Closed;
            Frame frame = new Frame();
            frame.Navigate(typeof(AddNewMilestoneInSeparateWindowPage));
            addNewMilestoneWindow.TitleBar.BackgroundColor = Colors.Transparent;
            addNewMilestoneWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            addNewMilestoneWindow.TitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            addNewMilestoneWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Transparent;
            addNewMilestoneWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            addNewMilestoneWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            ElementCompositionPreview.SetAppWindowContent(addNewMilestoneWindow, frame);
            await addNewMilestoneWindow.TryShowAsync();
            addNewMilestoneWindow.Title = "Add Milestone";

            App.IsAddNewMilestonePageOpen=true;
            OpenMilestonesContentPage();
        }

        private void AddNewMilestoneWindow_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
            App.IsAddNewMilestonePageOpen = false;
        }
        public void LoadUser(ZUser user)
        {
            throw new NotImplementedException();
        }

        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }

        public void LoadMilestone(MilestoneObj milestone)
        {
            throw new NotImplementedException();
        }

        public void AutoSuggestionBoxMilestoneSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<MilestoneObj> users)
        {
            throw new NotImplementedException();
        }
        private void MilestonesPagePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _CheckedMilestoneCollection.CollectionChanged -= CheckedMilestoneCollectionChanged;
            _EventNotification.MilestoneChecked -= UpdateMilestoneCheckedEvent;
            _EventNotification.MilestoneUnchecked -= UpdateMilestoneUncheckedEvent;
            _EventNotification.MilestonesDeleted -= UpdateMilestonesDeletedEvent;
            _EventNotification.NewMilestoneAdded -= UpdateNewMilestoneAddedEvent;
        }
       
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

      
    }

    public class SuggestionMilestoneGroup : ObservableCollection<MilestoneObj>
    {
        public SuggestionMilestoneGroup(IEnumerable<MilestoneObj> milestones) : base(milestones)
        {

        }

        public string GroupName { get; set; }

        public override string ToString()
        {
            return GroupName.ToString();
        }

    }
}