﻿using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Nancy.TinyIoc;
using longtooth.Abstractions.Interfaces.Models;
using longtooth.Models;
using longtooth.Server.Abstractions.Interfaces;
using longtooth.Server.Implementations.Business;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Implementations.MessagesProcessor;
using longtooth.Common.Abstractions.Interfaces.MessagesProtocol;
using longtooth.Common.Implementations.MessagesProtocol;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Protocol.Implementations.Implementations;

namespace longtooth.Droid
{
    [Activity(Label = "longtooth", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            #region IoC

            // Registering IoC stuff
            App.Container = new TinyIoCContainer();
            App.Container.Register<IMainModel, MainModel>().AsSingleton();
            App.Container.Register<IServer, ServerImplementation>().AsSingleton();
            App.Container.Register<IMessagesProtocol, MessagesProtocol>().AsSingleton();
            App.Container.Register<IMessagesProcessor, MessagesProcessor>().AsMultiInstance();
            App.Container.Register<IServerSideMessagesProcessor, ServerSideMessagesProcessor>().AsSingleton();
            App.Container.Register<IResponseToClientHeaderGenerator, ResponseToClientHeaderGenerator>().AsSingleton();

            #endregion

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}