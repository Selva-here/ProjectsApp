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
    public sealed partial class ProjectsPage : Page,IGetProjects,IGetUsers,IEditProject, IDeleteProjects
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CoreDispatcher ZCoreDispatcher { get; }

        EventNotification _EventNotification=EventNotification.GetInstance();
        List<ProjectObj> _ProjectList= new List<ProjectObj>();
        ObservableCollection<ProjectObj> _ProjectCollection =new ObservableCollection<ProjectObj>();
        ObservableCollection<ProjectObj> _CheckedProjectCollection = new ObservableCollection<ProjectObj> { };
        ProjectsType _SelectedProjectsType = ProjectsType.All;
        ObservableCollection<string> _AppliedFilterCollection=new ObservableCollection<string>();
        GetProjectsVM _GetProjectsVM;
        GetUsersVM _GetUsersVM;
        EditProjectVM _EditProjectVM;
        DeleteProjectsVM _DeleteProjectsVM;
        bool _IsSearchFilterApplied = false;
        public ProjectsPage()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetProjectsVM = new GetProjectsVM(this);
            _GetUsersVM= new GetUsersVM(this);
            _EditProjectVM = new EditProjectVM(this);
            _DeleteProjectsVM=new DeleteProjectsVM(this);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
                _SelectedProjectsType = (ProjectsType)e.Parameter;
            LoadProjectsType();
        }
        void LoadDataToCollection(IEnumerable<ProjectObj> projects)
        {
            _ProjectCollection.Clear();
            foreach (ProjectObj project in projects)
            {
                _ProjectCollection.Add(project);
            }
        }
        public void LoadProjects(ProjectsType projectsType, IEnumerable<ProjectObj> projects)
        {
            _ProjectList.Clear();
            _ProjectCollection.Clear();
            ProjectsFilterControl._ProjectList.Clear();
            DataLoadingProgressRing.IsActive = false;
            foreach (ProjectObj project in projects)
            {
                _ProjectList.Add(project);
                _ProjectCollection.Add(project);
                ProjectsFilterControl._ProjectList.Add(project);
            }
        }
        public void LoadFilteredProjects(IEnumerable<ProjectObj> projects)
        {
            ProjectsPageSpiltView.IsPaneOpen = false;
            _ProjectList.Clear();
            _ProjectCollection.Clear();
            foreach (ProjectObj project in projects)
            {
                _ProjectList.Add(project);
                _ProjectCollection.Add(project);
            }
        }
        void UpdateNewProjectAdded(ProjectObj project)
        {
            _ProjectList.Add(project);
            _ProjectCollection.Add(project);
            ProjectsFilterControl._ProjectList.Add(project);
        }
       
        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }
        void UpdateProjectsFilteredEvent(IEnumerable<ProjectObj> projects)
        {
            LoadFilteredProjects(projects);
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        public static string GetPercentageString(int percentage)
        {
            return percentage.ToString() + " %";
        }

        private void ProjectsPagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            _CheckedProjectCollection.CollectionChanged += CheckedProjectCollectionChanged;
            _ProjectCollection.CollectionChanged += ProjectCollectionChanged;
            _EventNotification.NewProjectAdded += UpdateNewProjectAdded;
            _EventNotification.CheckedProjectsDeleted += UpdateCheckedProjectsDeletedEvent;
            _EventNotification.ProjectChecked += UpdateProjectCheckedEvent;
            _EventNotification.ProjectUnchecked += UpdateProjectUncheckedEvent;
            _EventNotification.CancelFilter += UpdateCancelFilterEvent;
            _EventNotification.CloseFilter += UpdateCloseFilterEvent;
            _EventNotification.AppliedFilters += UpdateAppliedFiltersEvent;
            ProjectsFilterControl.ProjectsFilteredEvent += UpdateProjectsFilteredEvent;

            _EventNotification.NewProjectAdded += UpdateNewProjectAddedEvent;

        }
        private void HeaderMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = (MenuFlyoutItem)sender;
            _SelectedProjectsType = (ProjectsType)Enum.Parse(typeof(ProjectsType), menuFlyoutItem.Tag.ToString());
            LoadProjectsType();
        }
        void LoadProjectsType()
        {
            PageHeaderTextBlock.Text=_SelectedProjectsType.ToString();
            _GetProjectsVM.GetProjects(_SelectedProjectsType, null,null);
        }
        private void SearchAutoSuggestBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            SearchProjects(SearchAutoSuggestBox.Text); 
        }

        private void SearchAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            SearchProjects(SearchAutoSuggestBox.Text);
        }
        void SearchProjects(string searchedText)
        {
            if (String.IsNullOrEmpty(searchedText))
            {
                LoadDataToCollection(_ProjectList);
                _IsSearchFilterApplied = false;
                return;
            }
            var searchResultProjects = new List<ProjectObj>();
            var splitText = searchedText.ToLower().Split(" ");
            foreach (var project in _ProjectList)
            {
                var found = splitText.All((key) =>
                {
                    return project.Name.ToLower().Contains(key);
                });
                if (found)
                {
                    searchResultProjects.Add(project);
                }
            }
            LoadDataToCollection(searchResultProjects);
            _IsSearchFilterApplied = true;
        }
        public void AutoSuggestionBoxProjectSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ProjectObj> projects)
        {
            throw new NotImplementedException();
        }

        private void FilterPropertyCancelMethod_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            _AppliedFilterCollection.Remove(button.DataContext.ToString());
            ProjectsFilterControl.CancelFilterProperty((FilterProperty)Enum.Parse(typeof(FilterProperty), button.DataContext.ToString()));
        }
        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsAddNewProjectPageOpen == false)
            {
                if (NewProjectPanel == null)
                {
                    this.FindName("NewProjectPanel");
                }
                NewProjectPanel.Visibility = Visibility.Visible;
                PageAddNewProjectContentControl.Refresh();
                ProjectsPageContentPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                _EventNotification.InvokeShowNotificationWithSeverity("Add Project page has already been opened", InfoBarSeverity.Informational);
            }
        }

        private void FilterProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectsPageSpiltView.IsPaneOpen) {
                ProjectsPageSpiltView.IsPaneOpen = false;
            }
            else
            {
                if (_IsSearchFilterApplied)
                {
                    SearchAutoSuggestBox.Text = string.Empty;
                }
                ProjectsPageSpiltView.IsPaneOpen = true;
            }
        }
        private void CloseFilterPanelButton_Click(object sender, RoutedEventArgs e)
        {
            ProjectsPageSpiltView.IsPaneOpen = false;
        }
        void UpdateAppliedFiltersEvent(List<FilterProperty> appliedFilter)
        {
            _AppliedFilterCollection.Clear();
            foreach(var filter in appliedFilter)
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
            ProjectsPageSpiltView.IsPaneOpen = false;
        }
        private void CancelFilterPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CancelFilter();
        }
        void CancelFilter()
        {
            CancelFilterPanel.Visibility = Visibility.Collapsed;
            SearchAutoSuggestBox.Text = string.Empty;
            ProjectsPageSpiltView.IsPaneOpen = false;
            _AppliedFilterCollection.Clear();
            if (ProjectsFilterControl.GetIsFilterApplied())
            {
                ProjectsFilterControl.RefreahFilters();
                _GetProjectsVM.GetProjects(_SelectedProjectsType, null, null);
            }
        }
        private void ProjectsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProjectObj project = (ProjectObj)e.ClickedItem;
            Frame.Navigate((typeof(ProjectDetailPage)), project);
        }
       
        void ProjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_ProjectCollection.Count < 1)
            {
                NoResultsFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultsFoundPanel.Visibility= Visibility.Collapsed;
            }
        }
        void CheckedProjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_CheckedProjectCollection.Count < 1)
            {
                MultiSelectPopup.IsOpen = false;
            }
            else
            {
                MultiSelectPopup.IsOpen = true;
            }
        }
        private void CheckAllProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeCheckAllProjectsEvent();
        }
        string GetProjectCheckedCount(int count)
        {
            if (count < 2)
            {
                return count + " Project Selected";
            }
            return count + " Projects Selected";
        }
        void UpdateProjectCheckedEvent(ProjectObj project)
        {
            _CheckedProjectCollection.Add(project);
        }
        void UpdateProjectUncheckedEvent(ProjectObj project)
        {
            _CheckedProjectCollection.Remove(project);
        }
      
        private void MultiSelectPopupCancelButton(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeUncheckAllProjectsEvent();
        }

    
        private void MultiSelectStatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = (MenuFlyoutItem)sender;
            ProjectStatus status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), menuFlyoutItem.Text.Replace(" ", ""));

            foreach (ProjectObj project in _CheckedProjectCollection)
            {
                if (project.Status != status)
                {
                    project.Status = status;
                    _EditProjectVM.EditProjectProperty(project.ID, ProjectPropertyEditType.Status, project.Status);
                }
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
                    foreach (ProjectObj project in _CheckedProjectCollection)
                    {
                        if (project.StartDate != DateTime.MinValue)
                        {
                            newDate = project.StartDate.AddDays(-(StartDateNumberBox.Value));
                            UpdateStartDate(project, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't increment unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Postpone":
                    foreach (Project project in _CheckedProjectCollection)
                    {
                        if (project.StartDate != DateTime.MinValue)
                        {
                            newDate = project.StartDate.AddDays((StartDateNumberBox.Value));
                            UpdateStartDate(project, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't decrement unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Date":
                    foreach (Project project in _CheckedProjectCollection)
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
                        UpdateStartDate(project, newDate);
                    }
                    break;
            }
            MultiSelectStartDateFlyout.Hide();
        }
        void UpdateStartDate(Project project, DateTime newDate)
        {

            if (project.StartDate != newDate )
            {
                if(newDate > project.EndDate)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("StartDate should be less than EndDate",InfoBarSeverity.Informational);
                }
               
                if (newDate <= project.EndDate || project.EndDate==DateTime.MinValue)
                {
                    project.StartDate = newDate;
                    _EditProjectVM.EditProjectProperty(project.ID, ProjectPropertyEditType.StartDate, project.StartDate);
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
                    foreach (ProjectObj project in _CheckedProjectCollection)
                    {
                        if (project.EndDate != DateTime.MinValue)
                        {
                            newDate = project.EndDate.AddDays(-(DueDateNumberBox.Value));
                            UpdatEndDate(project, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't increment unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Postpone":
                    foreach (ProjectObj project in _CheckedProjectCollection)
                    {
                        if (project.EndDate != DateTime.MinValue)
                        {
                            newDate = project.EndDate.AddDays((DueDateNumberBox.Value));
                            UpdatEndDate(project, newDate);
                        }
                        else
                        {
                            _EventNotification.InvokeShowNotificationWithSeverity("Can't decrement unassigned value", InfoBarSeverity.Informational);
                        }
                    }
                    break;
                case "Date":
                    foreach (ProjectObj project in _CheckedProjectCollection)
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
                        UpdatEndDate(project,newDate);
                    }
                    break;
            }
            MultiSelectEndDateFlyout.Hide();
        }
        void UpdatEndDate(Project project, DateTime newDate)
        {

            if (project.EndDate != newDate )
            {
                if (newDate >= project.StartDate || newDate == DateTime.MinValue)
                {
                    project.EndDate = newDate;
                    _EditProjectVM.EditProjectProperty(project.ID, ProjectPropertyEditType.EndDate, project.EndDate);
                }
                else
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("End Date should be greater than Start Date", InfoBarSeverity.Informational);
                }
            }
        }
       
        private void MultiSelectOwnerRadioButtonPanel_Loaded(object sender, RoutedEventArgs e)
        {
            Panel panel = sender as Panel;
            RadioButton radioButton = panel.Children[0] as RadioButton;
            radioButton.IsChecked = true;
        }
        
        private void OwnerSearchAutoSuggestionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<ZUser>(3);

                if (sender.Text.ToString() != "")
                {

                    _GetUsersVM.GetUsers(UsersType.NameSearch, sender.Text.ToString(),-1, sender as AutoSuggestBox);
                }

                else if (sender.Text.ToString() == "")
                {
                    sender.ItemsSource = null;
                }
                sender.ItemsSource = suitableItems;
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
        
        private async void MultiSelectDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteConformationContentDialog contentDialog = new DeleteConformationContentDialog();
            contentDialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            int checkedProjectsCount = _CheckedProjectCollection.Count;
            if (checkedProjectsCount < 2)
            {
                contentDialog.Title = "Are you sure about Delete this Project ?";
            }
            else
            {
                contentDialog.Title = "Are you sure about Delete these " + checkedProjectsCount + " Projects ?";
            }
            await contentDialog.ShowAsync();
            ContentDialogResult result = contentDialog.Result;
            if (result == ContentDialogResult.Primary)
            {
               _DeleteProjectsVM.DeleteProject(_CheckedProjectCollection);
                foreach (ProjectObj project in _CheckedProjectCollection)
                {
                    _ProjectCollection.Remove(project);
                    _ProjectList.Remove(project);
                    ProjectsFilterControl._ProjectList.Remove(project);
                }
                _CheckedProjectCollection.Clear();
                _EventNotification.InvokeCheckedProjectsDeletedEvent(_CheckedProjectCollection);
                MultiSelectPopup.IsOpen = false;
            }
           
        }
        void UpdateCheckedProjectsDeletedEvent(IList<ProjectObj> projects)
        {

        }
        void UpdateNewProjectAddedEvent(ProjectObj project)
        {
            OpenProjectsContentPage();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OpenProjectsContentPage();
        }

        private async void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {

            AppWindow addNewProjectWindow = await AppWindow.TryCreateAsync();
            addNewProjectWindow.Closed += AddNewProjectWindow_Closed;
            Frame frame = new Frame();
            frame.Navigate(typeof(AddNewProjectInSeparateWindowpage), addNewProjectWindow);
            addNewProjectWindow.TitleBar.BackgroundColor = Colors.Transparent;
            addNewProjectWindow.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            addNewProjectWindow.TitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            addNewProjectWindow.TitleBar.ButtonPressedBackgroundColor = Colors.Transparent;
            addNewProjectWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            addNewProjectWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            ElementCompositionPreview.SetAppWindowContent(addNewProjectWindow, frame);
            await addNewProjectWindow.TryShowAsync();
            addNewProjectWindow.Title = "Add Project";
            App.IsAddNewProjectPageOpen = true;
            OpenProjectsContentPage();
        }

        private void AddNewProjectWindow_Closed(AppWindow sender, AppWindowClosedEventArgs args)
        {
            App.IsAddNewProjectPageOpen = false;
        }

        void OpenProjectsContentPage()
        {
            NewProjectPanel.Visibility = Visibility.Collapsed;
            
            ProjectsPageContentPanel.Visibility = Visibility.Visible;
        }
        public void LoadProject(ProjectObj project)
        {
            throw new NotImplementedException();
        }

        public void LoadUser(ZUser user)
        {
            throw new NotImplementedException();
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }
        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void ProjectsPagePanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.NewProjectAdded -= UpdateNewProjectAdded;
            _EventNotification.CheckedProjectsDeleted -= UpdateCheckedProjectsDeletedEvent;
            _CheckedProjectCollection.CollectionChanged -= CheckedProjectCollectionChanged;
            _EventNotification.ProjectChecked -= UpdateProjectCheckedEvent;
            _EventNotification.ProjectUnchecked -= UpdateProjectUncheckedEvent;
            ProjectsFilterControl.ProjectsFilteredEvent -= UpdateProjectsFilteredEvent;

            _EventNotification.NewProjectAdded -= UpdateNewProjectAddedEvent;
        }

       
    }
  
}
