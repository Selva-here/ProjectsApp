using Projects.Core.AppEnum;
using Projects.Core.Entity;
using Projects.Data.DataBaseAdapter;
using Projects.DatabaseConnector;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Shapes;

namespace Projects.Data.DBHandler
{
    public interface IUserDBHandler
    {
        void AddUser(ZUser user);
        List<ZUser> GetUsers(UsersType usersType, Object value,int projectID);
        List<ZUser> GetUser(int? id);
        List<ZUser> GetProjectUsers(int projectID);
    }
    public class ZUserDBHandler : IUserDBHandler
    {
        IDBAdapter _DBAdapter;
        public ZUserDBHandler()
        {
        }
        public ZUserDBHandler(IDBAdapter dBAdapter)
        {
            _DBAdapter = dBAdapter;
        }
        public List<ZUser> GetUsers(UsersType usersType, Object value, int projectID)
        {
            switch (usersType)
            {
                case UsersType.All:
                    return _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.GetUsersBaseQuery);
                case UsersType.Particular:
                    return _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.GetUsersBaseQuery + QueryStrings.IDCheckQuery, value);
                case UsersType.NameSearch:
                    return _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.GetUsersBaseQuery + " WHERE Name LIKE '%" + (string)value + "%' ");
                case UsersType.ProjectNameSearch:
                    return _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.GetUsersBaseQuery + " WHERE Name LIKE '%" + (string)value + "%' "+QueryStrings.AndQuery+QueryStrings.ProjectUserIDCheckQuery,projectID);
                case UsersType.MailID:
                    return _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.GetUsersBaseQuery + " WHERE MailID LIKE '%" + (string)value + "%' ", value);
                case UsersType.LastUser:
                    return _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.GetUsersBaseQuery + QueryStrings.WhereQuery + QueryStrings.LastUserIDQuery);
                default:
                    throw new NotImplementedException();
            }
        }
        public List<ZUser> GetUser(int? id)
        {
            return _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.GetUsersBaseQuery + QueryStrings.IDCheckQuery, id);
        }

        public List<ZUser> GetProjectUsers(int projectID)
        {
            return _DBAdapter.ExecuteQuery<ZUser>( QueryStrings.GetUsersBaseQuery + QueryStrings.WhereQuery + QueryStrings.ProjectUserIDCheckQuery,projectID);
            
        }
        public void AddUser(ZUser user)
        {
            _DBAdapter.ExecuteQuery<ZUser>(QueryStrings.InsertUserQuery , user.Name,user.MailID,user.Password,user.DesignationEnum,user.ImagePath);
        }
      
    }
}

