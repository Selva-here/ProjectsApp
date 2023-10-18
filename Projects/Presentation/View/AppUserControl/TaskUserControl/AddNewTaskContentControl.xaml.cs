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
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class AddNewTaskContentControl : UserControl,IGetProjects,IGetMilestones,IGetTasks,IGetUsers,IAddTask,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public CoreDispatcher ZCoreDispatcher { get; }

        EventNotification _EventNotification = EventNotification.GetInstance();
        ProjectObj SelectedProject = null;
        Milestone SelectedMilestone = null;
        ZUser SelectedOwner = null;
      
        GetProjectsVM _GetProjectsVM;
        GetMilestonesVM _GetMilestonesVM;
        GetTasksVM _GetTasksVM;
        GetUsersVM _GetUsersVM;
        AddTaskVM _AddTaskVM;
        public AddNewTaskContentControl()
        {
            this.InitializeComponent();
            ZCoreDispatcher = Dispatcher;
            _GetProjectsVM = new GetProjectsVM(this);
            _GetMilestonesVM = new GetMilestonesVM(this);
            _GetTasksVM = new GetTasksVM(this);
            _GetUsersVM = new GetUsersVM(this);
            _AddTaskVM = new AddTaskVM(this);
        }
        private void AddNewTaskContentControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            StartDateCalendarDatePicker.Date = DateTime.Today;
            EndDateCalendarDatePicker.Date = DateTime.Today.AddDays(1);
        }
        public void AutoSuggestionBoxProjectSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ProjectObj> projects)
        {
            autoSuggestBox.ItemsSource = projects;
        }
        public void AutoSuggestionBoxMilestoneSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<MilestoneObj> milestones)
        {
           autoSuggestBox.ItemsSource = milestones;
        }
        public void AutoSuggestionBoxUserSuggestion(AutoSuggestBox autoSuggestBox, IEnumerable<ZUser> users)
        {
            autoSuggestBox.ItemsSource = users;
        }
      
        public void TaskAdded(ZTaskObj task)
        {
            _EventNotification.InvokeNewTaskAddedEvent(task);
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

                if (sender.Text.ToString() != "")
                {
                    _GetProjectsVM.GetProjects(ProjectsType.NameSearch,sender.Text,sender);
                }
                //else if (sender.Text.ToString() == "")
                //{
                //    sender.ItemsSource = null;
                //}
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
            string selectedProjectName = args.QueryText.ToString();
            if (selectedProjectName != "")
            {
                ProjectObj project = args.ChosenSuggestion as ProjectObj;
                if (project != null)
                {
                    SelectedProject = project;
                    SelectedProjectNameTextBlock.Visibility = Visibility.Visible;
                    ProjectSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;

                    SelectedMilestone = null;
                    SelectedMilestoneNameTextBlock.Visibility = Visibility.Collapsed;
                    MilestoneSelectionAutoSuggestBox.Visibility = Visibility.Visible;
                    MilestoneSelectionAutoSuggestBox.IsEnabled = true;
                    SelectedOwner = null;
                    SelectedUserControl.Visibility = Visibility.Collapsed;
                    UserSelectionAutoSuggestBox.Visibility = Visibility.Visible;
                    UserSelectionAutoSuggestBox.IsEnabled = true;
                    OnPropertyChanged(nameof(SelectedProject));
                    OnPropertyChanged(nameof(SelectedMilestone));
                    OnPropertyChanged(nameof(SelectedOwner));
                    
                    SelectedProjectNameTextBlock.Visibility = Visibility.Visible;
                    ProjectSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;
                    ProjectSelectionAutoSuggestBox.Text = String.Empty;
                    ProjectSelectionAutoSuggestBox.ItemsSource = null;
                    ProjectSelectionAutoSuggestBox.IsFocusEngaged = false;
                }
            }
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
        private void MilestoneSelectionAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedMilestone != null)
            {
                SelectedMilestoneNameTextBlock.Visibility = Visibility.Visible;
                MilestoneSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;
            }
        }
        private void MilestoneSelectionAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (String.IsNullOrEmpty(sender.Text))
                {
                    sender.ItemsSource = null;
                }
                else
                {
                    if (sender.Text.ToString() != "")
                    {
                        _GetMilestonesVM.GetMilestones(MilestonesType.ProjectNameSearch, sender.Text, SelectedProject.ID, sender);
                    }
                }
            }
        }
           
        private void MilestoneSelectionAutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            MilestoneObj selectedProject = (MilestoneObj)args.SelectedItem;
            if (selectedProject != null)
            {
                sender.Text = (string)selectedProject.Name;
            }
            else if (selectedProject == null)
            {
               sender.Text = "";
            }
        }
        private void MilestoneSelectionAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string selectedMilestoneName = args.QueryText.ToString();
            AutoSuggestBox autoSuggestionBox = sender as AutoSuggestBox;
            MilestoneObj milestone = args.ChosenSuggestion as MilestoneObj;
            if (milestone!=null)
            {
                SelectedMilestone = milestone;
                OnPropertyChanged(nameof(SelectedMilestone));
                SelectedMilestoneNameTextBlock.Visibility = Visibility.Visible;
                autoSuggestionBox.Visibility = Visibility.Collapsed;
                autoSuggestionBox.Text = String.Empty;
                autoSuggestionBox.ItemsSource = null;
                autoSuggestionBox.IsFocusEngaged = false;
            }
        }
        
        private void SelectedMilestoneNameTextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedMilestoneNameTextBlock.Visibility = Visibility.Collapsed;
            MilestoneSelectionAutoSuggestBox.Visibility = Visibility.Visible;
        }
        private void PriorityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
            ZUser user = args.ChosenSuggestion as ZUser;
            if (user !=null)
            {
                SelectedOwner = user;
                OnPropertyChanged(nameof(SelectedOwner));
                SelectedUserControl.Visibility = Visibility.Visible;
                UserSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;
                UserSelectionAutoSuggestBox.Text = String.Empty;
                UserSelectionAutoSuggestBox.ItemsSource = null;
                UserSelectionAutoSuggestBox.IsFocusEngaged = false;
            }
        }
        private void SelectedUserControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedUserControl.Visibility = Visibility.Collapsed;
            UserSelectionAutoSuggestBox.Visibility = Visibility.Visible;
        }
       private void UserSelectionAutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SelectedOwner != null)
            {
                SelectedUserControl.Visibility = Visibility.Visible;
                UserSelectionAutoSuggestBox.Visibility = Visibility.Collapsed;
            }
        }
       
        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {

            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;
                if (dateTimeOffset.DateTime > EndDateCalendarDatePicker.Date)
                {
                    StartDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Start Date should be lesser than Due Date.");
                    return;
                }

            }
            else
            {
                StartDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("Start Date cant be null");
            }
        }

        private void EndDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;

                if (dateTimeOffset.DateTime < StartDateCalendarDatePicker.Date)
                {
                    EndDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Due Date should be greater than Start Date.");
                    return;
                }
               
            }
            else
            {
                EndDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("Due Date cant be null");
            }
        }
        public void Refresh()
        {
            TaskNameTextBox.Text = string.Empty;
            SelectedMilestone = null;
            SelectedProject = null;
            SelectedOwner = null;
            PriorityComboBox.SelectedIndex = 0;
            StartDateCalendarDatePicker.Date=DateTime.Today;
            EndDateCalendarDatePicker.Date = DateTime.Today.AddDays(1);
            AddNewTaskDescriptionControl.Refresh();
        }
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskNameTextBox.Text.Count() > 0 && SelectedProject != null && SelectedMilestone != null && SelectedOwner != null)
            {
                DateTimeOffset starDateTimeOffset = (DateTimeOffset)StartDateCalendarDatePicker.Date;
                DateTime startDate = starDateTimeOffset.Date;
                DateTimeOffset dueDateTimeOffset = (DateTimeOffset)EndDateCalendarDatePicker.Date;
                DateTime endDate = dueDateTimeOffset.Date;

                ComboBoxItem comboBoxItem = PriorityComboBox.SelectedItem as ComboBoxItem;

                Priority priority = (Priority)Enum.Parse(typeof(Priority), comboBoxItem.Content.ToString());
                ZTaskObj task = new ZTaskObj(TaskNameTextBox.Text,SelectedMilestone,SelectedProject,SelectedOwner,App.AppUser,priority,startDate,endDate,AddNewTaskDescriptionControl.DesciptionText);
                _AddTaskVM.AddTask(task);
            }
        }
       
        public void LoadMilestones(MilestonesType milestonesType, IEnumerable<MilestoneObj> milestones)
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
        public void LoadProjects(ProjectsType projectsType, IEnumerable<ProjectObj> projects)
        {
            throw new NotImplementedException();
        }

        public void LoadTasks(TasksType tasksType, IEnumerable<ZTaskObj> tasks)
        {
            throw new NotImplementedException();
        }
        public void ProjectLoaded(ProjectObj project)
        {
            throw new NotImplementedException();
        }

        public void MilestoneLoaded(MilestoneObj milestone)
        {
            throw new NotImplementedException();
        }

        public void LoadProject(ProjectObj project)
        {
            throw new NotImplementedException();
        }

        public void LoadMilestone(MilestoneObj milestone)
        {
            throw new NotImplementedException();
        }

        public void LoadTask(ZTaskObj task)
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

       

        private void AddNewTaskContentControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
