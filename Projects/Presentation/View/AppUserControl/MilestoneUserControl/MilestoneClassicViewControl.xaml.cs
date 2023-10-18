using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.Notification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl.MilestoneUserControl
{
    public sealed partial class MilestoneClassicViewControl : UserControl
    {
      

        public event Action<MilestoneObj> MilestoneItemClickedEvent;
        public ObservableCollection<MilestoneObj> MilestoneCollection { get; set; }
        EventNotification _EventNotification = EventNotification.GetInstance();
        ObservableCollection<MilestoneGroup> MilestoneGroupCollection = new ObservableCollection<MilestoneGroup>();
        public MilestoneClassicViewControl()
        {
            this.InitializeComponent();
        }
        private void MilestoneClassicViewControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMilestoneGroups();
            _EventNotification.MilestonesDeleted += UpdateMilestonesDeletedEvent;
            _EventNotification.NewMilestoneAdded += UpdateNewMilestoneAddedEvent;
        }
        public void LoadMilestoneGroups()
        {
            var groups = from item in MilestoneCollection
                         group item by item.Project.ID into g
                         orderby g.Key
                         select new MilestoneGroup(g) { Key = g.Key };
            MilestoneGroupCollection.Clear();
            foreach (var group in groups)
            {
                group.Project = group.First().Project;
                MilestoneGroupCollection.Add(group);
            }
            DataLoadingProgressRing.IsActive = false;
            if (MilestoneCollection.Count < 1)
            {
                NoResultsFoundPanel.Visibility = Visibility.Visible;
            }
            else
            {
                NoResultsFoundPanel.Visibility = Visibility.Collapsed;
            }
        }
        void UpdateMilestonesDeletedEvent(IList<MilestoneObj> milestones)
        {
            foreach (MilestoneObj milestone in milestones)
            {
                foreach (MilestoneGroup group in MilestoneGroupCollection)
                {
                    if ((int)group.Key == milestone.ProjectID)
                    {
                        group.Remove(milestone);
                    }
                }
            }
        }
        void UpdateNewMilestoneAddedEvent(MilestoneObj milestone)
        {
            foreach (MilestoneGroup group in MilestoneGroupCollection)
            {
                if ((int)group.Key == milestone.ProjectID)
                {
                    group.Add(milestone);
                }
            }
        }

        private void ClassicViewListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            MilestoneObj milestone = (MilestoneObj)e.ClickedItem;
            MilestoneItemClickedEvent?.Invoke(milestone);
        }
        private void HeaderProjectButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            _EventNotification.InvokeProjectTappedForDetailViewEvent((int)button.Tag);
        }
       
        private void MilestonePlainViewItemControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Control control = sender as Control;
            Milestone milestone = (Milestone)control.DataContext;
            _EventNotification.InvokeMilestoneTappedForDetailViewEvent(milestone.ID);
        }
        private void MilestoneClassicViewControlPanel_Unloaded(object sender, RoutedEventArgs e)
        {
            _EventNotification.NewMilestoneAdded -= UpdateNewMilestoneAddedEvent;
            _EventNotification.MilestonesDeleted -= UpdateMilestonesDeletedEvent;
        }
    }
    public class MilestoneGroup : ObservableCollection<MilestoneObj>
    {
        public MilestoneGroup(IEnumerable<MilestoneObj> milestones) : base(milestones)
        {

        }

        public object Key { get; set; }
        public Project Project { get; set; }

        public override string ToString()
        {
            return Project.Name;
        }

    }
    public enum MilestoneViewType
    {
        Plain,
        Classic,
        Kanban
    }
}
