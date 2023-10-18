using Projects.Core.AppEnum;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZohoProjects;

namespace Projects.Core.Entity
{
    public class ZTaskBase: INotifyPropertyChanged
    {
       
        [AutoIncrement]
        [Column("ID"), PrimaryKey]
        public int ID { get; set; }

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
        Priority _Priority;
        [Column("Priority"), DefaultValue(0)]
        public Priority Priority
        {
            get
            {
                return _Priority;
            }
            set
            {
                if (_Priority != value)
                {
                    _Priority = value;
                    OnPropertyChanged("Priority");
                }
            }
        }

        private ZTaskStatus _Status = 0;
        [Column("Status"), DefaultValue(0)]
        public ZTaskStatus Status
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

        DateTime _StartDate;
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

        DateTime _EndDate;
        [Column("DueDate")]
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
                    OnPropertyChanged("DueDate");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
