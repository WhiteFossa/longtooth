using longtooth.Abstractions.DTOs;
using longtooth.Abstractions.Interfaces.AppManager;
using longtooth.Abstractions.Interfaces.Models;
using longtooth.Abstractions.Interfaces.Permissions;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Mobile.Abstractions.Interfaces.UserNotifier;
using longtooth.Mobile.Views;
using longtooth.Models;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using longtooth.Server.Abstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace longtooth.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        #region Commands

        /// <summary>
        /// Command, executed when user starts server
        /// </summary>
        public ICommand StartServerCommand { get; }

        /// <summary>
        /// Command, executed when user stops server
        /// </summary>
        public ICommand StopServerCommand { get; }

        /// <summary>
        /// Exit application
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Add mountpoint
        /// </summary>
        public ICommand AddMountpointCommand { get; }

        #endregion

        /// <summary>
        /// Main model
        /// </summary>
        public MainModel MainModel;

        /// <summary>
        /// We use it for navigation
        /// </summary>
        public INavigation Navigation { get; set; }

        /// <summary>
        /// Local server IPs
        /// </summary>
        private ObservableCollection<ServerIpDto> _serverIps = new ObservableCollection<ServerIpDto>();

        public ObservableCollection<ServerIpDto> ServerIps
        {
            get
            {
                return _serverIps;
            }
            set
            {
                _serverIps = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Add mountpoint page
        /// </summary>
        private AddMountpointPageView _addMountpointPageView = new AddMountpointPageView();

        private readonly IServer _server;
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly IServerSideMessagesProcessor _serverSideMessagesProcessor;
        private readonly IPermissionsManager _permissionsManager;
        private readonly IAppManager _appManager;
        private readonly IFilesManager _filesManager;
        private readonly IUserNotifier _userNotifier;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPageViewModel()
        {
            MainModel = App.Container.Resolve<IMainModel>() as MainModel;
            MainModel.MainPageViewModel = this;

            _server = App.Container.Resolve<IServer>();
            _messagesProcessor = App.Container.Resolve<IMessagesProcessor>();
            _serverSideMessagesProcessor = App.Container.Resolve<IServerSideMessagesProcessor>();
            _permissionsManager = App.Container.Resolve<IPermissionsManager>();
            _appManager = App.Container.Resolve<IAppManager>();
            _filesManager = App.Container.Resolve<IFilesManager>();
            _userNotifier = App.Container.Resolve<IUserNotifier>();

            // Binding commands to handlers
            StartServerCommand = new Command(async () => await OnServerStartAsync());
            StopServerCommand = new Command(async() => await OnServerStopAsync());
            ExitCommand = new Command(async() => await OnExitAppAsync());
            AddMountpointCommand = new Command(async() => await AddMountpointAsync());

            // Local IPs
            var localIps = _server
                .GetLocalIps()
                .Select(ip => new ServerIpDto(ip.ToString()));

            foreach(var localIp in localIps)
            {
                ServerIps.Add(localIp);
            }
        }

        /// <summary>
        /// Called when user tries to start server
        /// </summary>
        public async Task OnServerStartAsync()
        {
            // TODO: Add "Use root mode?" setting

            //// Requesting root access
            //var rootRequestResult = _permissionsManager.RequestRootAccess();
            //if (!rootRequestResult.IsRootAccess)
            //{
            //    await UserDialogs.Instance.AlertAsync("Root access is required!", "Error", "OK");
            //}

            // And storage access
            var doWeHavePermission = await _permissionsManager.RequestPermissionAsync<StorageWrite>();
            if (!doWeHavePermission)
            {
                await _userNotifier.ShowErrorMessageAsync("Error", "Storage permission is required!");
            }

            try
            {
                await _server.StartAsync(OnNewDataReadFromClientAsync);
            }
            catch(Exception)
            {
                // TODO: Handle me
            }
        }

        /// <summary>
        /// Called when we are receiving new data from client
        /// </summary>
        private async Task<ResponseDto> OnNewDataReadFromClientAsync(IReadOnlyCollection<byte> data)
        {
            var decodedMessage = _messagesProcessor.OnNewMessageArriveServer(data);

            if (decodedMessage == null)
            {
                // We aren't ready to response now
                return new ResponseDto(false, false, new List<byte>());
            }

            return await _serverSideMessagesProcessor.ParseMessageAsync(decodedMessage);
        }

        /// <summary>
        /// Called when user tries to stop server
        /// </summary>
        public async Task OnServerStopAsync()
        {
            _server.Stop();
        }

        public async Task OnExitAppAsync()
        {
            _appManager.CloseApp();
        }

        public async Task AddMountpointAsync()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushModalAsync(_addMountpointPageView);
            });
        }

        public async Task OnAddNewMountpointAsync(MountpointDto mountpoint)
        {
            _ = mountpoint ?? throw new ArgumentNullException(nameof(mountpoint));

            try
            {
                _filesManager.AddMountpoint(mountpoint);
            }
            catch (ArgumentException)
            {
                await _userNotifier.ShowNotificationMessageAsync("Warning", "Mountpoint with this path or name already exist!");
            }
        }
    }
}
