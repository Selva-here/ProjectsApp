using Newtonsoft.Json.Linq;
using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZohoProjects;

namespace Projects.Data.DataParser
{
   public class TaskDataParser
    {
        public static List<ZTaskObj> ParseForumPostData(string jsonResponseString)
        {

            List<ZTaskObj> tasks = new List<ZTaskObj>();
           
            var jObject = JObject.Parse(jsonResponseString);
            var jToken = jObject.Value<JToken>("PeopleJson");
            foreach (var post in jToken.Values<JToken>())
            {
          
                var id = post.Value<int>("ID");
                var name = post.Value<String>("Name");
                var priority = post.Value<Priority>("Priority");
                var status = post.Value<ZTaskStatus>("Status");
                var milestoneID = post.Value<int>("MilestoneID");
                var projectID = post.Value<int>("ProjectID");
                var ownerID = post.Value<int>("OwnerID");
                var createdUserID = post.Value<int>("CreatedUserID");
                var completedPercentage = post.Value<int>("CompletedPercentage");
                var startDate = post.Value<DateTime>("StartDate");
                var endDate = post.Value<DateTime>("EndDate");
                var description = post.Value<String>("Description");
                
                ZTaskObj task = new ZTaskObj() { ID=id,Name=name,Priority=priority,Status=status,
                    MilestoneID=milestoneID,ProjectID=projectID,OwnerID=ownerID,CreatedUserID=createdUserID,
                    CompletedPercentage=completedPercentage,StartDate=startDate,EndDate=endDate,Description=description};

                tasks.Add(task);
            }
            
            return tasks;

        }
    }
   
}
