using Projects.Core.Entity;
using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ZUserUseCase;
using Projects.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Projects.Data.DataManager.ZUserDataManager
{
    public class AddUserDataManager: IAddUserDataManger
    {
        IUserDBHandler _UserDBHandler;
        public AddUserDataManager(IUserDBHandler userDBHandler)
        {
            this._UserDBHandler = userDBHandler;
        }

        public void AddUser(IAddUserUseCaseRequest addUserUseCaseRequest, IUseCaseCallBackBase<AddUserUseCaseResponse> addUserUseCaseCallBack)
        {
            try
            {
                _UserDBHandler.AddUser(addUserUseCaseRequest.User);
                List<ZUser> lastUser = _UserDBHandler.GetUsers(Core.AppEnum.UsersType.LastUser, null,-1);
                if (lastUser.Count < 1)
                {
                    throw new Exception("Last User Not Found");
                }
                else
                {
                    addUserUseCaseRequest.User.Id = lastUser[0].Id;
                }

                AddUserUseCaseResponse addUserUseCaseResponseUseCaseResponse = new AddUserUseCaseResponse(addUserUseCaseRequest.User);
                addUserUseCaseCallBack.OnSuccess(addUserUseCaseResponseUseCaseResponse);
            }
            catch (Exception ex)
            {
                addUserUseCaseCallBack.OnError(ex.Message);
            }
        }
    }
}
