using Material.Icons;
using System.ComponentModel;
using System.Windows;

namespace SecureLibrary.Windows
{
    public class MenuButton : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _Content;
        private MaterialIconKind _Icon;
        private MenuButtonClickHandler? _Clicked;
        private MenuButtonClickHandler? _RightClicked;

        public MenuButton()
        {
            _Content = string.Empty;
        }

        public MenuButton(string content, MaterialIconKind icon, MenuButtonClickHandler? clicked)
        {
            _Content = content;
            _Icon = icon;
            _Clicked = clicked;
        }

        public MenuButton(string content, MaterialIconKind icon, MenuButtonClickHandler? clicked,
                          MenuButtonClickHandler? rigthClicked) : this(content, icon, clicked)
        {
            _RightClicked = rigthClicked;
        }

        public string Content
        {
            get => _Content;
            set
            {
                _Content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        public MaterialIconKind Icon
        {
            get => _Icon;
            set
            {
                _Icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public MenuButtonClickHandler? Clicked
        {
            get => _Clicked;
            set
            {
                _Clicked = value;
                OnPropertyChanged(nameof(Clicked));
            }
        }

        public MenuButtonClickHandler? RightClicked
        {
            get => _RightClicked;
            set
            {
                _Clicked = value;
                OnPropertyChanged(nameof(RightClicked));
            }
        }

        public void Click(MainWindow? window)
        {
            Clicked?.Invoke(this, window);
        }

        public void RightClick(MainWindow? window)
        {
            RightClicked?.Invoke(this, window);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }

        public delegate void MenuButtonClickHandler(MenuButton sender, MainWindow? window);
    }
}
