using SQLite;

namespace Projects.Core.Entity
{
    [Table("ProjectUserConnectionTable")]
    public class ProjectAndUserConnection
    {
        public ProjectAndUserConnection()
        {

        }
        public ProjectAndUserConnection(int projectID, int ownerID)
        {

            ProjectID = projectID;
            OwnerId = ownerID;
        }
        [Column("ProjectID")]
        public int ProjectID { get; set; }
        [Column("UserID")]
        public int OwnerId { get; set; }
    }
}
