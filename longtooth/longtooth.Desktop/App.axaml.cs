using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using longtooth.Common.Abstractions.Models;
using longtooth.Desktop.ViewModels;
using longtooth.Desktop.Views;

namespace longtooth.Desktop
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = new MainWindowViewModel(new MainModel())
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}