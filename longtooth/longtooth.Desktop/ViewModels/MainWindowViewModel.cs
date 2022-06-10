using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Abstractions.Models;
using longtooth.FilesManager.Abstractions.Interfaces;
using longtooth.Protocol.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<MountpointDto> _mountpoints = new ObservableCollection<MountpointDto>();
        private ObservableCollection<DirectoryContentItemDto> _directoryContent = new ObservableCollection<DirectoryContentItemDto>();

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
        /// Mountpoints, exported by server
        /// </summary>
        public ObservableCollection<MountpointDto> Mountpoints
        {
            get => _mountpoints;
            set => this.RaiseAndSetIfChanged(ref _mountpoints, value);
        }

        /// <summary>
        /// Directories and files of current level
        /// </summary>
        public ObservableCollection<DirectoryContentItemDto> DirectoryContent
        {
            get => _directoryContent;
            set => this.RaiseAndSetIfChanged(ref _directoryContent, value);
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
        /// Ping server
        /// </summary>
        public ReactiveCommand<Unit, Unit> PingAsyncCommand { get; }

        /// <summary>
        /// Disconnect from server gracefully
        /// </summary>
        public ReactiveCommand<Unit, Unit> GracefulDisconnectAsyncCommand { get; }

        /// <summary>
        /// Command to list mountpoints
        /// </summary>
        public ReactiveCommand<Unit, Unit> GetMountpointsAsyncCommand { get; }

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
        private readonly IFilesManager _filesManager;


        public MainWindowViewModel(MainModel model) : base()
        {
            _mainModel = model ?? throw new ArgumentNullException(nameof(model));

            #region DI

            _client = Program.Di.GetService<IClient>();
            _client.SetupResponseCallback(OnServerResponse);

            _logger = Program.Di.GetService<ILogger>();

            _messagesProcessor = Program.Di.GetService<IMessagesProcessor>();
            _messagesProcessor.SetupOnNewMessageDelegate(OnNewMessageAsync);

            _commandGenerator = Program.Di.GetService<ICommandToServerHeaderGenerator>();
            _clientSideMessagesProcessor = Program.Di.GetService<IClientSideMessagesProcessor>();

            _filesManager = Program.Di.GetService<IFilesManager>();

            #endregion

            _logger.SetLoggingFunction(AddLineToConsole);

            #region Commands binding

            ConnectAsyncCommand = ReactiveCommand.Create(ConnectAsync);
            DisconnectAsyncCommand = ReactiveCommand.Create(DisconnectAsync);
            PingAsyncCommand = ReactiveCommand.Create(PingAsync);
            GracefulDisconnectAsyncCommand = ReactiveCommand.Create(GracefulDisconnectAsync);
            GetMountpointsAsyncCommand = ReactiveCommand.Create(GetMountpointsAsync);

            #endregion
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        public async void ConnectAsync()
        {
            _mainModel.ServerIp = IPAddress.Parse(_serverIp);
            _mainModel.ServerPort = uint.Parse(_serverPort);

            await _client.ConnectAsync(new ConnectionDto(_mainModel.ServerIp, _mainModel.ServerPort));
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public async void DisconnectAsync()
        {
            await _client.DisconnectAsync();
        }

        private async void OnServerResponse(List<byte> response)
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

        private async void OnNewMessageAsync(List<byte> decodedMessage)
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

                case CommandType.GetMountpoints:
                    var getMountpointsResponse = runResult as GetMountpointsRunResult;
                    Mountpoints = new ObservableCollection<MountpointDto>(getMountpointsResponse.Mountpoints);
                    break;

                case CommandType.GetDirectoryContent:
                    var getDirectoryContentResponse = runResult as GetDirectoryContentRunResult;
                    if (!getDirectoryContentResponse.DirectoryContent.IsSuccessful)
                    {
                        await _logger.LogErrorAsync("Failed to get directory content!");
                        return;
                    }

                    DirectoryContent = new ObservableCollection<DirectoryContentItemDto>(getDirectoryContentResponse.DirectoryContent.Items);
                    break;

                default:
                    throw new InvalidOperationException("Incorrect command type in response!");
            }
        }

        private async Task PrepareAndSendCommand(byte[] commandMessage)
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

        /// <summary>
        /// Get mountpoints list
        /// </summary>
        private async void GetMountpointsAsync()
        {
            var getMountpointsMessage = _commandGenerator.GenerateGetMountpointsCommand();

            await PrepareAndSendCommand(getMountpointsMessage);
        }

        /// <summary>
        /// Mountpoint changed - need to display content for given mountpoint
        /// </summary>
        public async Task OnMountpointChanged(string serverSidePath)
        {
            var getDirectoryContentCommand = _commandGenerator.GenerateGetDirectoryContentCommand(serverSidePath);

            await PrepareAndSendCommand(getDirectoryContentCommand);
        }
    }
}
