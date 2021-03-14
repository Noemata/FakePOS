using System.Threading.Tasks;

namespace FakePOS.Services
{
    public interface IResourceService
    {
        string LoadString(string name);
        string LoadString(string name, string path);
        string LoadString<T>(string name, string path = "FakePOS.Assets");
    }
}
