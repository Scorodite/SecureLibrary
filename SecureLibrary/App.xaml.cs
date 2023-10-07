using SecureLibrary.Controls;
using SecureLibrary.Core;
using SecureLibrary.Core.Features;
using SecureLibrary.Core.Features.LibraryItems;
using SecureLibrary.Core.Features.Storage;
using SecureLibrary.Core.Features.Storage.Modifiers;
using SecureLibrary.Core.Storage;
using SecureLibrary.Core.Storage.Methods;
using SecureLibrary.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SecureLibrary
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            ProgramDirectory = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)!;
            PluginsDirectory = Path.Join(ProgramDirectory, "plugins");
            PluginLibrariesDirectory = Path.Join(PluginsDirectory, "libraries");
        }

        public App()
        {
            Plugins = new();
            Features = new();

            LoadPlugins();
            LoadFeatures();
        }

        public static new App Current => (App)Application.Current;

        public static IEnumerable<LibraryItemFactory> CurrentItemFactories =>
            Current.Features.ItemFactories;
        public static IEnumerable<BinaryStorageFactory> CurrentStorageFactories =>
            Current.Features.StorageFactories;
        public static IEnumerable<StorageMethodFactory> CurrentStorageMethodFactories =>
            Current.Features.StorageMethodFactories;

        public static string ProgramDirectory { get; }
        public static string PluginsDirectory { get; }
        public static string PluginLibrariesDirectory { get; }

        public PluginCollection Plugins { get; }
        public FeatureCollection Features { get; }

        public new MainWindow MainWindow
        {
            get => (MainWindow)base.MainWindow;
            set => base.MainWindow = value;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow = new();
            Features.InitAll(this);
            ProcessArgs(e.Args);
            MainWindow.Show();
        }

        private void LoadPlugins()
        {
            Directory.CreateDirectory(PluginsDirectory);
            Directory.CreateDirectory(PluginLibrariesDirectory);

            Plugins.LoadLibraries(PluginLibrariesDirectory);
            Plugins.LoadPlugins(PluginsDirectory);
        }

        private void LoadFeatures()
        {
            Features.AddRegistered(Assembly.GetCallingAssembly());
            Plugins.LoadFeatures(Features);
            Features.Sort();
        }

        private void ProcessArgs(string[] args)
        {
            BinaryStorage? pickedStorage = null;
            StorageMethod? pickedMethod = null;
            for (int i = 0; i < args.Length;)
            {
                string arg = args[i];

                if (arg.StartsWith('-'))
                {
                    int nextArg = IndexOf(args, i + 1, a => a.StartsWith('-'));
                    string[] subargs =
                        nextArg == -1 ?
                            args[(i + 1)..] :
                            nextArg == i + 1 ?
                                Array.Empty<string>() :
                                args[(i + 1)..nextArg];
                    switch (arg)
                    {
                        case "--storage":
                        case "-s":
                            pickedStorage = Features.CreateStorageFromArgs(subargs);
                            break;
                        case "--method":
                        case "-m":
                            pickedMethod = Features.CreateMethodFromArgs(subargs);
                            break;
                        case "--library":
                        case "-l":
                        case "--dll":
                        case "-d":
                            Plugins.LoadLibrary(string.Join(' ', subargs));
                            break;
                        case "--plugin":
                        case "-p":
                            Plugins.LoadPlugin(string.Join(' ', subargs))?.AddRegistered(Features, this);
                            break;
                    }

                    if (nextArg == -1)
                    {
                        break;
                    }

                    i = nextArg;
                }
                else
                {
                    i++;
                }
            }

            if (pickedStorage is not null)
            {
                MainWindow.LoadLibrary(pickedStorage, pickedMethod ?? DirectStorageMethod.Instance);
            }
        }

        private static int IndexOf<T>(T[] array, int begin, Predicate<T> predicate)
        {
            for (int i = begin; i < array.Length; i++)
            {
                if (predicate(array[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        #region Utilities

        public static string? InputBox(string title, string? initial = null)
        {
            TextBox box = new()
            {
                Text = initial ?? string.Empty,
            };
            box.CaretIndex = box.Text.Length;
            box.SelectAll();
            return ElementDialog(box, title, 64) ? box.Text : null;
        }

        public static string? PasswordBox(string title, string? initial = null)
        {
            ExtendedPasswordBox box = new()
            {
                Text = initial ?? string.Empty,
            };
            box.CaretIndex = box.Text.Length;
            box.SelectAll();
            return ElementDialog(box, title, 64) ? box.Text : null;
        }

        private static bool ElementDialog(FrameworkElement element, string title, int height)
        {
            element.Margin = new(4);
            element.HorizontalAlignment = HorizontalAlignment.Stretch;
            Window win = new()
            {
                Title = title,
                Width = 400,
                Height = height,
                ResizeMode = ResizeMode.NoResize,
                Topmost = true,
                Content = element,
            };
            win.PreviewKeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Enter:
                        win.DialogResult = true;
                        win.Close();
                        e.Handled = true;
                        break;
                    case Key.Escape:
                        win.DialogResult = false;
                        win.Close();
                        e.Handled = true;
                        break;
                }
            };
            win.Loaded += (s, e) => element.Focus();
            win.ShowDialog();
            return win.DialogResult == true;
        }

        public static MessageBoxResult MessageBox(string title, string content, MessageBoxButton button,
                                                  MessageBoxImage image, MessageBoxResult defaultResult)
        {
            return System.Windows.MessageBox.Show(content, title, button, image, defaultResult);
        }

        public static MessageBoxResult MessageBox(string title, string content) =>
            MessageBox(title, content, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);

        #endregion
    }
}
