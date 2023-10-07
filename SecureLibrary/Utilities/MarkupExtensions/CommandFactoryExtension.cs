using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace SecureLibrary.Utilities.MarkupExtensions
{
    /// <summary>
    /// Creates ICommand instances from methods
    /// </summary>
    public class CommandFactoryExtension : MarkupExtension
    {
        public CommandFactoryExtension() { }

        public CommandFactoryExtension(CommandHandler? handler)
        {
            Handler = handler;
        }

        public CommandHandler? Handler { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new CommandImplementation(Handler);
        }

        public delegate void CommandHandler(object? parameter);

        private class CommandImplementation : ICommand
        {
            public event EventHandler? CanExecuteChanged;

            public CommandImplementation(CommandHandler? handler)
            {
                Handler = handler;
            }

            public CommandHandler? Handler { get; }

            public bool CanExecute(object? parameter)
            {
                return true;
            }

            public void Execute(object? parameter)
            {
                Handler?.Invoke(parameter);
            }
        }
    }
}
