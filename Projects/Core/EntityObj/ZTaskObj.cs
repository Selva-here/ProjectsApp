using Projects.Core.AppEnum;
using Projects.Core.Entity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Core.EntityObj
{
    public class ZTaskObj:ZTask
    {

        private bool _IsChecked = false;
        [Ignore]
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                if (_IsChecked != value)
                {
                    _IsChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }
        public ObservableCollection<ZSubTask> SubTaskCollection= new ObservableCollection<ZSubTask>();
        public Project Project { get; set; }
        public Milestone Milestone { get; set; }
        public ZUser CreatedUser { get; set; }

        ZUser _owner;
        public ZUser Owner
        {
            get{ return _owner; }
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    OnPropertyChanged(nameof(Owner));
                }
            }
        }
        public ZTaskObj() : base()
        {

        }
        public ZTaskObj(string name, Milestone milestone, Project project, ZUser owner, ZUser assignedUser,Priority priority, DateTime startDate, DateTime dueTime,string description) : base()
        {
            Name = name;
            Milestone = milestone;
            MilestoneID = milestone.ID;
            Project= project;
            ProjectID = project.ID;
            Owner= owner;
            OwnerID = owner.Id;
            CreatedUser = assignedUser;
            CreatedUserID = assignedUser.Id;
            Priority=priority;
            StartDate = startDate;
            EndDate = dueTime;
            Description = description;
        }
       public override string ToString()
        {
            return Name;
        }

        ~ZTaskObj()
        { 
        }

    }
}
