using longtooth.Vfs.Linux.Abstractions.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using longtooth.Common.Abstractions.Interfaces.ClientService;
using Tmds.Fuse;

namespace longtooth.Vfs.Linux.Implementations.Implementations
{
    public class VfsManager : IVfsManager
    {
        /// <summary>
        /// Is FUSE mounted?
        /// </summary>
        private bool _isMounted = false;

        /// <summary>
        /// FUSE mounts here
        /// </summary>
        private string _localMountpoint;

        private Thread _fuseThread;

        private readonly IClientService _clientService;

        public VfsManager(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task MountAsync(string localPath)
        {
            if (_isMounted)
            {
                throw new InvalidOperationException("Already mounted!");
            }

            if (!Fuse.CheckDependencies())
            {
                throw new InvalidOperationException("FUSE dependencies aren't met!");
            }

            _localMountpoint = localPath;

            _fuseThread = new Thread(new ThreadStart(FuseMountThreadRunAsync));
            _fuseThread.Start();

            _isMounted = true;
        }

        private async void FuseMountThreadRunAsync()
        {
            using (var mount = Fuse.Mount(_localMountpoint, new Vfs(_clientService)))
            {
                await mount.WaitForUnmountAsync();
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

            Fuse.LazyUnmount(_localMountpoint);
        }
    }
}
