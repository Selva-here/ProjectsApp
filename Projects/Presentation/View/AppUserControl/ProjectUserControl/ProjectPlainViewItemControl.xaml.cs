using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.ProjectUserControl
{
    public sealed partial class ProjectPlainViewItemControl : UserControl,IEditProject
    {
        EventNotification _EventNotification=EventNotification.GetInstance();
        ProjectObj _Project { get
            {
                return this.DataContext as ProjectObj;
            } 
        }

        public CoreDispatcher ZCoreDispatcher { get; }
       EditProjectVM _EditProjectVM;
        public ProjectPlainViewItemControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            ZCoreDispatcher = Dispatcher;
            _EditProjectVM = new EditProjectVM(this);
        }
        public void ShowNotification(string message)
        {
            _EventNotification.InvokeShowNotification(message);
        }
        private void ProjectPlainViewItemControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.UncheckAllProjects += UpdateUncheckAllProjects;
            _EventNotification.CheckAllProjects += UpdateCheckAllProjectsEvent;
        }

        private void UpdateCheckAllProjectsEvent()
        {
            ItemSelectCheckBox.IsChecked = true;
        }

        private void ItemSelectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeProjectCheckedEvent(_Project);
        }

        private void ItemSelectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _EventNotification.InvokeProjectUncheckedEvent(_Project);
        }
        void UpdateUncheckAllProjects()
        {
            ItemSelectCheckBox.IsChecked = false;
        }
        private void StatusMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            _Project.Status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), menuFlyoutItem.Text.Replace(" ", ""));
            _EditProjectVM.EditProjectProperty(_Project.ID,ProjectPropertyEditType.Status,_Project.Status);
        }

        private void StartDateButton_Click(object sender, RoutedEventArgs e)
        {
            StartDateCalendarDatePicker.IsCalendarOpen = true;
        }
        private void StartDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {

            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;
                if (dateTimeOffset.DateTime.Date > _Project.EndDate && _Project.EndDate != DateTime.MinValue)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("Start Date should be lesser than End Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                else
                {
                    _Project.StartDate = dateTimeOffset.DateTime.Date;
                    _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.StartDate, _Project.StartDate);
                }
            }
            else
            {
                _Project.StartDate = DateTime.MinValue;
                _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.StartDate, _Project.StartDate);
            }
           
        }
       
        private void DueDateButton_Click(object sender, RoutedEventArgs e)
        {
            DueDateCalendarDatePicker.IsCalendarOpen = true;

        }

        private void DueDateCalendarDatePicker_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (args.NewDate != null)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)args.NewDate;
                if (dateTimeOffset.DateTime.Date < _Project.StartDate)
                {
                    _EventNotification.InvokeShowNotificationWithSeverity("End Date should be greater than Start Date", Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational);
                    return;
                }
                else
                {
                    _Project.EndDate = dateTimeOffset.DateTime.Date;
                    _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.EndDate, _Project.EndDate);
                }
            }
            else
            {
                _Project.EndDate = DateTime.MinValue;
                _EditProjectVM.EditProjectProperty(_Project.ID, ProjectPropertyEditType.EndDate, _Project.EndDate);
            }
            
        }

       

        private void ProjectPlainViewItemControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.UncheckAllProjects -= UpdateUncheckAllProjects;
            _EventNotification.CheckAllProjects -= UpdateCheckAllProjectsEvent;
        }
    }
}
