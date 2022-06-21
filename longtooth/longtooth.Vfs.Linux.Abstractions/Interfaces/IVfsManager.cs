namespace longtooth.Vfs.Linux.Abstractions.Interfaces;

/// <summary>
/// Interface to work with FUSE on Linux
/// </summary>
public interface IVfsManager
{
    /// <summary>
    /// Mounts FUSE to a directory, specified by localPath
    /// </summary>
    Task MountAsync(string localPath);

    /// <summary>
    /// Unmount previously mounted FUSE
    /// </summary>
    Task UnmountAsync();
}
