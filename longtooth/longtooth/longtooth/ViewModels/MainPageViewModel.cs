using longtooth.Abstractions.Interfaces.Models;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Implementations.MessagesProcessor;
using longtooth.Models;
using longtooth.Server.Abstractions.DTOs;
using longtooth.Server.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

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

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPageViewModel()
        {
            MainModel = App.Container.Resolve<IMainModel>() as MainModel;
            _server = App.Container.Resolve<IServer>();
            _messagesProcessor = App.Container.Resolve<IMessagesProcessor>();

            // Binding commands to handlers
            StartServerCommand = new Command(async () => await OnServerStartAsync());
            StopServerCommand = new Command(async() => await OnServerStopAsync());
        }

        /// <summary>
        /// Called when user tries to start server
        /// </summary>
        public async Task OnServerStartAsync()
        {
            await _server.StartAsync(OnNewDataReadFromClient);
        }

        /// <summary>
        /// Called when we are receiving new data from client
        /// </summary>
        private ResponseDto OnNewDataReadFromClient(List<byte> data)
        {
            var decodedMessage = _messagesProcessor.OnNewMessageArriveServer(data);

            if (decodedMessage == null)
            {
                // We aren't ready to response now
                return new ResponseDto(false, false, new List<byte>());
            }

            // Encoding outgoing message
            var encodedMessage = _messagesProcessor.PrepareMessageToSend(decodedMessage);
            var response = new ResponseDto(true, false, encodedMessage);

            return response;
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
