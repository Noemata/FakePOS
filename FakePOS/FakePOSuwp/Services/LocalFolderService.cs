using Windows.Storage;

namespace FakePOS.Services
{
    public class LocalFolderService : ILocalFolderService
    {
        public string LocalFolderPath()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }
    }
}
