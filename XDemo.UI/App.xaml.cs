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
            RegisterNavigations(containerRegistry);
            RegisterServices(containerRegistry);
            RegisterMockServices(containerRegistry);
        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILogger, Logger>();
            containerRegistry.Register<IDataProxy, RestApi>();
            containerRegistry.Register<IStartupService, StartupService>();
            // todo: register logic services which using for app
        }

        private void RegisterNavigations(IContainerRegistry containerRegistry)
        {
            // todo: register for navigations
        }

        private void RegisterMockServices(IContainerRegistry containerRegistry)
        {
            // todo: register mock service for devs undependent develop
        }
        void PrepareMetaData()
        {
            // todo: prepare all our metadata here, such as: defauts values, enviroment...
        }

        #region App lifecycle
        protected override void OnInitialized()
        {
            InitializeComponent();
            PrepareMetaData();
            // todo: sample mainpage => to remove
            MainPage = new ContentPage
            {
                Content = new Label
                {
                    Text = "Hello Xamarin.Forms!",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
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
