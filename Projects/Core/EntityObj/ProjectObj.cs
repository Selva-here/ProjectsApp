using Projects.Core.Entity;
using System;
using System.Collections.ObjectModel;

namespace Projects.Core.EntityObj
{
    public class ProjectObj:Project,IDisposable
    {
        public ProjectObj():base() { }
        public ProjectObj(string name, ZUser owner, ZUser createdUser) 
        {
            Name = name;
            Owner = owner;
            OwnerID = owner.Id;
            CreatedUser = createdUser;  
            CreatedUserID = createdUser.Id;
        }
        public ZUser CreatedUser;
        ZUser owner;
        public ZUser Owner {
            get
            {
                
                return owner;
            }
            set
            {
                if (owner != value)
                {
                    owner = value;
                    OnPropertyChanged(nameof(Owner));
                }
            }
        }
        public ObservableCollection<Milestone> MilestoneCollection { get; set; } = new ObservableCollection<Milestone>();
        public ObservableCollection<ZUser> UserCollection { get; set; } = new ObservableCollection<ZUser>();
        public override string ToString()
        {
            return Name;
        }
      
    }
}
