using Projects.Core.AppEnum;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZohoProjects;

namespace Projects.Core.Entity
{
    [Table("MilestoneTable")]
    public class Milestone:IDisposable,INotifyPropertyChanged, IWorkItem
    {

        int _ID;
        [AutoIncrement]
        [Column("ID"), PrimaryKey]
        public int ID
        {
            get { return _ID; }
            set
            {
                if (_ID != value)
                {
                    _ID = value;
                }
            }
        }

        string _Name;
        [Column("Name")]
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        string _Description="";
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

        [Column("ProjectID")]
        public int ProjectID { get; set; }

        [Column("OwnerID")]
        public int OwnerID { get; set; }

        [Column("CreatedUserID")]
        public int CreatedUserID { get; set; }

        private MilestoneStatus _Status = MilestoneStatus.Active;
        [Column("Status")]
        public MilestoneStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged("Status");
                }
            }
        }
        [Ignore]
        public int CompletedPercentage
        {
            get
            {
                try
                {
                    return (_CompletedTasksCount / _AllTasksCount) * 100;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {

            }
        }
        int _CompletedTasksCount = 0;
        [Column("CompletedTasksCount")]
        public int CompletedTasksCount
        {
            get
            {
                return _CompletedTasksCount;
            }
            set
            {
                if (value != _CompletedTasksCount)
                {
                    _CompletedTasksCount = value;
                    OnPropertyChanged(nameof(CompletedTasksCount));
                    OnPropertyChanged(nameof(CompletedPercentage));
                }
            }
        }
        int _AllTasksCount = 0;
        [Column("AllTasksCount")]
        public int AllTasksCount
        {
            get
            {
                return _AllTasksCount;
            }
            set
            {
                if (value != _AllTasksCount)
                {
                    _AllTasksCount = value;
                    OnPropertyChanged(nameof(AllTasksCount));
                    OnPropertyChanged(nameof(CompletedPercentage));
                }
            }
        }



        DateTime _StartDate= DateTime.MinValue;
        [Column("StartDate")]
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                if (_StartDate != value)
                {
                    _StartDate = value;
                    OnPropertyChanged("StartDate");
                }
            }
        }
        DateTime _EndDate= DateTime.MinValue;
        [Column("EndDate")]
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                if (_EndDate != value)
                {
                    _EndDate = value;
                    OnPropertyChanged("EndDate");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public override string ToString()
        {
            return Name;
        }

        public Milestone()
        {

        }

        public Milestone(string name, int projectId,int ownerID,int createdUserId)
        {
            Name = name;
            ProjectID = projectId;
            OwnerID = ownerID;
            CreatedUserID = createdUserId;
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                catch { }
            }
        }
        public void Dispose()
        {
            
        }
        ~Milestone() { }
    }
}
