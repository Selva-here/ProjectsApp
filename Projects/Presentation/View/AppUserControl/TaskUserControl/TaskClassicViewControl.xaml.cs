using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Data.DataManager.MilestoneDataManager;
using Projects.Data.DBHandler;
using Projects.Notification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class TaskClassicViewControl : UserControl
    {
       
        public ObservableCollection<ZTaskObj> TaskCollection { get; set; }
        EventNotification _EventNotification=EventNotification.GetInstance();
        ObservableCollection<TaskGroup> TaskGroupCollection=new ObservableCollection<TaskGroup>();
        public TaskClassicViewControl()
        {
            this.InitializeComponent();
            
        }
        public void LoadTaskGroups()
        {
            TaskGroupCollection.Clear();
            DataLoadingProgressRing.IsActive = true;
            if (TaskCollection.Count < 1)
            {
                
                NoResultsFoundPanel.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
            }
           
            var groups = from task in TaskCollection
                         group task by task.MilestoneID into g
                         orderby g.Key
                         select new TaskGroup(g) { MilestoneID = g.Key };

            foreach (var group in groups)
            {
                group.Milestone=group.First().Milestone;
                group.Project=group.First().Project;

                TaskGroupCollection.Add(group);
                Debug.WriteLine(group.MilestoneID);
            }
            DataLoadingProgressRing.IsActive = false;
        }
        private void TaskClassicViewControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTaskGroups();
            _EventNotification.TasksDeleted += UpdateTasksDeletedEvent;
            _EventNotification.NewTaskAdded += UpdateNewTaskAddedEvent;
        }
        
        private void ClassicViewTasksList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ZTaskObj task = (ZTaskObj)e.ClickedItem;
            TaskGroup tasks=TaskGroupCollection.Where(g => g.Contains(task)).First();
            _EventNotification.InvokeTaskTappedForDetailViewEvent(task,tasks);
        }
        private void TaskClassicViewControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            TaskGroupCollection.Clear();
            _EventNotification.TasksDeleted -= UpdateTasksDeletedEvent;
            _EventNotification.NewTaskAdded -= UpdateNewTaskAddedEvent;
        }
        void UpdateTasksDeletedEvent(IList<ZTaskObj> tasks)
        {
            foreach (var task in tasks)
            {
                if(TaskCollection.Contains(task))
                TaskCollection.Remove(task);
                foreach(TaskGroup group in TaskGroupCollection)
                {
                    if (group.Contains(task))
                        group.Remove(task);
                }
            }
        }
        void UpdateNewTaskAddedEvent(ZTaskObj task)
        {
            foreach (TaskGroup group in TaskGroupCollection)
            {
                if (group.MilestoneID == task.MilestoneID)
                {
                    group.Add(task);
                    break;
                }
            }
        }
        private void HeaderProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Button button=sender as Button;
            TaskGroup group=button.DataContext as TaskGroup;
            _EventNotification.InvokeProjectTappedForDetailViewEvent(group.Project.ID);
        }

        private void HeaderMilestoneButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TaskGroup group = button.DataContext as TaskGroup;
            _EventNotification.InvokeMilestoneTappedForDetailViewEvent(group.MilestoneID);
           }
    }
    public class TaskGroup : ObservableCollection<ZTaskObj>
    {
        public TaskGroup(IEnumerable<ZTaskObj> tasks) : base(tasks)
        {

        }

        public int MilestoneID { get; set; }
        public Milestone Milestone { get; set; }
        public Project Project { get; set; }
       
        public override string ToString()
        {
            return "Milestone ID " + MilestoneID.ToString();
        }

    }
}
