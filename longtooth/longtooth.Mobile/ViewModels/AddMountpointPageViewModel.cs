using Android.Content.Res;
using longtooth.Abstractions.Interfaces.Models;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Mobile.Abstractions.FilesPicke;
using longtooth.Mobile.Abstractions.FilesPicker;
using longtooth.Mobile.Abstractions.Interfaces.UserNotifier;
using longtooth.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace longtooth.Mobile.ViewModels
{
    public class AddMountpointPageViewModel : BindableObject
    {
        #region Commands

        /// <summary>
        /// Command, executed when user selects path
        /// </summary>
        public ICommand SelectPathCommand { get; }

        /// <summary>
        /// Command, executed when user press "Accept"
        /// </summary>
        public ICommand AcceptCommand { get; }

        /// <summary>
        /// Command, executed when user press "Cancel"
        /// </summary>
        public ICommand CancelCommand { get; }

        #endregion

        private string _name;
        private string _path;

        /// <summary>
        /// Mountpoint name
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }

        private INavigation _navigation;
        private readonly IUserNotifier _userNotifier;
        private readonly MainModel _mainModel;

        public AddMountpointPageViewModel()
        {
            _userNotifier = App.Container.Resolve<IUserNotifier>();
            _mainModel = App.Container.Resolve<IMainModel>() as MainModel;

            SelectPathCommand = new Command(async () => await OnSelectPathAsync());
            AcceptCommand = new Command(async () => await OnAcceptAsync());
            CancelCommand = new Command(async () => await OnCancelAsync());
        }

        /// <summary>
        /// Call this after model initialization
        /// </summary>
        public void SetNavigationProperty(INavigation navigation)
        {
            _navigation = navigation ?? throw new ArgumentNullException(nameof(navigation));
        }

        /// <summary>
        /// Resets mountpoint parameters
        /// </summary>
        public void Reset()
        {
            Name = string.Empty;
            Path = string.Empty;
        }

        public async Task OnSelectPathAsync()
        {
            var directorySelectionDialog = App.Container.Resolve<IFilesPicker>();
            directorySelectionDialog.Setup(FileSelectionMode.FolderChoose);

            var path = await directorySelectionDialog.GetFileOrDirectoryAsync(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);

            if (path == null)
            {
                return;
            }

            Path = path;
        }

        public async Task OnAcceptAsync()
        {
            if (Name.Equals(string.Empty) || Path.Equals(string.Empty))
            {
                await _userNotifier.ShowErrorMessageAsync("Warning", "Name and path have to be specified!");
                return;
            }

            var mountpoint = new MountpointDto(Name, Path);

            await _mainModel.MainPageViewModel.OnAddNewMountpointAsync(mountpoint);

            await _navigation.PopModalAsync();
        }

        public async Task OnCancelAsync()
        {
            // Just closing
            await _navigation.PopModalAsync();
        }

    }
}
