using longtooth.Abstractions.Interfaces.Models;
using longtooth.Models;
using longtooth.Server.Abstractions.Interfaces;
using System.Diagnostics;
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

        #endregion

        /// <summary>
        /// Main model
        /// </summary>
        public MainModel MainModel;

        private IServer _server;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPageViewModel()
        {
            MainModel = App.Container.Resolve<IMainModel>() as MainModel;
            _server = App.Container.Resolve<IServer>();

            // Binding commands to handlers
            StartServerCommand = new Command(async () => await OnServerStartAsync());
        }

        /// <summary>
        /// Called when user tries to start server
        /// </summary>
        public async Task OnServerStartAsync()
        {
            Debug.WriteLine("Server starting...");

            await _server.StartAsync();
        }
    }
}
