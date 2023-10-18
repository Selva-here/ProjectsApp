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
using Windows.UI.Xaml.Media;

namespace Projects.Core.EntityObj
{
    public class MilestoneObj : Milestone,IDisposable
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
        public Project Project{get; set;}
        public ZUser CreatedUser { get; set; }
        public ZUser Owner { get; set; }
        public MilestoneObj() : base() { }
        public override string ToString()
        {
            return Name;
        }
        public MilestoneObj(string name, Project project, ZUser owner, ZUser assignedUser,DateTime startDate, DateTime endDate,string description) : base()
        {
            Name = name;
            Project = project;
            ProjectID = project.ID;
            Owner = owner;
            OwnerID = owner.Id;
            CreatedUser = assignedUser;
            CreatedUserID = assignedUser.Id;
            StartDate = startDate;
            EndDate = endDate;
            Description = description;
        }
    }
}
