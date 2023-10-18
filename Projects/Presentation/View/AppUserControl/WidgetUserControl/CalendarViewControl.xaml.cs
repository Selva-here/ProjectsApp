using Projects.Core.EntityObj;
using Projects.Notification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

namespace Projects.Presentation.View.AppUserControl.WidgetUserControl
{
    public sealed partial class CalendarViewControl : UserControl
    {
        public IEnumerable<ZTaskObj> Tasks { get; set; }
        ObservableCollection<CalendarViewWeekControl> _WeekCalendarViewUserControlCollection = new ObservableCollection<CalendarViewWeekControl>();
        DateTime _ThisWeekFirstDate;
        bool IsFlipViewReady = false;
        public CalendarViewControl()
        {
            this.InitializeComponent();
            
        }
        private void CalendarViewControPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoadWeekCalendarView();
        }
        public void LoadWeekCalendarView()
        {
            _WeekCalendarViewUserControlCollection.Clear();
            _ThisWeekFirstDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            DateTime firstDate = _ThisWeekFirstDate.AddDays(-14);
            for (int i = 0; i < 5; i++)
            {
                CalendarViewWeekControl weekCalendarViewUserControl = CreateWeekCalenderViewUserControl(firstDate.AddDays(i * 7));
                _WeekCalendarViewUserControlCollection.Add(weekCalendarViewUserControl);
            }
            CalendarViewFlipView.SelectedIndex = 2;
            IsFlipViewReady = true;
        }
        CalendarViewWeekControl CreateWeekCalenderViewUserControl(DateTime firstDate)
        {
            CalendarViewWeekControl weekCalenderViewUserControl = new CalendarViewWeekControl();
            weekCalenderViewUserControl.FirstDate = firstDate;
            weekCalenderViewUserControl.LoadWorkItems(Tasks);
            return weekCalenderViewUserControl;
        }

        private void CalenderViewFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalendarViewWeekControl weekCalendarViewUserControl = (CalendarViewWeekControl)CalendarViewFlipView.SelectedItem;
            //EventNotification.Instance.InvokeWeekCalendarViewUserControlAlignmentChangedEvent(weekCalenderViewUserControl.ColumnAlignment);

            if (IsFlipViewReady)
            {
                try
                {
                    var addedItems = e.AddedItems;
                    int index = _WeekCalendarViewUserControlCollection.IndexOf((CalendarViewWeekControl)addedItems[0]);

                    if ((_WeekCalendarViewUserControlCollection.Count - 1) - index < 2)
                    {
                        AddItemsAtLast(_WeekCalendarViewUserControlCollection.Last().FirstDate, 4);
                    }
                    else if (index < 2)
                    {
                        AddItemsAtFirst(_WeekCalendarViewUserControlCollection.First().FirstDate, 4);
                    }
                }
                catch(Exception ex) { 
                    Debug.WriteLine(ex);
                }
            }
        }

        void AddItemsAtLast(DateTime firstDateOfWeek, int itemCountToAdd)
        {
            for (int i = 1; i <= itemCountToAdd; i++)
            {
                CalendarViewWeekControl calendarViewWeekControl = CreateWeekCalenderViewUserControl(firstDateOfWeek.AddDays(i * 7));
                _WeekCalendarViewUserControlCollection.Add(calendarViewWeekControl);
            }
        }
        void AddItemsAtFirst(DateTime firstDateOfWeek, int itemCountToAdd)
        {
            for (int i = 1; i <= itemCountToAdd; i++)
            {
                CalendarViewWeekControl calendarViewWeekControl = CreateWeekCalenderViewUserControl(firstDateOfWeek.AddDays(-(i * 7)));
                _WeekCalendarViewUserControlCollection.Insert(0, calendarViewWeekControl);
            }
        }
        public void MoveLeft()
        {
            int index = CalendarViewFlipView.SelectedIndex - 1;
            while (index >= 0)
            {
                if (_WeekCalendarViewUserControlCollection[index].TaskList.Count > 0)
                {
                    CalendarViewFlipView.SelectedIndex = index;
                    break;
                }
                index--;
            }
        }
        public void MoveToday()
        {
            int index = _WeekCalendarViewUserControlCollection.IndexOf(_WeekCalendarViewUserControlCollection.Where(collection => collection.FirstDate == _ThisWeekFirstDate).FirstOrDefault());
            CalendarViewFlipView.SelectedIndex = index;
        }
        public void MoveRight()
        {
            int index = CalendarViewFlipView.SelectedIndex + 1;
            while (index < _WeekCalendarViewUserControlCollection.Count)
            {
                if (_WeekCalendarViewUserControlCollection[index].TaskList.Count > 0)
                {
                    CalendarViewFlipView.SelectedIndex = index;
                    break;
                }
                index++;
            }
        }
        public void AddTask(ZTaskObj task)
        {
            foreach(var collection in _WeekCalendarViewUserControlCollection)
            {
                collection.AddTask(task);
            }
            
        }
        public void RemoveTask(ZTaskObj task)
        {
            foreach (var weekView in _WeekCalendarViewUserControlCollection)
            {
                weekView.RemoveTask(task);
            }

        }
    }
}
