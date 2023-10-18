
using Projects.Core.AppEnum;
using Projects.Core.Entity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using ZohoProjects;

namespace Projects.DatabaseConnector
{
    public class InitialDataLoader
    {
        SQLiteConnection _Connection;
        DateTime DT = new DateTime(2022, 11, 25, 9, 30, 0);
        Random random = new Random();
        DateTime tempDate;
        List<ZUser> users = new List<ZUser>();
        public InitialDataLoader(SQLiteConnection connection)
        {
            _Connection = connection;
        }

        public void LoadInitialData()
        {
            LoadUsers();
            LoadProjects();
            _Connection.Close();
        }

      
        public void LoadUsers()
        {
            
            List<string> names = new List<string>() { "Selva",
                "Napolean", "Ceasar", "Charlemagne",
                "Leonidas","Alexander","George",
                "Arthur", "Bismarck", "Gustavus" };
                
            for (int i = 0; i < 10; i++)
            {
                users.Add(new ZUser(names[i]));
            }
            _Connection.InsertAll(users);
            string _imagePath;
            int j = 1;
            foreach (ZUser user in users)
            {
                _imagePath = ApplicationData.Current.LocalFolder.Path + @"\UserPP" + @"\" + j + ".jpeg";
                user.ImagePath = _imagePath;
                j++;
            }
            _Connection.UpdateAll(users);
        }

        List<string> ProjectNameList = new List<string>() {"" };
        public void LoadProjects()
        {
            Milestone milestone;
            Project project;
            ZTask task;
            //int projectUsers[0].Id;
            List<ZUser> projectUsers = new List<ZUser>();
            DateTime tempDate;
            DateTime startDate = GetAddedDate(DateTime.Today.AddDays(-5));

            project = new Project("Front End Development", 1, 1);
            project.Status = (ProjectStatus)random.Next(0, 10);
            project.StartDate= tempDate = GetAddedDate(startDate);
            
            _Connection.Insert(project);
           projectUsers.Clear();
            for (int userID = random.Next(2, 4); userID < 11; userID++)
            {
                projectUsers.Add(users[userID - 1]);
                _Connection.Insert(new ProjectAndUserConnection(project.ID, userID));
            }
            projectUsers.Add(users[0]);
            _Connection.Insert(new ProjectAndUserConnection(project.ID, 1));
            //projectUsers[0].Id = projectUsers[0].Id;
            milestone = new Milestone("Sign In Page Completion", project.ID, projectUsers[0].Id, projectUsers[1].Id);
            milestone.Status = (MilestoneStatus)random.Next(0, 8);
            _Connection.Insert(milestone);
            task = new ZTask("Complete Sign In Page Name Text Box", milestone.ID, project.ID, projectUsers[2].Id, projectUsers[3].Id, tempDate= GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);
            task = new ZTask("Complete Sign In Page Mail Text Box", milestone.ID, project.ID, projectUsers[4].Id, projectUsers[2].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);


            milestone = new Milestone("Sign Up Page Completion", project.ID, projectUsers[0].Id, projectUsers[3].Id);
            milestone.Status = (MilestoneStatus)random.Next(0, 8);
            _Connection.Insert(milestone);
            task = new ZTask("Complete Sign Up Page Name Text Box", milestone.ID, project.ID, projectUsers[1].Id, projectUsers[3].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);
            task = new ZTask("Complete Sign Up Page Mail Text Box", milestone.ID, project.ID, projectUsers[2].Id, projectUsers[2].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);

            milestone = new Milestone("Main Page Completion", project.ID, projectUsers[1].Id, projectUsers[4].Id);
            milestone.Status = (MilestoneStatus)random.Next(0, 8);
            _Connection.Insert(milestone);

            task = new ZTask("Complete Back Button", milestone.ID, project.ID, projectUsers[4].Id, projectUsers[2].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);
            task = new ZTask("Complete Home Button", milestone.ID, project.ID, projectUsers[3].Id, projectUsers[5].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);





            project = new Project("Back End Development", 1, 1);
            project.Status = (ProjectStatus)random.Next(0, 10);
            _Connection.Insert(project);
            projectUsers.Clear();
            for (int userID = random.Next(2, 4); userID < 11; userID++)
            {
                projectUsers.Add(users[userID - 1]);
                _Connection.Insert(new ProjectAndUserConnection(project.ID, userID));
            }
            projectUsers.Add(users[0]);
            _Connection.Insert(new ProjectAndUserConnection(project.ID, 1));
            //projectUsers[0].Id = projectUsers[0].Id;
            milestone = new Milestone("Changing Date Fetch Method", project.ID, projectUsers[2].Id, projectUsers[3].Id);
            milestone.Status = (MilestoneStatus)random.Next(0, 8);
            _Connection.Insert(milestone);
            task = new ZTask("Change Date Fetch Method for Milestone", milestone.ID, project.ID, projectUsers[3].Id, projectUsers[0].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);
            task = new ZTask("Change Date Fetch Method for Projects", milestone.ID, project.ID, projectUsers[1].Id, projectUsers[4].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);


            milestone = new Milestone("Change Data Fetch Efficiency", project.ID, projectUsers[1].Id, projectUsers[3].Id);
            milestone.Status = (MilestoneStatus)random.Next(0, 8);
            _Connection.Insert(milestone);
            task = new ZTask("Decrease the Home page loading time", milestone.ID, project.ID, projectUsers[3].Id, projectUsers[5].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);
            task = new ZTask("Fetch only required on starting", milestone.ID, project.ID, projectUsers[2].Id, projectUsers[4].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);

            milestone = new Milestone("Main Page Completion", project.ID, projectUsers[1].Id, projectUsers[4].Id);
            milestone.Status = (MilestoneStatus)random.Next(0, 8);
            _Connection.Insert(milestone);

            task = new ZTask("Complete Back Button", milestone.ID, project.ID, projectUsers[3].Id, projectUsers[1].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);
            task = new ZTask("Complete Home Button", milestone.ID, project.ID, projectUsers[2].Id, projectUsers[3].Id, tempDate = GetAddedDate(startDate), GetAddedDate(tempDate));
            task.Status = (ZTaskStatus)random.Next(0, 8);
            task.CompletedPercentage = random.Next(0, 10) * 10;
            _Connection.Insert(task);


            //for (int i = 1; i < 3; i++)
            //{
            //    int projectOwnerID = random.Next(1, 11);
            //    Project project = new Project(i.ToString()+ "Project",1 ,1);
            //    project.Status=(ProjectStatus)random.Next(0, 10);
            //    _Connection.Insert(project);
            //    List<ZUser> projectUsers=new List<ZUser>();
            //    for (int userID = random.Next(1,11); userID < 11; userID++)
            //    {
            //        projectUsers.Add(users[userID - 1]);
            //        _Connection.Insert(new ProjectAndUserConnection(project.ID,userID));
            //    }

            //    for (int j = 1; j<3;j++ )
            //    {
            //        int milestoneUserID = projectUsers[0].Id;
            //        Milestone milestone = new Milestone(j.ToString() + "Milestone" + " "+ i.ToString() + "Project", project.ID,milestoneUserID, milestoneUserID);
            //        milestone.Status=(MilestoneStatus)random.Next (0, 8);
            //        _Connection.Insert(milestone);
            //        for (int k = 1; k < 3; k++)
            //        {
            //            int ownerID = projectUsers[0].Id;
            //            DateTime startDate = GetAddedDate(DateTime.Today.AddDays(-5));
            //            ZTask task = new ZTask(k.ToString() + "Task" + " " + j.ToString() + "Milestone" + " " + i.ToString() + "Project", milestone.ID, project.ID, ownerID, ownerID, startDate, GetAddedDate(startDate));
            //            task.Status = (ZTaskStatus)random.Next(0, 8);
            //            task.CompletedPercentage = random.Next(0, 10) * 10;
            //            _Connection.Insert(task);
            //            List<ZSubTask> subTasks = new List<ZSubTask>();
            //            for (int l = 1; l < 3; l++)
            //            {
            //                ZSubTask subTask = new ZSubTask(task.ID, l.ToString() + "Sub Task" + k.ToString() + "Task",Priority.Medium,DateTime.Today.Date,DateTime.Today.Date.AddDays(1));
            //                subTasks.Add(subTask);
            //            }
            //            _Connection.InsertAll (subTasks);
            //            foreach (ZSubTask subTask in subTasks)
            //            {
            //                TaskAndSubTaskConnection taskAndSubTaskConnection = new TaskAndSubTaskConnection(task.ID,subTask.ID);
            //                _Connection.Insert(taskAndSubTaskConnection);
            //            }
            //        }
            //    }

            //}

        }
        DateTime GetAddedDate(DateTime date) { 

            return date.AddDays(random.Next(1,5));
        } 
    }
}
