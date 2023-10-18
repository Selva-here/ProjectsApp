using SQLite;

namespace Projects.Core.Entity
{
    [Table("TaskAndSubTaskConnection")]
    public class TaskAndSubTaskConnection
    {
        public TaskAndSubTaskConnection()
        {

        }
        public TaskAndSubTaskConnection(int taskID, int subTaskID)
        {
            ZTaskID = taskID;
            ZSubTaskID = subTaskID;
        }
        [Column("ZTaskID")]
        public int ZTaskID { get; set; }
        [Column("ZSubTaskID")]
        public int ZSubTaskID { get; set; }
    }
}
