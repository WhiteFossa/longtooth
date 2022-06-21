using System.Threading.Tasks;

namespace longtooth.Vfs.Linux.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to control FUSE
    /// </summary>
    public interface IVfsManager
    {
        // <summary>
        /// Mounts FUSE to a directory, specified by localPath
        /// </summary>
        Task MountAsync(string localPath);

        /// <summary>
        /// Unmount previously mounted FUSE
        /// </summary>
        Task UnmountAsync();
    }
}
