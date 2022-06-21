// using System;
// using System.Threading.Tasks;
// using longtooth.Vfs.Linux.Abstractions.Interfaces;
// using Tmds.Fuse;
//
// namespace longtooth.Vfs.Linux.Implementations.Implementations
// {
//
//     public class VfsManager : IVfsManager
//     {
//         /// <summary>
//         /// Is FUSE mounted?
//         /// </summary>
//         private bool _isMounted = false;
//
//         /// <summary>
//         /// FUSE mounts here
//         /// </summary>
//         private string _localMountpoint;
//
//         public async Task MountAsync(string localPath)
//         {
//             if (_isMounted)
//             {
//                 throw new InvalidOperationException("Already mounted!");
//             }
//
//             if (!Fuse.CheckDependencies())
//             {
//                 throw new InvalidOperationException("FUSE dependencies aren't met!");
//             }
//
//             using (var mount = Fuse.Mount(localPath, new Vfs()))
//             {
//                 await mount.WaitForUnmountAsync();
//             }
//
//             _localMountpoint = localPath;
//             _isMounted = true;
//         }
//
//         public async Task UnmountAsync()
//         {
//             if (!_isMounted)
//             {
//                 throw new InvalidOperationException("Not mounted!");
//             }
//
//             Fuse.LazyUnmount(_localMountpoint);
//
//             _isMounted = false;
//         }
//     }
// }
