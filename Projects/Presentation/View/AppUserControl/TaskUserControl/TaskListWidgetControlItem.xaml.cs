using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.ViewContract;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TaskListWidgetControlItem : Page,IEditTask
    {
        public ZTaskObj _ZTask { get { return this.DataContext as ZTaskObj; } }

        public CoreDispatcher ZCoreDispatcher { get; }

        EventNotification _EventNotification=EventNotification.GetInstance();
        EditTaskVM _EditTaskVM { get; set; }
        public TaskListWidgetControlItem()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) => Bindings.Update();
            ZCoreDispatcher = Dispatcher;
            _EditTaskVM = new EditTaskVM(this);
        }
        public void ShowNotification(string message)
        {
           _EventNotification.InvokeShowNotification(message);
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
                if (dateTimeOffset.DateTime.Date > _ZTask.EndDate.Date)
                {
                    StartDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("Start Date should be lesser than Due Date.");
                    return;
                }
                if (_ZTask.StartDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZTask.StartDate = dateTimeOffset.DateTime;
                    _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.StartDate, _ZTask.StartDate);
                    _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.StartDate, _ZTask);
                }
            }
            else
            {
                StartDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("Start Date cant be null");
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

                if (dateTimeOffset.DateTime.Date < _ZTask.StartDate.Date)
                {
                    DueDateCalendarDatePicker.Date = args.OldDate;
                    _EventNotification.InvokeShowNotification("End Date should be greater than Start Date.");
                    return;
                }
                if (_ZTask.EndDate.Date != dateTimeOffset.DateTime.Date)
                {
                    _ZTask.EndDate = dateTimeOffset.DateTime;
                    _EditTaskVM.EditTaskProperty(_ZTask.ID, TaskPropertyEditType.EndDate, _ZTask.EndDate);
                    _EventNotification.InvokeTaskPropertyEditedEvent(TaskPropertyEditType.EndDate, _ZTask);
                }
            }
            else
            {
                DueDateCalendarDatePicker.Date = args.OldDate;
                _EventNotification.InvokeShowNotification("End Date cant be null");
            }
        }

       
    }
}
