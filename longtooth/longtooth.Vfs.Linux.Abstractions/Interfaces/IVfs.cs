using System.Threading.Tasks;
using longtooth.Common.Abstractions.DTOs.ClientService;

namespace longtooth.Vfs.Linux.Abstractions.Interfaces
{
    /// <summary>
    /// Get directory content delegate
    /// </summary>
    public delegate Task<FilesystemItemDto> GetDirectoryContentDelegate(string path);

    /// <summary>
    /// VFS (FUSE) interface
    /// </summary>
    public interface IVfs
    {

    }
}
