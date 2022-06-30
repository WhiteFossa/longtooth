using System.Threading.Tasks;

namespace longtooth.Vfs.Windows.Abstractions.Interfaces
{
    /// <summary>
    /// Interface to control Dokan
    /// </summary>
    public interface IVfsManager
    {
        // <summary>
        /// Mounts Dokan to a to a given path
        /// </summary>
        Task MountAsync(string localMountpoint);

        /// <summary>
        /// Unmount previously mounted Dokan
        /// </summary>
        Task UnmountAsync();
    }
}
