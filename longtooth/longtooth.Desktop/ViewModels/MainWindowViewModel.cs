using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Desktop.Models;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Protocol.Abstractions.Responses;
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

            #endregion

            _logger.SetLoggingFunction(AddLineToConsole);

            #region Commands binding

            ConnectAsyncCommand = ReactiveCommand.Create(ConnectAsync);
            DisconnectAsyncCommand = ReactiveCommand.Create(DisconnectAsync);
            PingAsyncCommand = ReactiveCommand.Create(PingAsync);
            GracefulDisconnectAsyncCommand = ReactiveCommand.Create(GracefulDisconnectAsync);

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
            await response.RunAsync(_logger, _client);
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
            // TODO: Remove code duplicates
            await _logger.LogInfoAsync("Exiting...");

            var exitCommandMessage = _commandGenerator.GenerateExitCommand();

            await PrepareAndSendCommand(exitCommandMessage);
        }
    }
}
