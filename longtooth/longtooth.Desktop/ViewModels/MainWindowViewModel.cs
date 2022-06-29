using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Abstractions.Models;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Vfs.Linux.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive;
using System.Threading.Tasks;

namespace longtooth.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        #region Bound properties

        private string _serverIp;
        private string _serverPort;
        private string _consoleText;
        private int _consoleCaretIndex;
        private string _localMountpoint;
        private string _diskLetter;

        /// <summary>
        /// Server IP
        /// </summary>
        public string ServerIp
        {
            get => _serverIp;
            set => this.RaiseAndSetIfChanged(ref _serverIp, value);
        }

        /// <summary>
        /// Server port
        /// </summary>
        public string ServerPort
        {
            get => _serverPort;
            set => this.RaiseAndSetIfChanged(ref _serverPort, value);
        }

        /// <summary>
        /// Text in console
        /// </summary>
        public string ConsoleText
        {
            get => _consoleText;
            set => this.RaiseAndSetIfChanged(ref _consoleText, value);
        }

        /// <summary>
        /// Consone caret index (to scroll programmatically)
        /// </summary>
        public int ConsoleCaretIndex
        {
            get => _consoleCaretIndex;
            set => this.RaiseAndSetIfChanged(ref _consoleCaretIndex, value);
        }

        /// <summary>
        /// FUSE will be mounted here
        /// </summary>
        public string LocalMountpoint
        {
            get => _localMountpoint;
            set => this.RaiseAndSetIfChanged(ref _localMountpoint, value);
        }

        public string DiskLetter
        {
            get => _diskLetter;
            set => this.RaiseAndSetIfChanged(ref _diskLetter, value);
        }

        #endregion

        #region Commands

        /// <summary>
        /// Connect to server command
        /// </summary>
        public ReactiveCommand<Unit, Unit> ConnectAsyncCommand { get; }

        /// <summary>
        /// Disconnect from server command
        /// </summary>
        public ReactiveCommand<Unit, Unit> DisconnectAsyncCommand { get; }

        /// <summary>
        /// Disconnect from server gracefully
        /// </summary>
        public ReactiveCommand<Unit, Unit> GracefulDisconnectAsyncCommand { get; }

        /// <summary>
        /// Mount FUSE
        /// </summary>
        public ReactiveCommand<Unit, Unit> MountFuseAsyncCommand { get; }

        /// <summary>
        /// Umount FUSE
        /// </summary>
        public ReactiveCommand<Unit, Unit> UnmountFuseAsyncCommand { get; }

        /// <summary>
        /// Mount Dokan
        /// </summary>
        public ReactiveCommand<Unit, Unit> MountDokanAsyncCommand { get; }

        /// <summary>
        /// Unmount Dokan
        /// </summary>
        public ReactiveCommand<Unit, Unit> UnmountDokanAsyncCommand { get; }

        #endregion

        /// <summary>
        /// Main application model
        /// </summary>
        private MainModel _mainModel { get; }

        private readonly IClient _client;
        private readonly ILogger _logger;
        private readonly IMessagesProcessor _messagesProcessor;
        private readonly ICommandToServerHeaderGenerator _commandGenerator;
        private readonly IClientSideMessagesProcessor _clientSideMessagesProcessor;
        private readonly IVfsManager _vfsLinux;
        private readonly IClientService _clientService;

        public MainWindowViewModel(MainModel model)
        {
            _mainModel = model ?? throw new ArgumentNullException(nameof(model));

            #region DI

            _client = Program.Di.GetService<IClient>();
            _client.SetupResponseCallback(OnServerResponse);

            _logger = Program.Di.GetService<ILogger>();

            _messagesProcessor = Program.Di.GetService<IMessagesProcessor>();

            _commandGenerator = Program.Di.GetService<ICommandToServerHeaderGenerator>();
            _clientSideMessagesProcessor = Program.Di.GetService<IClientSideMessagesProcessor>();

            _vfsLinux = Program.Di.GetService<IVfsManager>();

            _clientService = Program.Di.GetService<IClientService>();

            #endregion

            #region Initialization

            _messagesProcessor.SetupOnNewMessageDelegate(OnNewMessageAsync);

            // TODO : Remove me, debug
            LocalMountpoint = @"/home/fossa/longtooth-mountpoint";

            _logger.SetLoggingFunction(AddLineToConsole);

            ServerPort = "5934";

            #endregion

            #region Commands binding

            ConnectAsyncCommand = ReactiveCommand.Create(ConnectAsync);
            DisconnectAsyncCommand = ReactiveCommand.Create(DisconnectAsync);

            GracefulDisconnectAsyncCommand = ReactiveCommand.Create(GracefulDisconnectAsync);

            MountFuseAsyncCommand = ReactiveCommand.Create(MountFuseAsync);
            UnmountFuseAsyncCommand = ReactiveCommand.Create(UnmountFuseAsync);

            MountDokanAsyncCommand = ReactiveCommand.Create(MountDokanAsync);
            UnmountDokanAsyncCommand = ReactiveCommand.Create(UnmountDokanAsync);

            #endregion
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        public async void ConnectAsync()
        {
            _mainModel.ServerIp = IPAddress.Parse(_serverIp);
            _mainModel.ServerPort = int.Parse(_serverPort);

            await _client.ConnectAsync(new ConnectionDto(_mainModel.ServerIp, _mainModel.ServerPort));
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public async void DisconnectAsync()
        {
            await _client.DisconnectAsync();
        }

        private async void OnServerResponse(IReadOnlyCollection<byte> response)
        {
            _messagesProcessor.OnNewMessageArrive(response);
        }

        /// <summary>
        /// Adds a new text line to console. Feed it to logger
        /// </summary>
        public void AddLineToConsole(string line)
        {
            ConsoleText += $"{line}{Environment.NewLine}";

            ConsoleCaretIndex = ConsoleText.Length;
        }

        private async Task OnNewMessageAsync(IReadOnlyCollection<byte> decodedMessage)
        {
            var response = _clientSideMessagesProcessor.ParseMessage(decodedMessage);
            var runResult = await response.RunAsync();

            switch (runResult.ResponseToCommand)
            {
                case CommandType.Ping:
                    await _logger.LogInfoAsync("Pong!");
                    break;

                case CommandType.Exit:
                    await _client.DisconnectGracefullyAsync();
                    await _logger.LogInfoAsync("Disconnected");
                    break;

                default:
                    await _clientService.OnNewMessageAsync(decodedMessage);
                    break;
            }
        }

        private async Task PrepareAndSendCommand(IReadOnlyCollection<byte> commandMessage)
        {
            var encodedMessage = _messagesProcessor.PrepareMessageToSend(new List<byte>(commandMessage));
            await _client.SendAsync(encodedMessage);
        }

        /// <summary>
        /// Ping server
        /// </summary>
        private async void PingAsync()
        {
            await _logger.LogInfoAsync("Ping");

            var pingCommandMessage = _commandGenerator.GeneratePingCommand();

            await PrepareAndSendCommand(pingCommandMessage);
        }

        /// <summary>
        /// Exit gracefully
        /// </summary>
        private async void GracefulDisconnectAsync()
        {
            await _logger.LogInfoAsync("Disconnecting");

            var exitCommandMessage = _commandGenerator.GenerateExitCommand();

            await PrepareAndSendCommand(exitCommandMessage);
        }

        private async void MountFuseAsync()
        {
            try
            {
                await _vfsLinux.MountAsync(LocalMountpoint);
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
            }
        }

        private async void UnmountFuseAsync()
        {
            await _vfsLinux.UnmountAsync();
        }

        private async void MountDokanAsync()
        {
            try
            {
                _logger.LogInfoAsync("Mounting");
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
            }
        }

        private async void UnmountDokanAsync()
        {
            await _logger.LogInfoAsync("Unmounting");
        }
    }
}
