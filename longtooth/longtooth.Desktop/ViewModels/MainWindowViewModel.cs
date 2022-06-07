using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Desktop.Business.Interfaces;
using longtooth.Desktop.Models;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive;

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
        /// Send test message
        /// </summary>
        public ReactiveCommand<Unit, Unit> SendTestMessageAsyncCommand { get; }

        #endregion

        /// <summary>
        /// Main application model
        /// </summary>
        private MainModel _mainModel { get; }

        private readonly IClient _client;
        private readonly ILogger _logger;
        private readonly IMessagesProcessor _messagesProcessor;

        private readonly List<byte> _experimentalMessage;

        private const int PacketSize = 16384;
        private const int PacketsCount = 1000;

        private int _packetsCounter = 0;

        private Stopwatch _stopWatch = new Stopwatch();

        public MainWindowViewModel(MainModel model) : base()
        {
            _mainModel = model ?? throw new ArgumentNullException(nameof(model));

            #region DI

            _client = Program.Di.GetService<IClient>();
            _logger = Program.Di.GetService<ILogger>();

            _messagesProcessor = Program.Di.GetService<IMessagesProcessor>();
            _messagesProcessor.SetupOnNewMessageDelegate(OnNewMessageAsync);

            #endregion

            _logger.SetLoggingFunction(AddLineToConsole);

            #region Commands binding

            ConnectAsyncCommand = ReactiveCommand.Create(ConnectAsync);
            DisconnectAsyncCommand = ReactiveCommand.Create(DisconnectAsync);
            SendTestMessageAsyncCommand = ReactiveCommand.Create(SendTestMessageAsync);

            #endregion

            _experimentalMessage = new List<byte>();
            var random = new Random();
            for (var i = 0; i < PacketSize; i++)
            {
                _experimentalMessage.Add((byte)(random.Next() % 256));
            }
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

        /// <summary>
        /// Send test message
        /// </summary>
        private async void SendTestMessageAsync()
        {
            await _logger.LogInfoAsync($"Sending { PacketsCount} x { PacketSize } bytes");

            _stopWatch.Reset();
            _stopWatch.Start();

            _packetsCounter = 0;
            var encodedMessage = _messagesProcessor.PrepareMessageToSend(_experimentalMessage);
            await _client.SendAsync(encodedMessage, OnServerResponse);
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
            // Is data correct ?
            if (!decodedMessage.SequenceEqual(_experimentalMessage))
            {
                await _logger.LogErrorAsync("Wrong data received");

                for (var i = 0; i < decodedMessage.Count; i++)
                {
                    if (decodedMessage[i] != _experimentalMessage[i])
                    {
                        await _logger.LogErrorAsync(
                            $"I = { i }, decoded = {decodedMessage[i]}, experimental = {_experimentalMessage[i] }");
                    }
                }

                return;
            }

            _packetsCounter++;

            if (_packetsCounter >= PacketsCount)
            {
                await _logger.LogInfoAsync("Completed!");

                _stopWatch.Stop();
                var elapsed = _stopWatch.Elapsed;

                await _logger.LogInfoAsync($"Elapsed: {elapsed.TotalSeconds}");

                var totalBytes = PacketsCount * PacketSize;
                var speed = totalBytes / elapsed.TotalSeconds;

                await _logger.LogInfoAsync($"Speed: {speed}");

                return;
            }
            var encodedMessage = _messagesProcessor.PrepareMessageToSend(_experimentalMessage);
            await _client.SendAsync(encodedMessage, OnServerResponse);
        }
    }
}
