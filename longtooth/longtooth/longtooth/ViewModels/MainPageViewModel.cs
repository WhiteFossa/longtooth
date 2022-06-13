using Acr.UserDialogs;
using longtooth.Abstractions.Interfaces.Models;
using longtooth.Abstractions.Interfaces.Permissions;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Models;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Server.Abstractions.DTOs;
using longtooth.Server.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Xamarin.Essentials.Permissions;

namespace longtooth.ViewModels
{
    internal class MainPageViewModel : BindableObject
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

        #endregion

        /// <summary>
        /// Main model
        /// </summary>
        public MainModel MainModel;

        private IServer _server;
        private IMessagesProcessor _messagesProcessor;
        private IServerSideMessagesProcessor _serverSideMessagesProcessor;
        private IPermissionsManager _permissionsManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPageViewModel()
        {
            MainModel = App.Container.Resolve<IMainModel>() as MainModel;

            _server = App.Container.Resolve<IServer>();
            _messagesProcessor = App.Container.Resolve<IMessagesProcessor>();
            _serverSideMessagesProcessor = App.Container.Resolve<IServerSideMessagesProcessor>();
            _permissionsManager = App.Container.Resolve<IPermissionsManager>();

            // Binding commands to handlers
            StartServerCommand = new Command(async () => await OnServerStartAsync());
            StopServerCommand = new Command(async() => await OnServerStopAsync());
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
                await UserDialogs.Instance.AlertAsync("Storage permission is required!", "Error", "OK");
            }

            await _server.StartAsync(OnNewDataReadFromClientAsync);
        }

        /// <summary>
        /// Called when we are receiving new data from client
        /// </summary>
        private async Task<ResponseDto> OnNewDataReadFromClientAsync(List<byte> data)
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
    }
}
