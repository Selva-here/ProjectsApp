using Projects.Core.AppEnum;
using Projects.Core.EntityObj;
using Projects.Core.Helper;
using Projects.Notification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using ZohoProjects;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.TaskUserControl
{
    public sealed partial class TaskKanbanViewControl : UserControl
    {
        public ObservableCollection<ZTaskObj> TaskCollection { get; set; }
        public ObservableCollection<KanbanTaskCollection> StatusKanbanTaskCollection = new ObservableCollection<KanbanTaskCollection>();
        public ObservableCollection<KanbanTaskCollection> PriorityKanbanTaskCollection = new ObservableCollection<KanbanTaskCollection>();
        public ObservableCollection<KanbanTaskCollection> PercentageKanbanTaskCollection = new ObservableCollection<KanbanTaskCollection>();
        public event PropertyChangedEventHandler PropertyChanged;

        EventNotification _EventNotification=EventNotification.GetInstance();

        public TaskKanbanViewControl()
        {
            this.InitializeComponent();
        }
        private void TaskKanbanViewControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.TasksDeleted += UpdateTasksDeletedEvent;
            _EventNotification.TaskPropertyEdited += UpdateTaskPropertyEditedEvent;
            _EventNotification.NewTaskAdded += UpdateNewTaskAddedEvent;
        }
        public void LoadKanbanTaskCollections()
        {
            if (TaskCollection.Count < 1)
            {
                KanbanPivotPanel.Visibility = Visibility.Collapsed;
                NoResultsFoundPanel.Visibility = Visibility.Visible;
                DataLoadingProgressRing.IsActive = false;
                return;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
                KanbanPivotPanel.Visibility = Visibility.Visible;
            }

            if (StatusPanel != null)
                LoadStatusKanbanViewData();
            if (PriorityPanel != null)
                LoadPriorityKanbanViewData();
            if (PercentagePanel != null)
                LoadPercentageKanbanViewData();
        }
        private void KanbanPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PivotItem pivotItem = (PivotItem)e.AddedItems[0];
            LoadView((KanbanViewType)Enum.Parse(typeof(KanbanViewType), pivotItem.Tag.ToString()));
        }
        private void BoardWidthSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _EventNotification.InvokeKanbanViewBoardidthChangedEvent((int)BoardWidthSlider.Value);
        }
        void LoadView(KanbanViewType kanbanViewType)
        {
            if (TaskCollection.Count < 1)
            {
                KanbanPivotPanel.Visibility = Visibility.Collapsed;
                NoResultsFoundPanel.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
                KanbanPivotPanel.Visibility = Visibility.Visible;
            }
            if (kanbanViewType == KanbanViewType.Status)
            {
                this.FindName("StatusPanel");
            }
            else if (kanbanViewType == KanbanViewType.Priority)
            {
                this.FindName("PriorityPanel");
            }
            else if (kanbanViewType == KanbanViewType.Percentage)
            {
                this.FindName("PercentagePanel");
            }

        }
       
        private void StatusPanel_Loaded(object sender, RoutedEventArgs e)
        {

            IEnumerable<ZTaskStatus> StatusList = (IEnumerable<ZTaskStatus>)Enum.GetValues(typeof(ZTaskStatus));
            StatusKanbanTaskCollection.Clear();
            DataLoadingProgressRing.IsActive = true;
            foreach (ZTaskStatus status in StatusList)
            {
                StatusKanbanTaskCollection.Add(new KanbanTaskCollection(KanbanViewType.Status, status, ZTaskHelper.ConvertTaskStatusToString(status)));
            }
            LoadStatusKanbanViewData();
        }
        void LoadStatusKanbanViewData()
        {

            var groups = from task in TaskCollection
                         group task by task.Status into g
                         orderby g.Key
                         select g;
            foreach(var collection in StatusKanbanTaskCollection)
            {
                collection.Collection.Clear();
            }
            foreach (var group in groups)
            {
                StatusKanbanTaskCollection.Where(collection => (ZTaskStatus)collection.Key == group.Key).First().LoadCollection(group);
            }
            DataLoadingProgressRing.IsActive = false;
        }
        private void PriorityPanel_Loaded(object sender, RoutedEventArgs e)
        {

            IEnumerable<Priority> priorityList = (IEnumerable<Priority>)Enum.GetValues(typeof(Priority));
            PriorityKanbanTaskCollection.Clear();
            DataLoadingProgressRing.IsActive = true;
            foreach (Priority priority in priorityList)
            {
                PriorityKanbanTaskCollection.Add(new KanbanTaskCollection(KanbanViewType.Priority, priority, priority.ToString()));
            }
            LoadPriorityKanbanViewData();
        }
        void LoadPriorityKanbanViewData()
        {
            var groups = from task in TaskCollection
                         group task by task.Priority into g
                         orderby g.Key
                         select g;
            foreach (var collection in PriorityKanbanTaskCollection)
            {
                collection.Collection.Clear();
            }
            foreach (var group in groups)
            {
                PriorityKanbanTaskCollection.Where(collection => (Priority)collection.Key == group.Key).First().LoadCollection(group);
            }
            DataLoadingProgressRing.IsActive = false;
        }
        private void PercentagePanel_Loaded(object sender, RoutedEventArgs e)
        {
            DataLoadingProgressRing.IsActive =true;
            PercentageKanbanTaskCollection.Clear();
            for (int i = 0; i <= 100; i = i + 10)
            {
                PercentageKanbanTaskCollection.Add(new KanbanTaskCollection(KanbanViewType.Percentage,i, i.ToString()));
            }

            LoadPercentageKanbanViewData();
            DataLoadingProgressRing.IsActive = false;
        }


        void LoadPercentageKanbanViewData()
        {
            var groups = from task in TaskCollection
                         group task by task.CompletedPercentage into g
                         orderby g.Key
                         select g;
            foreach (var collection in PercentageKanbanTaskCollection)
            {
                collection.Collection.Clear();
            }
            foreach (var group in groups)
            {
                PercentageKanbanTaskCollection.Where(collection => (int)collection.Key == group.Key).First().LoadCollection(group);
            }
            DataLoadingProgressRing.IsActive = false;
        }
        void UpdateTasksDeletedEvent(IList<ZTaskObj> tasks)
        {
            foreach (var task in tasks)
            {
                foreach(KanbanTaskCollection collection in StatusKanbanTaskCollection)
                {
                    if(collection.Collection.Contains(task))
                        collection.Collection.Remove(task);
                }
                foreach (KanbanTaskCollection collection in PriorityKanbanTaskCollection)
                {
                    if (collection.Collection.Contains(task))
                        collection.Collection.Remove(task);
                }
                foreach (KanbanTaskCollection collection in PercentageKanbanTaskCollection)
                {
                    if (collection.Collection.Contains(task))
                        collection.Collection.Remove(task);
                }
            }
        }
        void UpdateTaskPropertyEditedEvent(TaskPropertyEditType taskProperty,ZTaskObj task)
        {
            switch (taskProperty)
            {
                case TaskPropertyEditType.Status:
                    foreach (KanbanTaskCollection collection in StatusKanbanTaskCollection)
                    {
                        if (collection.Collection.Contains(task))
                        {
                            collection.Collection.Remove(task);
                        }
                        if ((ZTaskStatus)collection.Key == task.Status)
                        {
                            collection.Collection.Add(task);
                        }
                    }
                    break;
                case TaskPropertyEditType.Priority:
                    foreach (KanbanTaskCollection collection in PriorityKanbanTaskCollection)
                    {
                        if (collection.Collection.Contains(task))
                        {
                            collection.Collection.Remove(task);
                        }
                        if ((Priority)collection.Key == task.Priority)
                        {
                            collection.Collection.Add(task);
                        }
                    }
                    break;
                case TaskPropertyEditType.Percentage:
                    foreach (KanbanTaskCollection collection in PercentageKanbanTaskCollection)
                    {
                        if (collection.Collection.Contains(task))
                        {
                            collection.Collection.Remove(task);
                        }
                        if ((int)collection.Key == task.CompletedPercentage)
                        {
                            collection.Collection.Add(task);
                        }
                    }
                    break;
            }
        }
        void UpdateNewTaskAddedEvent(ZTaskObj task)
        {
            foreach (KanbanTaskCollection collection in StatusKanbanTaskCollection)
            {
                if ((ZTaskStatus)collection.Key == task.Status)
                {
                    collection.Collection.Add(task);
                    break;
                }
            }
            foreach (KanbanTaskCollection collection in PriorityKanbanTaskCollection)
            {
                if ((Priority)collection.Key == task.Priority)
                {
                    collection.Collection.Add(task);
                    break;
                }
            }
            foreach (KanbanTaskCollection collection in PercentageKanbanTaskCollection)
            {
                if ((int)collection.Key == task.CompletedPercentage)
                {
                    collection.Collection.Add(task);
                    break;
                }
            }
        }
        private void StatusPanel_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void PriorityPanel_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void PercentagePanel_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void TaskKanbanViewControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.TasksDeleted -= UpdateTasksDeletedEvent;
            _EventNotification.NewTaskAdded -= UpdateNewTaskAddedEvent;
            _EventNotification.TaskPropertyEdited -= UpdateTaskPropertyEditedEvent;
        }

    }
    public class KanbanTaskCollection:ObservableCollection<ZTaskObj>
    {

        public KanbanViewType KanbanType { get; set; }
        public object Key { get; set; }
        public string Header { get; set; }

        public ObservableCollection<ZTaskObj> Collection = new ObservableCollection<ZTaskObj>();

        public KanbanTaskCollection(KanbanViewType kanbanType,object key, string header)
        {
            Key = key;
            Header = header;
            KanbanType = kanbanType;
        }
        public void LoadCollection(IEnumerable<ZTaskObj> collection)
        {
            Collection.Clear();
            foreach (ZTaskObj task in collection)
            {
                Collection.Add(task);
            }
        }
    }

   

}
