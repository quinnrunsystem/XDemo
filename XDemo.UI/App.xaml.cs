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
            containerRegistry.Register<IStartupService, StartupService>();
            containerRegistry.Register<ISecurityService, SecurityService>();
            containerRegistry.Register<IPatientService, PatientService>();
            containerRegistry.Register<IPhotoService, PhotoService>();
            // todo: register logic services which using for app
            // ...
        }

        private void RegisterMockServices(IContainerRegistry containerRegistry)
        {
            // todo: register mock service for devs undependent develop
        }

        private void RegisterNavigation(IContainerRegistry containerRegistry)
        {
            //main navaigation container, dont has any viewmodels (especially)
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<PrismTabbedPage>();

            // as our team-convention: all pages used in app will be registerd with their explicit viewmodel's name instead of view's name
            // use 'nameof' key word to restrict defined more constant string values
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

        #region App lifecycle

        protected override void OnInitialized()
        {
            InitializeComponent();
            //init the thread helper. store current uicontext for future use whole app
            ThreadHelper.Init(SynchronizationContext.Current);
            //resolve the startup service
            var startupService = Container.Resolve<IStartupService>();
            //prepare all metadata using for our app
            startupService.PrepareMetaData();
            InitNavigation();
        }

        protected override void OnStart()
        {
            // set default viewtype to viewmodel type resolver. In future, we can change the naming rule if needed
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
