using SecureLibrary.Core.Features;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace SecureLibrary.Core
{
    public class PluginCollection : ObservableCollection<Plugin>
    {
        public IEnumerable<Assembly> Assemblies => this.Select(i => i.Assembly);

        public void LoadLibraries(string directory)
        {
            foreach (string dll in Directory.GetFiles(directory, "*.dll"))
            {
                LoadLibrary(dll);
            }
        }

        public Assembly? LoadLibrary(string dll)
        {
            try
            {
                return Assembly.LoadFile(dll);
            }
            catch (Exception ex)
            {
                App.MessageBox("Error", "Failed to load plugin library: " + ex.Message,
                               MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return null;
            }
        }

        public void LoadPlugins(string directory)
        {
            foreach (string dll in Directory.GetFiles(directory, "*.dll"))
            {
                LoadPlugin(dll);
            }
        }

        public Plugin? LoadPlugin(string dll)
        {
            try
            {
                Plugin plugin = new(Path.GetFileName(dll), Assembly.LoadFile(dll));
                Add(plugin);
                return plugin;
            }
            catch (Exception ex)
            {
                App.MessageBox("Error", "Failed to load plugin: " + ex.Message,
                               MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return null;
            }
        }

        public void LoadFeatures(FeatureCollection features)
        {
            foreach (Plugin plugin in this)
            {
                plugin.AddRegistered(features);
            }
        }
    }
}
