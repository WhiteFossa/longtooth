using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using longtooth.Abstractions.Interfaces.AppManager;
using longtooth.Abstractions.Interfaces.Models;
using longtooth.Abstractions.Interfaces.Permissions;
using longtooth.Common.Abstractions.DAO.Mountpoints;
using longtooth.Common.Abstractions.Interfaces.DataCompressor;
using longtooth.Common.Abstractions.Interfaces.FilesManager;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Abstractions.Interfaces.MessagesProtocol;
using longtooth.Common.Implementations.DataCompressor;
using longtooth.Common.Implementations.MessagesProcessor;
using longtooth.Common.Implementations.MessagesProtocol;
using longtooth.Droid.Implementations.AppManager;
using longtooth.Droid.Implementations.DAO.Mountpoints;
using longtooth.Droid.Implementations.FilesManager;
using longtooth.Droid.Implementations.FilesPicker;
using longtooth.Droid.Implementations.PermissionsManager;
using longtooth.Droid.Implementations.UserNotifier;
using longtooth.Mobile.Abstractions.FilesPicker;
using longtooth.Mobile.Abstractions.Interfaces.UserNotifier;
using longtooth.Models;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Protocol.Implementations.Implementations;
using longtooth.Server.Abstractions.Interfaces;
using longtooth.Server.Implementations.Business;
using Nancy.TinyIoc;

namespace longtooth.Droid
{
    [Activity(Label = "longtooth", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        /// <summary>
        /// This activity (we need it to be able to exit gracefully)
        /// </summary>
        public static MainActivity Instance { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            #region DI

            // Registering DI stuff
            App.Container = new TinyIoCContainer();
            App.Container.Register<IMainModel, MainModel>().AsSingleton();
            App.Container.Register<IServer, ServerImplementation>().AsSingleton();
            App.Container.Register<IMessagesProtocol, MessagesProtocol>().AsSingleton();
            App.Container.Register<IMessagesProcessor, MessagesProcessor>().AsMultiInstance();
            App.Container.Register<IServerSideMessagesProcessor, ServerSideMessagesProcessor>().AsSingleton();
            App.Container.Register<IResponseToClientHeaderGenerator, ResponseToClientHeaderGenerator>().AsSingleton();
            App.Container.Register<IFilesManager, FilesManagerImplementation>().AsSingleton();
            App.Container.Register<IPermissionsManager, PermissionsManager>().AsSingleton();
            App.Container.Register<IAppManager, AppManager>().AsSingleton();
            App.Container.Register<IDataCompressor, DataCompressor>().AsSingleton();
            App.Container.Register<IFilesPicker, FilesPicker>().AsMultiInstance();
            App.Container.Register<IUserNotifier, UserNotifier>().AsSingleton();
            App.Container.Register<IMountpointsDao, MountpointsDao>().AsSingleton();

            #endregion

            base.OnCreate(savedInstanceState);
            Instance = this;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            UserDialogs.Init(this);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}