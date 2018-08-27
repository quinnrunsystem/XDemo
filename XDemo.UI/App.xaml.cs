using Xamarin.Forms.Xaml;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using XDemo.Core.Infrastructure.Networking.ApiGateway;
using XDemo.Core.Infrastructure.Logging;
using Autofac;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Implementations;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.UI.Views.Common;
using XDemo.Core.BusinessServices.Implementations.Common;
using XDemo.Core.BusinessServices.Interfaces.Patients;
using XDemo.Core.BusinessServices.Implementations.Patients;
using XDemo.UI.Extensions;

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
            IAutofacContainerExtension containerExtension = new AutofacContainerExtension(containerRegistry.GetBuilder());
            
            RegisterNavigation(containerRegistry);
            RegisterServices(containerRegistry);
            RegisterMockServices(containerRegistry);
        }

        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILogger, Logger>();
            containerRegistry.Register<IDataProxy, RestApi>();
            containerRegistry.Register<IStartupService, StartupService>();
            containerRegistry.Register<ISecurityService, SecurityService>();
            containerRegistry.Register<IPatientService, PatientService>();
            containerRegistry.Register<ISecurityService, SecurityService>();
            // todo: register logic services which using for app
            // ...
        }

        private void RegisterMockServices(IContainerRegistry containerRegistry)
        {
            // todo: register mock service for devs undependent develop
        }

        private void RegisterNavigation(IContainerRegistry containerRegistry)
        {
            // about navigations registration: DO NOT REGISTER ANY NAVIGATION - YOU MUST NAME YOUR VIEWS AND VIEWMODELS MATCH NAMING CONVENTIONS
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<TabbedPage>();
            
            /* DONT REGISTER LIKE THIS: containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
             * OR THIS: containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>("Navigation_Name_For_Main_Page");
             * JUST SIMPLIFIED LIKE THIS: containerRegistry.RegisterForNavigation<MainPage>();
             */
            containerRegistry.RegisterForNavigation<HomePage>();
            containerRegistry.RegisterForNavigation<LoginPage>();
            containerRegistry.RegisterForNavigation<SettingPage>();
            containerRegistry.RegisterForNavigation<TransactionPage>();
        }

        async void InitNavigation()
        {
            await NavigationService.GoToLoginPageAsync();
        }

        #region App lifecycle
        protected override void OnInitialized()
        {
            InitializeComponent();
            //init metadata
            var startupService = Container.Resolve<IStartupService>();
            startupService.PrepareMetaData();
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
