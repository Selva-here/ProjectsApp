using Projects.Domain.UseCase.ZUserUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Data.DBHandler;
using Projects.Core.Entity;

namespace Projects.Data.DataManager.ZUserDataManager
{
    public class GetUsersDataManager : IGetUsersDataManger
    {
        IUserDBHandler _UserDBHandler;
        public GetUsersDataManager(IUserDBHandler userDBHandler)
        {
            this._UserDBHandler = userDBHandler;
        }
        public void GetUsers(IGetUsersUseCaseRequest getUsersUseCaseRequest, IUseCaseCallBackBase<GetUsersUseCaseResponse> getUsersUseCaseCallBack)
        {
            try
            {
                List<ZUser> fetchedUsers = _UserDBHandler.GetUsers(getUsersUseCaseRequest.UserType,getUsersUseCaseRequest.Value,getUsersUseCaseRequest.ProjectID);
                GetUsersUseCaseResponse getUsersUseCaseResponse = new GetUsersUseCaseResponse(fetchedUsers,getUsersUseCaseRequest.UserType,getUsersUseCaseRequest.AutoSuggestBox);
                getUsersUseCaseCallBack.OnSuccess(getUsersUseCaseResponse);
            }
            catch (Exception e)
            {
                getUsersUseCaseCallBack.OnError(e.Message);
            }
        }
    }
}
