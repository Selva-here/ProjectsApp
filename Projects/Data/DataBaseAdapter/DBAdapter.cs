using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Core.EntityObj;
using Projects.DatabaseConnector;
using Projects.Domain;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using ZohoProjects;

namespace Projects.Data.DataBaseAdapter
{
    public interface IDBAdapter
    {
        List<T> ExecuteQuery<T>(string query, params object[] args) where T : new();
       
    }
    internal class DBAdapter : IDBAdapter
    {
        static string _DBPath;
        static SQLiteConnection _Connection;
        public static void Initialize()
        {
            _DBPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "PROJECTS.db");

            if (File.Exists(_DBPath) == false)
            {
                _Connection = new SQLiteConnection(_DBPath);
                _Connection.Query<ZUser>(QueryStrings.UserTableCreateQuery);
                _Connection.Query<Project>(QueryStrings.ProjectTableCreateQuery);
                _Connection.Query<Milestone>(QueryStrings.MilestoneTableCreateQuery);
                _Connection.Query<ZTask>(QueryStrings.TaskTableCreateQuery);
                _Connection.CreateTable<ZSubTask>();
                _Connection.Query<ProjectAndUserConnection>(QueryStrings.ProjectAndUserConnectionTableCreateQuery);
                _Connection.Query<TaskAndSubTaskConnection>(QueryStrings.TaskAndSubTaskTableCreateQuery);

                InitialDataLoader initialDataLoader = new InitialDataLoader(_Connection);
                initialDataLoader.LoadInitialData();
            }
            _Connection = new SQLiteConnection(_DBPath);
        }
        public List<T> ExecuteQuery<T>(string query, params object[] args) where T : new()
        {
            return _Connection.Query<T>(query, args);
        }

    }
}
