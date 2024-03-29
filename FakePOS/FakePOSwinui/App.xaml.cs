﻿using System;
using System.Linq;
using System.Reflection;

using Windows.ApplicationModel;

using Microsoft.UI.Xaml;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;

using FakePOS.Services;
using FakePOS.ViewModels;
using FakePOS.Providers;

namespace FakePOS
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Application version infromation.
        /// </summary>
        public static string AppVersion;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            // LT! Error CS1061 'App' does not contain a definition for 'Suspending' and no accessible extension method 'Suspending' accepting a first argument of type 'App' could be found
            //this.Suspending += OnSuspending;

            var assembly = (typeof(App)).GetTypeInfo().Assembly;
            AppVersion = assembly.GetName().Version.ToString();

            // MP! resolve: better way to force assembly load.
            typeof(FakePOS_ViewModels_ForceLoad).Assembly.GetName();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Ioc.Default.ConfigureServices(ConfigureServices());
            InitializeSettings();

            m_window = new MainWindow();
            m_window.Activate();
        }

        private IServiceProvider ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            services.AddSingleton<IResourceService, ResourceService>();
            services.AddSingleton<ILocalFolderService, LocalFolderService>();
            services.AddSingleton<ILoggingService, DebugLogger>();
            services.AddTransient<INavigationService, NavigationService>();
            services.AddSingleton<IUserNotificationService, UserNotificationService>();
            services.AddSingleton<ICatalogProvider, CatalogProvider>();

            RegisterWithIoc(services);

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Initialize settings default values.
        /// </summary>
        private void InitializeSettings()
        {
            ISettingsService settings = Ioc.Default.GetService<ISettingsService>();

            // Initialize default settings
            settings.SetValue(SettingsKeys.ShowVersionInfo, true, false);
        }

        /// <summary>
        /// Register attributed classes with your Ioc container.
        /// </summary>
        /// <param name="services">The ServiceCollection to be used.</param>
        void RegisterWithIoc(ServiceCollection services)
        {
            string localname = (typeof(App)).GetTypeInfo().Assembly.GetName().Name;

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                string name = a.GetName().Name;

                if (name != localname && !name.EndsWith("ViewModels"))
                    continue;

                var types = a.GetTypes().Select(t => new { T = t, Mode = t.GetCustomAttribute<RegisterWithIocAttribute>()?.Mode })
                .Where(o => o.Mode != null && o.Mode != InstanceMode.None);

                foreach (var t in types)
                {
                    var type = t.T;
                    if (t.Mode == InstanceMode.Singleton)
                        services.AddSingleton(type);
                    else if (t.Mode == InstanceMode.Transient)
                        services.AddTransient(type);
                }
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            // Save application state and stop any background activity
        }

        private Window m_window;
    }
}
