using longtooth.Abstractions.DTOs;
using System.Threading.Tasks;
using static Xamarin.Essentials.Permissions;

namespace longtooth.Abstractions.Interfaces.Permissions
{
    public interface IPermissionsManager
    {
        /// <summary>
        /// Asks user for permission, if user agree - returns true, otherwise false
        /// </summary>
        Task<bool> RequestPermissionAsync<T>() where T : BasePermission, new ();

        /// <summary>
        /// Request root access for application
        /// </summary>
        RootRequestResultDto RequestRootAccess();
    }
}
