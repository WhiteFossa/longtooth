using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using longtooth.Client.Abstractions.Interfaces;
using longtooth.Client.Implementations.Business;
using longtooth.Common.Abstractions.Interfaces.Logger;
using longtooth.Common.Abstractions.Interfaces.MessagesProcessor;
using longtooth.Common.Abstractions.Interfaces.MessagesProtocol;
using longtooth.Common.Implementations.Logger;
using longtooth.Common.Implementations.MessagesProcessor;
using longtooth.Common.Implementations.MessagesProtocol;
using longtooth.Protocol.Abstractions.Interfaces;
using longtooth.Protocol.Implementations.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System;
using longtooth.Common.Abstractions.Interfaces.DataCompressor;
using longtooth.Common.Implementations.DataCompressor;

namespace longtooth.Desktop
{
    internal class Program
    {
        /// <summary>
        /// Dependency injection service provider
        /// </summary>
        public static ServiceProvider Di { get; set; }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            // Preparing DI
            Di = ConfigureServices()
                .BuildServiceProvider();

            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        // Setting up DI
        public static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IClient, ClientImplementation>();
            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<IMessagesProtocol, MessagesProtocol>();
            services.AddTransient<IMessagesProcessor, MessagesProcessor>();
            services.AddSingleton<ICommandToServerHeaderGenerator, CommandToServerHeaderGenerator>();
            services.AddSingleton<IClientSideMessagesProcessor, ClientSideMessagesProcessor>();
            services.AddSingleton<IDataCompressor, DataCompressor>();
            return services;
        }

        // Getting main window
        public static Window GetMainWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }
    }
}
