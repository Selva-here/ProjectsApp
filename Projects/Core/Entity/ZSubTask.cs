using Projects.Core.AppEnum;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.Core.Entity
{
    [Table("ZSubTaskTable")]
    public class ZSubTask:ZTaskBase
    {
        int _ParentTaskID;
        [Column("ParentTaskID")]
        public int ParentTaskID
        {
            get { return _ParentTaskID; }
            set
            {
                if (_ParentTaskID != value)
                {
                    _ParentTaskID = value;
                }
            }
        }
        public ZSubTask()
        {

        }
        public ZSubTask(int parentTaskID,string name,Priority priority,DateTime startDate,DateTime dueDate)
        {
            ParentTaskID = parentTaskID;
            Name = name;
            Priority = priority;
            StartDate = startDate;
            EndDate = dueDate;
        }
    }
}
