using System;
using System.IO;
using System.Reflection;

namespace FakePOS.Services
{
    public class ResourceService : IResourceService
    {
        public string LoadString(string name)
        {
            return LoadString<ResourceService>(name);
        }

        public string LoadString(string name, string path)
        {
            return LoadString<ResourceService>(name, path);
        }

        public string LoadString<T>(string name, string path = "FakePOS.Assets")
        {
            Assembly assembly = typeof(T).GetTypeInfo().Assembly;

            return LoadString(name, path, assembly);
        }

        public string LoadString(string name, string path, Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream($"{path}.{name}"))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }
    }
}
