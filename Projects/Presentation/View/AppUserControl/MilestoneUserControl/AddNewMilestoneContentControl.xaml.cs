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
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.MilestoneUserControl
{
    public sealed partial class AddNewMilestoneContentControl : UserControl, IGetUsers,IGetProjects,IAddMilestone,INotifyPropertyChanged
    {
       
        public CoreDispatcher ZCoreDispatcher { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        EventNotification _EventNotification = EventNotification.GetInstance();
       
        GetUsersVM _GetUsersVM;
        GetProjectsVM GetProjectsVM;
        AddMilestoneVM _AddMilestoneVM;
        public ProjectObj SelectedProject = null;
        ZUser SelectedOwner = App.AppUser;
        public AddNewMilestoneContentControl()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            GetProjectsVM=new GetProjectsVM(this);
            _GetUsersVM =new GetUsersVM(this);
            _AddMilestoneVM = new AddMilestoneVM(this);
            StartDateCalendarDatePicker.Date = DateTime.Today;
            EndDateCalendarDatePicker.Date = DateTime.Today.AddDays(1);
        }
        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }
        public void MilestoneAdded(MilestoneObj milestone)
        {
            _EventNotification.InvokeNewMilestoneAddedEvent(milestone);
        }
      
        public void ShowNotification(string message)
        {
            Debug.WriteLine(message);
            _EventNotification.InvokeShowNotification(message);
        }
       
        private void ProjectSelectionAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {

            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<ProjectObj>(3);

                if (sender.Text.ToString() != "")
                {
                    GetProjectsVM.GetProjects(ProjectsType.NameSearch,sender.Text, sender);
                }
                else 
                {
                    sender.ItemsSource = null;
                }
            }
        }
        private void ProjectSelectionAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ProjectObj selectedProject = (ProjectObj)args.SelectedItem;

            if (selectedProject != null)
            {
               
                ProjectSelectionAutoSuggestBox.Text = (string)selectedProject.Name;
            }
            else if (selectedProject == null)
            {
                ProjectSelectionAutoSuggestBox.Text = "";
            }
        }
        private void ProjectSelectionAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
           
            ProjectObj project = args.ChosenSuggestion as ProjectObj;
            if (project !=null)
            {
                
                SelectedProject = project;
                OnPropertyChanged(nameof(SelectedProject));
                UserSelectionAutoSuggestBox.IsEnabled = true;
                SelectedProjectNameTextBlock.Visibility = Visibility.Visible;
                ProjectSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;

                ProjectSelectionAutoSuggestBox.Text = String.Empty;
                ProjectSelectionAutoSuggestBox.ItemsSource = null;
                ProjectSelectionAutoSuggestBox.IsFocusEngaged = false;
            }
        }
        public void AutoSuggestionBoxProjectSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ProjectObj> projects)
        {
            autoSuggestBox.ItemsSource = projects;
        }
        private void ProjectSelectionAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedProject != null)
            {
                SelectedProjectNameTextBlock.Visibility = Visibility.Visible;
                ProjectSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;
            }
        }
        private void SelectedProjectNameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedProjectNameTextBlock.Visibility = Visibility.Collapsed;
            ProjectSelectionAutoSuggestBox.Visibility = Visibility.Visible;
        }
        private void UserSelectionAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.ToString() != "")
                {
                    _GetUsersVM.GetUsers(UsersType.ProjectNameSearch,sender.Text,SelectedProject.ID,sender);
                }
                else if (sender.Text.ToString() == "")
                {
                    sender.ItemsSource = null;
                }
            }
        }

        private void UserSelectionAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            ZUser selectedUser = (ZUser)args.SelectedItem;

            if (selectedUser != null)
            {
                UserSelectionAutoSuggestBox.Text = (string)selectedUser.Name;
            }
            else if (selectedUser == null)
            {
                UserSelectionAutoSuggestBox.Text = "";
            }
        }

        private void UserSelectionAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string selectedUserName = args.QueryText.ToString();
            if (selectedUserName != "")
            {
                ZUser user = args.ChosenSuggestion as ZUser;
                SelectedOwner = user;
                OnPropertyChanged(nameof(SelectedOwner));
                SelectedOwnerPanel.Visibility = Visibility.Visible;
                UserSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;
                UserSelectionAutoSuggestBox.Text = String.Empty;
                UserSelectionAutoSuggestBox.ItemsSource = null;
                UserSelectionAutoSuggestBox.IsFocusEngaged = false;
            }
        }
        private void SelectedOwnerNameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedOwnerPanel.Visibility = Visibility.Collapsed;
            UserSelectionAutoSuggestBox.Visibility = Visibility.Visible;
        }
        private void UserSelectionAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedOwner != null)
            {
                SelectedOwnerPanel.Visibility = Visibility.Visible;
                UserSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;
            }
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }
        public void LoadProjects(ProjectsType projectsType, IEnumerable<ProjectObj> projects)
        {
            throw new NotImplementedException();
        }

        public void LoadUser(ZUser user)
        {
            throw new NotImplementedException();
        }

        public void LoadProject(ProjectObj project)
        {
            throw new NotImplementedException();
        }

        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
        }
        private void EndDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
        }
        public void Refresh()
        {
            MilestoneNameTextBox.Text = string.Empty;
            SelectedProject = null;
            SelectedOwner = null;
            StartDateCalendarDatePicker.Date = null;
            EndDateCalendarDatePicker.Date=null;
            AddNewMilestoneDescriptionControl.Refresh();
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MilestoneNameTextBox.Text.Count() > 0 && SelectedProject != null && SelectedOwner != null)
            {
                if (EndDateCalendarDatePicker.Date != null && StartDateCalendarDatePicker.Date != null && StartDateCalendarDatePicker.Date > EndDateCalendarDatePicker.Date)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("Start can't be greater than End Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                DateTime startDate = DateTime.MinValue;
                DateTime endDate = DateTime.MinValue;
                if (StartDateCalendarDatePicker.Date != null)
                {
                    DateTimeOffset starDateTimeOffset = (DateTimeOffset)StartDateCalendarDatePicker.Date;
                    startDate = starDateTimeOffset.Date;
                }
                if (EndDateCalendarDatePicker.Date != null)
                {
                    DateTimeOffset dueDateTimeOffset = (DateTimeOffset)EndDateCalendarDatePicker.Date;
                    endDate = dueDateTimeOffset.Date;
                }

                MilestoneObj milestone = new MilestoneObj(MilestoneNameTextBox.Text, SelectedProject, SelectedOwner, App.AppUser, startDate, endDate, AddNewMilestoneDescriptionControl.DesciptionText);
                _AddMilestoneVM.AddMilestone(milestone);
            }
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
