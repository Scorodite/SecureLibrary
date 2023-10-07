using Material.Icons;
using SecureLibrary.Core.Features;
using SecureLibrary.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SecureLibrary.ExamplePlugin
{
    [RegisterFeature]
    public class CurrentTimeFeature : Feature
    {
        public override string Name => "What itme is it now?";
        public override MaterialIconKind Icon => MaterialIconKind.ClockAlert;

        public override void Init(App app)
        {
            app.MainWindow.MenuButtons.Add(new("What time is it now?", MaterialIconKind.Clock, ShowTime_Click));
        }

        private void ShowTime_Click(MenuButton sender, MainWindow? window)
        {
            App.MessageBox("Time", $"It's {DateTime.Now:H:mm} now");
        }
    }
}
