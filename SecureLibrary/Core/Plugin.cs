using SecureLibrary.Core.Features;
using System.Reflection;

namespace SecureLibrary.Core
{
    public class Plugin
    {
        public Plugin(string fileName, Assembly assembly)
        {
            FileName = fileName;
            Assembly = assembly;
        }

        public string FileName { get; }
        public Assembly Assembly { get; }

        public Feature[] AddRegistered(FeatureCollection features)
        {
            return features.AddRegistered(Assembly);
        }

        public Feature[] AddRegistered(FeatureCollection features, App initApp)
        {
            Feature[] registered = AddRegistered(features);
            foreach (Feature feature in registered)
            {
                feature.Init(initApp);
            }
            return registered;
        }
    }
}
