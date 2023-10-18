using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl
{
    public sealed partial class ListWidgetControl : UserControl
    {
        public string WidgetTitle { get; set; }
      
        public ObservableCollection<ZTaskObj> _Tasks { get; set; }
        ListWidgetControlViewType SelectedListWidgetControlViewType = ListWidgetControlViewType.List;
        EventNotification _EventNotification = EventNotification.GetInstance();
        public ListWidgetControl()
        {
            this.InitializeComponent();
        }
        private void ListWidgetControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (_Tasks.Count() < 1)
            {
                ListViewPanel.Visibility=Visibility.Collapsed;
                NoItemsFoundTextBlock.Visibility = Visibility.Visible;
            }
            _Tasks.CollectionChanged += Tasks_CollectionChanged;
            _EventNotification.NewTaskAdded += UpdateNewTaskAddedEvent;
            _EventNotification.TaskPropertyEdited += UpdateTaskPropertyEditedEvent;
        }

       
        private void ViewTypeMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            ViewOptionDropDownButton.Content = menuFlyoutItem.Text;
            if (menuFlyoutItem.Tag.ToString() == "Calendar")
            {
                if (CalendarViewPanel == null)
                {
                    this.FindName("CalendarViewPanel");
                }
                else
                {
                    this.UnloadObject(CalendarViewPanel);
                    this.FindName("CalendarViewPanel");
                }
                SelectedListWidgetControlViewType = ListWidgetControlViewType.Calendar;
               
            }
            else
            {
                SelectedListWidgetControlViewType = ListWidgetControlViewType.List;
                
            }
            ApplyVisibility();
        }
       
        private void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ApplyVisibility();
        }
        void ApplyVisibility()
        {
            if (SelectedListWidgetControlViewType == ListWidgetControlViewType.Calendar)
            {
                if (_Tasks.Count() < 1)
                {
                    ButtonsPanel.Visibility = Visibility.Collapsed;
                    CalendarViewButtonPanel.Visibility = Visibility.Collapsed;
                    ListViewPanel.Visibility = Visibility.Collapsed;
                    if(CalendarViewPanel!=null)
                    CalendarViewPanel.Visibility = Visibility.Collapsed;
                   
                }
                else
                {
                    ButtonsPanel.Visibility = Visibility.Visible;
                    CalendarViewButtonPanel.Visibility = Visibility.Visible;
                    if (CalendarViewPanel != null)
                        CalendarViewPanel.Visibility = Visibility.Visible;

                    ListViewPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (_Tasks.Count() < 1)
                {
                    ButtonsPanel.Visibility = Visibility.Collapsed;
                    CalendarViewButtonPanel.Visibility = Visibility.Collapsed;
                    if (CalendarViewPanel != null)
                        CalendarViewPanel.Visibility = Visibility.Collapsed;
                    ListViewPanel.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ButtonsPanel.Visibility = Visibility.Visible;
                    CalendarViewButtonPanel.Visibility = Visibility.Collapsed;
                    if (CalendarViewPanel != null)
                        CalendarViewPanel.Visibility = Visibility.Collapsed;
                    ListViewPanel.Visibility = Visibility.Visible;
                }
            }
        }
        void UpdateNewTaskAddedEvent(ZTaskObj task)
        {
            if (WidgetTitle == "Overdue Tasks")
            {
                if (task.EndDate < DateTime.Today.Date && task.EndDate.Date != DateTime.MinValue.Date)
                {
                    AddTask(task);
                }
               
            }
            if (WidgetTitle == "Due Today Tasks")
            {
                if (task.EndDate.Date == DateTime.Today.Date)
                {
                    AddTask(task);
                }
               
            }
        }
        void UpdateTaskPropertyEditedEvent(TaskPropertyEditType property, ZTaskObj task)
        {
            if (property == TaskPropertyEditType.EndDate) {
                ZTaskObj tempTask = task;
                bool isTaskFound=false;
                foreach (var item in _Tasks)
                {
                    if(item.ID == task.ID)
                    {
                        tempTask = item;
                        isTaskFound = true;
                        break;
                    }
                }
                
                if (WidgetTitle == "Overdue Tasks")
                {
                    if (task.EndDate < DateTime.Today.Date && task.EndDate.Date != DateTime.MinValue.Date)
                    {
                        AddTask(task);
                    }
                    else if (task.EndDate >= DateTime.Today.Date)
                    {
                        if (isTaskFound == true)
                        {
                            DeleteTask(tempTask);
                        }
                    }
                }
                if (WidgetTitle == "Due Today Tasks")
                {
                    if(task.EndDate.Date == DateTime.Today.Date)
                    {
                        AddTask(task);
                    }
                    else 
                    {
                        if (isTaskFound == true)
                        {
                            DeleteTask(tempTask);
                        }
                    }
                }
            }
        }
       
        void DeleteTask(ZTaskObj task)
        {
            _Tasks.Remove(task);
            if (WidgetCalendarViewControl != null)
                WidgetCalendarViewControl.RemoveTask(task);
        }
        void AddTask(ZTaskObj task)
        {
            _Tasks.Add(task);
            if (WidgetCalendarViewControl != null)
                WidgetCalendarViewControl.AddTask(task);
        }
        private void TasksListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ZTaskObj task=e.ClickedItem as ZTaskObj;
            _EventNotification.InvokeTaskTappedForDetailViewEvent(task,_Tasks);
        }
        private void MoveLeftButton_Click(object sender, RoutedEventArgs e)
        {
            WidgetCalendarViewControl.MoveLeft();
        }

        private void MoveRightButton_Click(object sender, RoutedEventArgs e)
        {
            WidgetCalendarViewControl.MoveRight();
        }
        private void GoTodayButton_Click(object sender, RoutedEventArgs e)
        {
            WidgetCalendarViewControl.MoveToday();
        }
        private void ListWidgetControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.TaskPropertyEdited -= UpdateTaskPropertyEditedEvent;
        }


    }
    public enum ListWidgetControlViewType {
        Calendar, List
    }

}
