using Xamarin.Forms.Xaml;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
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
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.BusinessServices.Implementations.Photos;
using XDemo.UI.ViewModels.Common;
using System;
using System.Reflection;
using System.Globalization;
using Prism.Mvvm;
using XDemo.Core.Shared;
using System.Threading;
using Akavache;

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
            /* ==================================================================================================
             * Register real services first
             * Then register mock. the service with mock implementation overwrite the real implementation
             * ================================================================================================*/
            RegisterServices(containerRegistry);
            RegisterMockServices(containerRegistry);
        }

        /// <summary>
        /// Register all service with its implementation
        /// </summary>
        /// <param name="containerRegistry">Container registry.</param>
        private void RegisterServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IStartupService, StartupService>();
            containerRegistry.Register<ISecurityService, SecurityService>();
            containerRegistry.Register<IPatientService, PatientService>();
            containerRegistry.Register<IPhotoService, PhotoService>();

            /* ==================================================================================================
             * todo: register logic services which using for app
             * ...
             * ================================================================================================*/
        }

        private void RegisterMockServices(IContainerRegistry containerRegistry)
        {
            /* ==================================================================================================
             * todo: register mock service for devs undependent develop
             * ================================================================================================*/
        }

        private void RegisterNavigation(IContainerRegistry containerRegistry)
        {
            /* ==================================================================================================
             * Main navaigation containers, dont has any viewmodels. 
             * Especially, we register without the viewmodel's name, it's implicit use view name.
             * ================================================================================================*/
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<PrismTabbedPage>();

            /* ==================================================================================================
             * As our team-convention: all pages used in app will be registerd with their explicit 
             * viewmodel's name instead of view's name.
             * Using 'nameof' key word to restrict defined more constant string values
             * ================================================================================================*/
            containerRegistry.RegisterForNavigation<HomePage>(nameof(HomePageViewModel));
            containerRegistry.RegisterForNavigation<LoginPage>(nameof(LoginPageViewModel));
            containerRegistry.RegisterForNavigation<SettingPage>(nameof(SettingPageViewModel));
            containerRegistry.RegisterForNavigation<TransactionPage>(nameof(TransactionPageViewModel));
            containerRegistry.RegisterForNavigation<PhotoDetailPage>(nameof(PhotoDetailPageViewModel));
        }

        /// <summary>
        /// map viewType to viewmodel type (base on prism default)
        /// </summary>
        /// <returns>The type to view model type.</returns>
        /// <param name="viewType">View type.</param>
        Type ViewTypeToViewModelType(Type viewType)
        {
            /* ==================================================================================================
             * Based on prism built-in map. In the future, we can change it if needed
             * ================================================================================================*/
            var viewName = viewType.FullName;
            viewName = viewName.Replace(".Views.", ".ViewModels.");
            var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            var suffix = viewName.EndsWith("View", StringComparison.Ordinal) ? "Model" : "ViewModel";
            var viewModelName = string.Format(CultureInfo.InvariantCulture, "{0}{1}, {2}", viewName, suffix, viewAssemblyName);
            return Type.GetType(viewModelName);
        }

        async void InitNavigation()
        {
            await NavigationService.GoToLoginPageAsync();
        }

        void StorageSettup()
        {
            /* ==================================================================================================
             * Your application's name. Set this at startup, this defines where your data will be stored 
             * (usually at %AppData%\[ApplicationName])
             * ================================================================================================*/
            BlobCache.ApplicationName = "XDemo";
            BlobCache.EnsureInitialized();
        }

        #region App lifecycle

        protected override void OnInitialized()
        {
            InitializeComponent();
            /* ==================================================================================================
             * Init the thread helper. store current uicontext for future use whole app
             * ================================================================================================*/
            ThreadHelper.Init(SynchronizationContext.Current);

            /* ==================================================================================================
             * Setting up the local storage
             * ================================================================================================*/
            StorageSettup();

            /* ==================================================================================================
             * Resolve the startup service, use it for prepare all metada for whole app
             * ================================================================================================*/
            var startupService = Container.Resolve<IStartupService>();
            startupService.PrepareMetaData();
            InitNavigation();
        }

        protected override void OnStart()
        {
            /* ==================================================================================================
             * Set default viewtype to viewmodel type resolver. In future, we can change the naming rule if needed
             * ================================================================================================*/
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(ViewTypeToViewModelType);
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
