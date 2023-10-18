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
    [Table("ProjectTable")]
    public class Project:IDisposable,INotifyPropertyChanged
    {
        [AutoIncrement]
        [Column("ID"), PrimaryKey]
        public int ID { get; set; }


        string _Name;
        [Column("Name")]
        public string Name
        {
            get {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        
        [Column("Description")]
        public string Description { get; set; } = "";

        [Column("OwnerID")]
        public int OwnerID { get; set; }

        [Column("CreatedUserID")]
        public int CreatedUserID { get; set; }


        ProjectStatus _Status = ProjectStatus.Active;
        [Column(nameof(Status))]
        public ProjectStatus Status { 
            get { 
                return _Status;
            }
            set {
                if (_Status != value)
                {
                    _Status= value;
                    OnPropertyChanged(nameof(Status));
                }
            } 
        }
        [Ignore]
        public int CompletedPercentage {
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


        DateTime _StartDate = DateTime.MinValue;
        [Column("StartDate")]
        public DateTime StartDate { 
            get 
            { 
                return _StartDate;
            } 
            set { 
                if (_StartDate != value)
                {
                    _StartDate= value;
                    OnPropertyChanged(nameof(StartDate));
                }
            } 
        }
        DateTime _EndDate = DateTime.MinValue;

        public event PropertyChangedEventHandler PropertyChanged;

        [Column("EndDate")]
        public DateTime EndDate {
            get
            {
                return _EndDate;
            }
            set {
                if (_EndDate != value)
                {
                    _EndDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            } 
        } 

        public override string ToString()
        {
            return Name;
        }

        public Project()
        {

        }

        public Project(string name,int ownerID,int createdUserID )
        {
            Name= name;
            OwnerID= ownerID;
            CreatedUserID= createdUserID;
        }
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public void Dispose()
        {

        }
        ~Project() { }
    }
}
