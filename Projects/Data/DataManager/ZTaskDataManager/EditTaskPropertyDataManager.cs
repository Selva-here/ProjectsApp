using Projects.Data.DBHandler;
using Projects.Domain.UseCase.ProjectUseCase;
using Projects.Domain;
using Projects.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Projects.Domain.UseCase.ZTaskUseCase;

namespace Projects.Data.DataManager.ZTaskDataManager
{
    public class EditTaskPropertyDataManager: IEditTaskPropertyDataManger
    {
        ITaskDBHandler _TaskDBHandler { get; set; }
        public EditTaskPropertyDataManager(ITaskDBHandler taskDBHandler)
        {
            _TaskDBHandler = taskDBHandler;
        }

        public void EditTaskProperty(IEditTaskPropertyUseCaseRequest editTaskPropertyUseCaseRequest, IUseCaseCallBackBase<EditTaskPropertyUseCaseResponse> editTaskPropertyUseCaseCallBack)
        {
            try
            {
                _TaskDBHandler.EditTaskProperty(editTaskPropertyUseCaseRequest);
               EditTaskPropertyUseCaseResponse editTaskPropertyUseCaseResponse = new EditTaskPropertyUseCaseResponse(editTaskPropertyUseCaseRequest.TaskPropertyEditType);
               editTaskPropertyUseCaseCallBack.OnSuccess(editTaskPropertyUseCaseResponse);
            }
            catch (Exception ex)
            {
                editTaskPropertyUseCaseCallBack.OnError(ex.Message);
            }
        }
    }
}
