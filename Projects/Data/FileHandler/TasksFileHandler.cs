using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Projects.Data.FileHandler
{
    public interface ITasksFileHandler
    {
        Task<string> GetDataFromFile();
    }


    public class TasksFileHandler:ITasksFileHandler
    {
        public async Task<string> GetDataFromFile()
        {

            try
            {
                Uri uri = new Uri($"ms-appx:///Json/Tasks.json");
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                return await FileIO.ReadTextAsync(file);
            }
            catch(Exception ex)
            {

            }
           return null;
        }
    }
}
