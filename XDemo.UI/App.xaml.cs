using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using XDemo.Core.Infrastructure.Networking.ApiGateway;
using XDemo.Core.Infrastructure.Logging;
using XDemo.Core.Services.Interfaces;
using XDemo.Core.Services.Implementations;
using Autofac;
using Autofac.Core;
using System.ComponentModel;
using System.Reflection;
using XDemo.UI.Views;
using Prism.Navigation;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XDemo.UI
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer) : base(initializer)
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterServices(containerRegistry);
            RegisterMockServices(containerRegistry);
            // about navigations registration: DO NOT REGISTER ANY NAVIGATION - YOU MUST NAME YOUR VIEWS AND VIEWMODELS MATCH NAMING CONVENTIONS
        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILogger, Logger>();
            containerRegistry.Register<IDataProxy, RestApi>();
            containerRegistry.Register<IStartupService, StartupService>();
            // todo: register logic services which using for app
        }

        private void RegisterMockServices(IContainerRegistry containerRegistry)
        {
            // todo: register mock service for devs undependent develop
        }

        private void RegisterNavigation(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
        }

        void PrepareMetaData()
        {
            // todo: prepare all our metadata here, such as: defauts values, enviroment...
        }
        async void InitNavigation()
        {
            var navigationService = Container.Resolve<INavigationService>();
            await navigationService.NavigateAsync(new Uri(nameof(MainPage), UriKind.Absolute));
        }
        #region App lifecycle
        protected override void OnInitialized()
        {
            InitializeComponent();
            PrepareMetaData();
            InitNavigation();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
        #endregion
    }
}
