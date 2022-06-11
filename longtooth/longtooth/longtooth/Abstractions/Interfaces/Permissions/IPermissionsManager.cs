using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace longtooth.Abstractions.Interfaces.Permissions
{
    public interface IPermissionsManager
    {
        /// <summary>
        /// Asks user for permission, if user agree - returns true, otherwise false
        /// </summary>
        Task<bool> RequestPermission<T>(Permission permission, string message) where T : BasePermission, new();
    }
}
