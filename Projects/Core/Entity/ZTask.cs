using System;
using SQLite;
using System.ComponentModel;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Projects.Core.AppEnum;
using ZohoProjects;
using System.Collections.ObjectModel;

using Windows.System;

namespace Projects.Core.Entity
{
    [Table("ZTaskTable")]
    public class ZTask : ZTaskBase
    {


        [Column("MilestoneID")]
        public int MilestoneID
        {
            get;
            set;
        }

        [Column("ProjectID")]
        public int ProjectID
        {
            get;
            set;
        }

        //public ObservableCollection<ZUser> OwnerCollection { get; set; }
        string _Description = "";
        [Column("Description")]
        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        [Column("OwnerID")]
        public int? OwnerID { get; set; }


        [Column("CreatedUserID")]
        public int CreatedUserID { get; set; } = 1;






        private int _CompletedPercentage = 0;
        [Column("CompletedPercentage"), DefaultValue(0)]
        public int CompletedPercentage
        {
            get { return _CompletedPercentage; }
            set
            {
                if (_CompletedPercentage != value)
                {
                    _CompletedPercentage = value;
                    OnPropertyChanged("CompletedPercentage");
                }
            }
        }



        public override string ToString()
        {
            return Name;
        }
        public ZTask()
        {

        }


        public ZTask(string name, int milestoneID, int projectID, int ownerID, int assignedUserID, DateTime startDate, DateTime dueTime)
        {
            Name = name;
            MilestoneID = milestoneID;
            ProjectID = projectID;
            OwnerID = ownerID;
            CreatedUserID = assignedUserID;
            StartDate = startDate;
            EndDate = dueTime;
        }

    }

}
