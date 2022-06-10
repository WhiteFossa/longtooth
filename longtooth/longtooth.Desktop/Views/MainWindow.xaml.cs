using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Desktop.ViewModels;
using System;

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

        private async void OnMountpointCellChangedAsync(object sender, EventArgs e)
        {
            var selectedItem = ((sender as DataGrid).SelectedItem as MountpointDto);

            if (selectedItem == null)
            {
                return;
            }

            var serverSidePath = selectedItem.ServerSidePath;

            var viewModel = (sender as Control).DataContext as MainWindowViewModel;

            await viewModel.OnMountpointChanged(serverSidePath);
        }
    }
}
