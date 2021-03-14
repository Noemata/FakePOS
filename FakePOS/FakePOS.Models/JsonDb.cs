using System;
using System.IO;
using System.Reflection;

using Microsoft.Toolkit.Mvvm.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FakePOS.Services
{
    public class JsonDb : IDisposable
    {
        static private object _sync = new Object();

        public JsonDb(string fileName)
        {
            FilePath = GetFilePath(fileName);
            Initialize();
            Deserialize();
        }

        static public string GetFilePath(string fileName)
        {
            ILocalFolderService loc = Ioc.Default.GetService<ILocalFolderService>();
            return Path.Combine(loc.LocalFolderPath(), fileName);
        }

        [JsonIgnore]
        protected string FilePath { get; }
        [JsonIgnore]

        protected Formatting Formatting { get; set; } = Formatting.Indented;

        private void Initialize()
        {
            var properties = this.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var property in properties)
            {
                if (property.PropertyType.GetConstructor(Type.EmptyTypes) != null)
                {
                    property.SetValue(this, Activator.CreateInstance(property.PropertyType));
                }
            }
        }

        public void SaveChanges()
        {
            Serialize();
        }

        private void Serialize()
        {
            string json = JsonConvert.SerializeObject(this, Formatting);
            lock (_sync)
            {
                File.WriteAllText(FilePath, json);
            }
        }

        private void Deserialize()
        {
            string json = null;

            lock (_sync)
            {
                if (File.Exists(FilePath))
                {
                    json = File.ReadAllText(FilePath);
                }
            }

            Deserialize(json);
        }

        protected void Deserialize(string json)
        {
            if (json != null)
            {
                var jObject = JObject.Parse(json);

                var properties = this.GetType().GetTypeInfo().DeclaredProperties;
                foreach (var property in properties)
                {
                    if (jObject.TryGetValue(property.Name, out JToken token))
                    {
                        var value = token.ToObject(property.PropertyType);
                        property.SetValue(this, value);
                    }
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
