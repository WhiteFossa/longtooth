using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Desktop.ViewModels;

namespace longtooth.Desktop.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void OnMountpointCellPressedAsync(object sender, DataGridCellPointerPressedEventArgs e)
        {
            var pressedItem = e.Cell.DataContext as MountpointDto;

            if (pressedItem == null)
            {
                return;
            }

            var serverSidePath = pressedItem.ServerSidePath;

            var viewModel = (sender as Control).DataContext as MainWindowViewModel;

            await viewModel.OnMountpointChangedAsync(serverSidePath);
        }

        private async void OnDirectoryContentCellPressedAsync(object sender, DataGridCellPointerPressedEventArgs e)
        {
            var pressedItem = e.Cell.DataContext as DirectoryContentItemDto;

            if (pressedItem == null)
            {
                return;
            }

            var viewModel = (sender as Control).DataContext as MainWindowViewModel;

            await viewModel.OnDirectoryContentCellChangedAsync(pressedItem);
        }
    }
}
