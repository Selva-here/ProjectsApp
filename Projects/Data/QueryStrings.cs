using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Controls.Primitives;

namespace Projects.Data
{
    internal static class QueryStrings
    {
        public const string UserTableCreateQuery = @"CREATE TABLE ZUserTable(
                                                Name  varchar,
                                                ID   integer NOT NULL,
                                                ImagePath   varchar,
                                                Designation   integer,
                                                Password varchar,
                                                MailID  varchar,
                                                PRIMARY KEY(ID AUTOINCREMENT)
                                                )";

        public const string ProjectTableCreateQuery = @"CREATE TABLE ProjectTable(
                                                ID    integer NOT NULL,
                                                Name  varchar,
                                                Description   TEXT,
                                                OwnerID  integer,
                                                CreatedUserID   integer,
                                                Status    integer,
                                                AllTasksCount  integer,
                                                CompletedTasksCount  integer,
                                                StartDate    bigint,
                                                EndDate  bigint,
                                                PRIMARY KEY(ID AUTOINCREMENT)
                                                );";

        public const string MilestoneTableCreateQuery = @"CREATE TABLE MilestoneTable(
                                                    ID    integer NOT NULL,
                                                    Name  varchar,
                                                    Description   TEXT,
                                                    ProjectID    integer,
                                                    OwnerID  integer,
                                                    CreatedUserID   integer,
                                                    Status    integer,
                                                    AllTasksCount  integer,
                                                    CompletedTasksCount  integer,
                                                    StartDate    bigint,
                                                    EndDate  bigint,
                                                    PRIMARY KEY(ID AUTOINCREMENT)
                                                    )";

        public const string TaskTableCreateQuery = @"CREATE TABLE ZTaskTable (
                                                ID    integer NOT NULL,
	                                            Name  varchar,
	                                            MilestoneID  integer,
	                                            ProjectID    integer,
	                                            Description   TEXT,
	                                            Priority  integer,
	                                            Status    integer,
	                                            OwnerID  integer,
	                                            CreatedUserID   integer,
	                                            StartDate    bigint,
	                                            DueDate  bigint,
	                                            CompletedPercentage  integer,
	                                            PRIMARY KEY(ID AUTOINCREMENT)
                                                )";

        public const string ProjectAndUserConnectionTableCreateQuery = @"CREATE TABLE ProjectUserConnectionTable (
	                                                                ProjectID	integer,
	                                                                UserID	integer
                                                                     )";

        public const string TaskAndSubTaskTableCreateQuery = @"CREATE TABLE TaskAndSubTaskConnection (
	                                                                ZTaskID	integer,
	                                                                ZSubTaskID	integer
                                                                     )";

        public const string OpenParenthesisQuery = " ( ";
        public const string CloseParenthesisQuery = " ) ";
        public const string WhereQuery = " WHERE ";
        public const string AndQuery = " AND ";
        public const string OrQuery = " OR ";
        public const string NotQuery = " NOT ";
        public const string InsertTaskQuery = "INSERT INTO ZTaskTable(Name,MilestoneID,ProjectID,OwnerID,CreatedUserID,StartDate,DueDate,Description) VALUES(?,?,?,?,?,?,?,?)";
        public const string InsertMilestoneQuery = "INSERT INTO MilestoneTable(Name,ProjectID,OwnerID,CreatedUserID,StartDate,EndDate,Description) VALUES(?,?,?,?,?,?,?)";
        public const string InsertProjectUserQuery = "INSERT INTO ProjectUserConnectionTable (ProjectID,UserID) VALUES(?,?)";
        public const string InsertProjectQuery = "INSERT INTO ProjectTable(Name,OwnerID,CreatedUserID,StartDate,EndDate,Description) VALUES(?,?,?,?,?,?)";
        public const string InsertUserQuery = "INSERT INTO ZUserTable(Name,MailID,Password,Designation,ImagePath) VALUES(?,?,?,?,?)";
        public const string LastProjectIDQuery = " ID = ( SELECT max(ID) FROM ProjectTable)";
        public const string LastMilestoneIDQuery = " ID = ( SELECT max(ID) FROM MilestoneTable)";
        public const string LastTaskIDQuery = " ID = ( SELECT max(ID) FROM ZTaskTable)";
        public const string LastUserIDQuery = " ID = ( SELECT max(ID) FROM ZUserTable)";
        public const string ProjectUserIDCheckQuery = " ID IN (SELECT UserID FROM  ProjectUserConnectionTable WHERE ProjectID == ? ) ";
       
        public const string UpdateTaskBaseQuery = " UPDATE ZTaskTable SET ";
        public const string UpdateMilestoneBaseQuery = " UPDATE MilestoneTable SET ";
        public const string UpdateProjectBaseQuery = " UPDATE ProjectTable SET ";

        public const string GetTasksBaseQuery = "SELECT * FROM ZTaskTable ";
        public const string GetTasksQuery = GetTasksBaseQuery + WhereQuery + UserProjectIDCheckQuery;
        public const string GetSubTasksBaseQuery = "SELECT * FROM ZSubTaskTable WHERE ParentTaskID = ?";
        public const string GetMilestonesBaseQuery = "SELECT * FROM MilestoneTable ";
        public const string GetProjectsBaseQuery = "SELECT * FROM ProjectTable ";
        public const string GetUsersBaseQuery = "SELECT * FROM ZUserTable ";
        public const string UserProjectCheckQuery = " ID IN (SELECT ProjectID FROM ProjectUserConnectionTable WHERE UserID = ?) ";
        public const string AndUserProjectCheckQuery = "AND ID IN (SELECT ProjectID FROM ProjectUserConnectionTable WHERE UserID = ?) ";
        public const string UserProjectIDCheckQuery = " ProjectID IN (SELECT ProjectID FROM ProjectUserConnectionTable WHERE UserID = ?) ";
        public const string AndUserProjectIDCheckQuery = "AND ProjectID IN (SELECT ProjectID FROM ProjectUserConnectionTable WHERE UserID = ?) ";
        public const string DeleteTaskBaseQuery = "DELETE FROM ZTaskTable ";
        public const string DeleteTaskSubTasksBaseQuery = "DELETE FROM ZSubTaskTable WHERE ParentTaskID = ?";
        public const string DeleteMilestoneBaseQuery = "DELETE FROM MilestoneTable ";
        public const string DeleteProjectBaseQuery = "DELETE FROM ProjectTable ";
        public const string DeleteProjectUserQuery = "DELETE FROM ProjectUserConnectionTable WHERE ProjectID = ? AND UserID = ? ";
        public const string DeleteProjectUsersQuery = "DELETE FROM ProjectUserConnectionTable WHERE ProjectID = ? ";

        public const string IDQuery = " ID = ? ";
        public const string MailIDQuery = " ID = ? ";
        public const string MilestoneIDQuery = " MilestoneID = ? ";
        public const string ProjectIDQuery = " ProjectID = ? ";
        public const string NameQuery = " Name = ? ";
        public const string NameLikeQuery = " Name LIKE '%?%' ";
        public const string StatusQuery = " Status = ? ";
        public const string StatusNotQuery = " Status = ? ";
        public const string PriorityQuery = " Priority = ? ";
        public const string StartDateQuery = " StartDate = ? ";
        public const string PercentageQuery = " CompletedPercentage = ? ";
        public const string DueDateQuery = " DueDate = ? ";
        public const string EndDateQuery = " EndDate = ? ";
        public const string OwnerIDQuery = " OwnerID = ? ";
        public const string CreatedUserIDQuery = " CreatedUserID = ? ";
        public const string DescriptionQuery = " Description = ? ";

        public const string IDCheckQuery = WhereQuery + IDQuery;
        public const string MailIDCheckQuery = WhereQuery + MailIDQuery;
        public const string StatusCheckQuery = WhereQuery + StatusQuery;
        public const string PriorityCheckQuery = WhereQuery + PriorityQuery;
        public const string StartDateCheckQuery = WhereQuery + StartDateQuery;
        public const string EndDateCheckQuery = WhereQuery + EndDateQuery;
        public const string DueDateCheckQuery = WhereQuery + DueDateQuery;
        public const string OwnerIDCheckQuery = WhereQuery + OwnerIDQuery;
        public const string CreatedUserIDCheckQuery = WhereQuery + CreatedUserIDQuery;
        public const string MilestoneIDCheckQuery = WhereQuery+MilestoneIDQuery;
        public const string ProjectIDCheckQuery = WhereQuery+ProjectIDQuery;
        public const string GetLastTaskQuery = GetTasksBaseQuery + WhereQuery + LastTaskIDQuery;
        public const string DeleteTaskQuery = DeleteTaskBaseQuery + IDCheckQuery;
        public const string DeleteMilestoneTaskQuery = DeleteTaskBaseQuery + MilestoneIDCheckQuery;
        public const string DeleteProjectTaskQuery = DeleteTaskBaseQuery + ProjectIDCheckQuery;
        public const string DeleteMilestoneQuery = DeleteMilestoneBaseQuery + IDCheckQuery;
        public const string DeleteProjectMilestonesQuery = DeleteMilestoneBaseQuery + ProjectIDCheckQuery;
        public const string DeleteProjectQuery = DeleteProjectBaseQuery + IDCheckQuery;

        public const string DueDateOverDueCheckQuery =WhereQuery + " DueDate < ? AND DueDate != ?";
        public const string EndDateOverDueCheckQuery =WhereQuery + " EndDate < ? AND EndDate != ?";
        public const string EndDateIntervalCheckQuery = WhereQuery + " EndDate <= ? AND EndDate >= ? ";
    }
}
