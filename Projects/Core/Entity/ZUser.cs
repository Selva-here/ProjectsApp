using Projects.Core.AppEnum;
using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Projects.Core.Entity
{
   [Table("ZUserTable")]
    public class ZUser : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly object threadLock = new object();
        public ZUser()
        {
        }
        public ZUser(string name)
        {
            lock (threadLock)
            {
                Name = name;
                MailID = GetMailAddress(name);
                Password = GetPassword(name);
            }
        }
        public ZUser(string name, string mailAddress, string password)
        {
            Name = name;
            MailID = mailAddress;
            Password = password;
        }
        string _name;
        [Column("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        int _id;
        [AutoIncrement]
        [Column("ID"), Unique, PrimaryKey]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;

            }
        }

        string _imagePath = "";
        [Column("ImagePath")]
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                if (value != "")
                {
                    _imagePath = value;
                }
            }
        } 

        DesignationsEnum _designationEnum = DesignationsEnum.MemberTechnicalStaff;
        [Column("Designation")]
        public DesignationsEnum DesignationEnum
        {
            get
            {
                return _designationEnum;
            }
            set
            {
                _designationEnum = value;
                Designation = SetDesignation(value);
                OnPropertyChanged("Designation");
            }
        }
        [Ignore]
        public string Designation { get; private set; }

        public string SetDesignation(DesignationsEnum designationEnum)
        {
            switch (designationEnum)
            {
                case DesignationsEnum.ProjectTrainee:
                    return "Project Trainee";
                case DesignationsEnum.MemberTechnicalStaff:
                    return "Member Technical Staff";
                case DesignationsEnum.LeadEngineering:
                    return "Lead Engineering";
                default:
                    return "Project Trainee";
            }
        }

        [Column("Password")]
        public string Password { get; set; }
        string GetPassword(string name)
        {
            string password = name.Replace(" ", ".").ToLower();
            return password;
        }
        [Column("MailID")]
        public string MailID { get; set; }
        string GetMailAddress(string name)
        {
            string mailAddress = name.Replace(" ", ".");
            mailAddress = mailAddress.ToLower() + "@zohocorp.com";
            return mailAddress;
        }
        public override string ToString()
        {
            return Name;
        }

        void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
