using Acr.UserDialogs;
using longtooth.Abstractions.Interfaces.Permissions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace longtooth.Droid.Implementations.Permissions
{
    public class PermissionsManager : IPermissionsManager
    {
        public async Task<bool> RequestPermission<T>(Permission permission, string message) where T : BasePermission, new()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync<T>();
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission))
                {
                    await UserDialogs.Instance.AlertAsync(message, "Error", "OK");
                }

                status = await CrossPermissions.Current.RequestPermissionAsync<T>();
            }

            if (status != PermissionStatus.Granted)
            {
                await UserDialogs.Instance.AlertAsync(message, "Error", "OK");
                return false;
            }

            return true;
        }
    }
}