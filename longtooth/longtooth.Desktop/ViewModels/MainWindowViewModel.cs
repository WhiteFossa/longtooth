using Avalonia.Controls;
using longtooth.Client.Abstractions.DTOs;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Common.Abstractions.DTOs.Responses;
using longtooth.Common.Abstractions.Enums;
using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Abstractions.Models;
using longtooth.Common.Implementations.Helpers;
using longtooth.Desktop.DTOs;
using longtooth.Protocol.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        private string _currentDirectory;
        private FileDto _currentFile;
        private double _progressValue;
        private string _newDirectoryName;

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

        /// <summary>
        /// Current directory
        /// </summary>
        public string CurrentDirectory
        {
            get => _currentDirectory;
            set => this.RaiseAndSetIfChanged(ref _currentDirectory, value);
        }

        /// <summary>
        /// Current file
        /// </summary>
        public FileDto CurrentFile
        {
            get => _currentFile;
            set => this.RaiseAndSetIfChanged(ref _currentFile, value);
        }

        /// <summary>
        /// Current progressbar value [0-1]
        /// </summary>
        public double ProgressValue
        {
            get => _progressValue;
            set => this.RaiseAndSetIfChanged(ref _progressValue, value);
        }

        /// <summary>
        /// New directory name
        /// </summary>
        public string NewDirectoryName
        {
            get => _newDirectoryName;
            set => this.RaiseAndSetIfChanged(ref _newDirectoryName, value);
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

        /// <summary>
        /// Download current file
        /// </summary>
        public ReactiveCommand<Unit, Unit> DownloadFileAsyncCommand { get; }

        /// <summary>
        /// Upload a file
        /// </summary>
        public ReactiveCommand<Unit, Unit> UploadFileAsyncCommand { get; }

        /// <summary>
        /// Delete current file
        /// </summary>
        public ReactiveCommand<Unit, Unit> DeleteFileAsyncCommand { get; }

        /// <summary>
        /// Create new directory
        /// </summary>
        public ReactiveCommand<Unit, Unit> CreateNewDirectoryCommand { get; }

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

        private const int DownloadChunkSize = 1000000;
        private int _alreadyDownloaded;
        private List<byte> _downloadedContent;

        private const int UploadChunkSize = 1000000;
        private string _pathToUpload;
        private int _alreadyUploaded;
        private List<byte> _contentToUpload;

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

            #region Initialization

            _logger.SetLoggingFunction(AddLineToConsole);

            ProgressValue = 0;

            ServerPort = "5934";

            CurrentDirectory = @"N/A";
            CurrentFile = new FileDto()
            {
                Name = @"N/A",
                FullPath = "",
                Size = 0
            };

            #endregion

            #region Commands binding

            ConnectAsyncCommand = ReactiveCommand.Create(ConnectAsync);
            DisconnectAsyncCommand = ReactiveCommand.Create(DisconnectAsync);
            PingAsyncCommand = ReactiveCommand.Create(PingAsync);
            GracefulDisconnectAsyncCommand = ReactiveCommand.Create(GracefulDisconnectAsync);
            GetMountpointsAsyncCommand = ReactiveCommand.Create(GetMountpointsAsync);
            DownloadFileAsyncCommand = ReactiveCommand.Create(DownloadFileAsync);
            UploadFileAsyncCommand = ReactiveCommand.Create(UploadFileAsync);
            DeleteFileAsyncCommand = ReactiveCommand.Create(DeleteFileAsync);
            CreateNewDirectoryCommand = ReactiveCommand.Create(CreateDirectoryAsync);

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

        private async void OnNewMessageAsync(IReadOnlyCollection<byte> decodedMessage)
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

                case CommandType.DownloadFile:
                    var fileResponse = runResult as DownloadFileRunResult;
                    if (!fileResponse.File.IsSuccessful)
                    {
                        await _logger.LogErrorAsync("Failed download file!");
                        return;
                    }

                    _alreadyDownloaded += (int)fileResponse.File.Length;

                    ProgressValue = _alreadyDownloaded / (double)CurrentFile.Size;

                    _downloadedContent.AddRange(fileResponse.File.Content);

                    if (_alreadyDownloaded == CurrentFile.Size)
                    {
                        // Done
                        ProgressValue = 0;
                        File.WriteAllBytes(CurrentFile.Name, _downloadedContent.ToArray());
                    }
                    else
                    {
                        // Next chunk
                        var chunkSize = CurrentFile.Size - _alreadyDownloaded;
                        if (chunkSize > DownloadChunkSize)
                        {
                            chunkSize = DownloadChunkSize;
                        }

                        await PrepareAndSendCommand(_commandGenerator.GenerateDownloadCommand(CurrentFile.FullPath,
                            (ulong)_alreadyDownloaded, (uint)chunkSize));
                    }

                    break;

                case CommandType.CreateFile:
                    var createFileResponse = runResult as CreateFileRunResult;
                    if (!createFileResponse.CreateFileResult.IsSuccessful)
                    {
                        await _logger.LogErrorAsync("Failed to create file!");
                        return;
                    }

                    await _logger.LogInfoAsync("File created, going to upload...");

                    // Uploading file
                    _alreadyUploaded = 0;
                    ProgressValue = 0;
                    if (_contentToUpload.Count > UploadChunkSize)
                    {
                        // Multichunk
                        var uploadBuffer = _contentToUpload.GetRange(0, UploadChunkSize);
                        await PrepareAndSendCommand(_commandGenerator.UpdateFileCommand(_pathToUpload, 0, uploadBuffer.ToArray()));
                    }
                    else
                    {
                        // Single chunk
                        await PrepareAndSendCommand(_commandGenerator.UpdateFileCommand(_pathToUpload, 0, _contentToUpload.ToArray()));
                    }

                    break;

                case CommandType.UpdateFile:
                    var updateFileResponse = runResult as UpdateFileRunResult;
                    if (!updateFileResponse.UpdateFileResult.IsSuccessful)
                    {
                        await _logger.LogErrorAsync("Failed to update file!");
                        return;
                    }

                    _alreadyUploaded += (int)updateFileResponse.UpdateFileResult.BytesWritten;
                    ProgressValue = _alreadyUploaded / (double)_contentToUpload.Count;

                    if (_alreadyUploaded == _contentToUpload.Count)
                    {
                        // Done
                        ProgressValue = 0;
                    }
                    else
                    {
                        var uploadBytesCount = _contentToUpload.Count - _alreadyUploaded;
                        if (uploadBytesCount > UploadChunkSize)
                        {
                            uploadBytesCount = UploadChunkSize;
                        }

                        var uploadBuffer = _contentToUpload.GetRange(_alreadyUploaded, uploadBytesCount);
                        await PrepareAndSendCommand(_commandGenerator.UpdateFileCommand(_pathToUpload, (ulong)_alreadyUploaded, uploadBuffer.ToArray()));
                    }

                    break;

                case CommandType.DeleteFile:
                    var deleteFileResponse = runResult as DeleteFileRunResult;

                    if (!deleteFileResponse.DeleteFileResult.IsSuccessful)
                    {
                        await _logger.LogErrorAsync("Failed to delete file");
                    }

                    await _logger.LogInfoAsync("File successfully deleted");

                    break;

                case CommandType.DeleteDirectory:
                    var deleteDirectoryResponse = runResult as DeleteDirectoryRunResult;

                    if (!deleteDirectoryResponse.DeleteDirectoryResult.IsSuccessful)
                    {
                        await _logger.LogErrorAsync("Failed to delete directory");
                    }

                    await _logger.LogInfoAsync("Directory successfully deleted");

                    break;

                case CommandType.CreateDirectory:
                    var createDirectoryResponse = runResult as CreateDirectoryRunResult;

                    if (!createDirectoryResponse.CreateDirectoryResult.IsSuccessful)
                    {
                        await _logger.LogErrorAsync("Failed to create directory");
                    }

                    await _logger.LogInfoAsync("Directory successfully created");

                    break;

                default:
                    throw new InvalidOperationException("Incorrect command type in response!");
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
        public async Task OnMountpointChangedAsync(string serverSidePath)
        {
            CurrentDirectory = serverSidePath;

            var getDirectoryContentCommand = _commandGenerator.GenerateGetDirectoryContentCommand(CurrentDirectory);

            await PrepareAndSendCommand(getDirectoryContentCommand);
        }

        /// <summary>
        /// Called when user clicks on file or directory
        /// </summary>
        public async Task OnDirectoryContentCellChangedAsync(DirectoryContentItemDto directoryItem)
        {
            if (!directoryItem.IsDirectory)
            {
                // File
                CurrentFile = new FileDto()
                {
                    Name = directoryItem.Name,
                    FullPath = FilesHelper.AppendFilename(CurrentDirectory, directoryItem.Name),
                    Size = directoryItem.Size
                };
                return;
            }

            // Directory
            if (directoryItem.IsDirectory && directoryItem.Name.Equals(".."))
            {
                CurrentDirectory = FilesHelper.MoveUp(CurrentDirectory);
            }
            else
            {
                CurrentDirectory = FilesHelper.MoveDown(CurrentDirectory, directoryItem.Name);
            }

            var getDirectoryContentCommand = _commandGenerator.GenerateGetDirectoryContentCommand(CurrentDirectory);

            await PrepareAndSendCommand(getDirectoryContentCommand);
        }

        /// <summary>
        /// Download file
        /// </summary>
        private async void DownloadFileAsync()
        {
            if (CurrentFile.Name.Equals(@"N/A"))
            {
                return;
            }

            ProgressValue = 0;

            _alreadyDownloaded = 0;
            _downloadedContent = new List<byte>();

            IReadOnlyCollection<byte> downloadCommand;

            if (CurrentFile.Size > DownloadChunkSize)
            {
                // Multichunk download
                downloadCommand = _commandGenerator.GenerateDownloadCommand(CurrentFile.FullPath, 0, DownloadChunkSize);
            }
            else
            {
                // Single chunk download
                downloadCommand = _commandGenerator.GenerateDownloadCommand(CurrentFile.FullPath, 0, (uint)CurrentFile.Size);
            }

            await PrepareAndSendCommand(downloadCommand);
        }

        /// <summary>
        /// Upload file
        /// </summary>
        private async void UploadFileAsync()
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileDialogFilter() { Name = "Any file", Extensions = { "*" } });
            dialog.AllowMultiple = false;

            var dialogResult = await dialog.ShowAsync(Program.GetMainWindow());

            if (dialogResult == null)
            {
                return;
            }

            var pathToFile = dialogResult.FirstOrDefault();
            var filename = Path.GetFileName(pathToFile);

            var remotePath = CurrentDirectory + $@"/{ filename }";

            var content = File.ReadAllBytes(pathToFile);

            _alreadyUploaded = 0;
            _pathToUpload = remotePath;
            _contentToUpload = content.ToList<byte>();

            var createCommand = _commandGenerator.CreateFileCommand(remotePath);
            await PrepareAndSendCommand(createCommand);
        }

        private async void DeleteFileAsync()
        {
            if (CurrentFile.Name == @"N/A")
            {
                return;
            }

            var deleteCommand = _commandGenerator.DeleteFileCommand(CurrentFile.FullPath);
            await PrepareAndSendCommand(deleteCommand);
        }

        private async void DeleteDirectoryAsync()
        {
            if (CurrentDirectory == @"N/A")
            {
                return;
            }

            var deleteCommand = _commandGenerator.DeleteDirectoryCommand(CurrentDirectory);
            await PrepareAndSendCommand(deleteCommand);
        }

        private async void CreateDirectoryAsync()
        {
            if (NewDirectoryName.Equals(string.Empty))
            {
                return;
            }

            var createDirectoryCommand = _commandGenerator.CreateDirectoryCommand(CurrentDirectory + @"/" + NewDirectoryName);
            await PrepareAndSendCommand(createDirectoryCommand);
        }
    }
}
