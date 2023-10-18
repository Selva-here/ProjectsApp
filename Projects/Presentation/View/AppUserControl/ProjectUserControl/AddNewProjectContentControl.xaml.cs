using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.ProjectUserControl
{
    public sealed partial class AddNewProjectContentControl : UserControl,IGetUsers,IAddProject
    {
        EventNotification _EventNotification=EventNotification.GetInstance();
        ObservableCollection<ZUser> SelectedUserCollection = new ObservableCollection<ZUser>();
        GetUsersVM _GetUsersVM;
        AddProjectVM _AddProjectVM;
        public CoreDispatcher ZCoreDispatcher { get; }

        public AddNewProjectContentControl()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetUsersVM=new GetUsersVM(this);
            _AddProjectVM =new AddProjectVM(this);
            SelectedUserCollection.Add(App.AppUser);
            StartDateCalendarDatePicker.Date = DateTime.Today.Date;
            EndDateCalendarDatePicker.Date = DateTime.Today.Date.AddDays(1);

        }
        public void ProjectAdded(ProjectObj project)
        {
            _EventNotification.InvokeNewProjectAddedEvent(project);
        }
       
        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }

        public void ShowNotification(string message)
        {
            Debug.WriteLine(message);
            _EventNotification.InvokeShowNotification(message);
        }
       
        private void UserSelectionAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput  )
            {
                if(sender.Text.ToString() != "")
                    _GetUsersVM.GetUsers(UsersType.NameSearch, sender.Text,-1, sender);
                else
                    sender.ItemsSource = null;
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
                if (!SelectedUserCollection.Contains(user))
                {
                    SelectedUserCollection.Add(user);
                }

                UserSelectionAutoSuggestBox.Text = String.Empty;
                UserSelectionAutoSuggestBox.ItemsSource = null;
                UserSelectionAutoSuggestBox.IsFocusEngaged = false;
            }
        }

        private void UserCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ZUser user = (ZUser)button.DataContext;
           
            if (user.Id == App.AppUserID)
            {
                _EventNotification.InvokeShowNotificationWithSeverity("You should be a user in project created by you.",Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
            }
            else if (SelectedUserCollection.Count < 2)
            {
                _EventNotification.InvokeShowNotificationWithSeverity("Atleast one user should be present", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
            }
            else
            {
                SelectedUserCollection.Remove(user);
            }
        }
        public void Refresh()
        {
            ProjectNameTextBox.Text = string.Empty;
            StartDateCalendarDatePicker.Date = null;
            EndDateCalendarDatePicker.Date = null;
            AddNewProjectDescriptionControl.Refresh();
            foreach(ZUser user in SelectedUserCollection.Where(user=>user.Id!=App.AppUserID))
            {
                SelectedUserCollection.Remove(user);
            }
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
           
            if (ProjectNameTextBox.Text.Count() > 0 && SelectedUserCollection.Count > 0)
            {


                if(EndDateCalendarDatePicker.Date != null && StartDateCalendarDatePicker.Date!=null && StartDateCalendarDatePicker.Date> EndDateCalendarDatePicker.Date)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("Start can't be greater than End Date",Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
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

                ProjectObj newProject = new ProjectObj(ProjectNameTextBox.Text, App.AppUser, App.AppUser);
                DateTimeOffset dateTimeOffset = (DateTimeOffset)StartDateCalendarDatePicker.Date;
                newProject.StartDate = dateTimeOffset.Date;
                dateTimeOffset = (DateTimeOffset)EndDateCalendarDatePicker.Date;
                newProject.EndDate = dateTimeOffset.Date;
                newProject.Description = AddNewProjectDescriptionControl.DesciptionText;
                foreach(ZUser user in SelectedUserCollection)
                {
                    newProject.UserCollection.Add(user);
                }
                _AddProjectVM.AddProject(newProject);
                _EventNotification.InvokeNewProjectAddedEvent(newProject);
            }
        }
        public void LoadUsers(IEnumerable<ZUser> users)
        {
            throw new NotImplementedException();
        }
        public void LoadUser(ZUser user)
        {
            throw new NotImplementedException();
        }
    }
}
