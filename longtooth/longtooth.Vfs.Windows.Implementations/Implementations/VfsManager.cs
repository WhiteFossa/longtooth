using DokanNet;
using DokanNet.Logging;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using longtooth.Vfs.Windows.Abstractions.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace longtooth.Vfs.Windows.Implementations.Implementations
{
    public class VfsManager : IVfsManager
    {
        /// <summary>
        /// Is Dokan mounted?
        /// </summary>
        private bool _isMounted = false;

        /// <summary>
        /// Dokan mounts here
        /// </summary>
        private string _localMountpoint;

        private Thread _dokanThread;
        private ManualResetEvent _mountEvent;

        private IClientService _clientService;

        public VfsManager(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task MountAsync(string localMountpoint)
        {
            if (_isMounted)
            {
                throw new InvalidOperationException("Already mounted");
            }

            _localMountpoint = localMountpoint;

            _dokanThread = new Thread(new ThreadStart(DokanMountThreadRunAsync));
            _dokanThread.Start();

            _isMounted = true;
        }

        private async void DokanMountThreadRunAsync()
        {
            using (_mountEvent = new ManualResetEvent(false))
            using (var dokanLogger = new ConsoleLogger("[Dokan] "))
            using (var dokan = new Dokan(dokanLogger))
            {
                var vfs = new Vfs(_clientService);

                var dokanBuilder = new DokanInstanceBuilder(dokan)
                    .ConfigureOptions(options =>
                    {
                        options.Options = DokanOptions.DebugMode
                        | DokanOptions.StderrOutput
                        | DokanOptions.CaseSensitive
                        | DokanOptions.NetworkDrive;

                        options.MountPoint = _localMountpoint;
                    });

                using (var dokanInstance = dokanBuilder.Build(vfs))
                {
                    _mountEvent.WaitOne();
                }
            }

            // Unmounted
            _isMounted = false;
        }

        public async Task UnmountAsync()
        {
            if (!_isMounted)
            {
                throw new InvalidOperationException("Not mounted!");
            }

            _mountEvent.Set();
        }
    }
}
