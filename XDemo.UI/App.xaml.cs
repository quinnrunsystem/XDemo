using Xamarin.Forms.Xaml;
using Prism;
using Prism.Autofac;
using Prism.Ioc;
using Autofac;
using Xamarin.Forms;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.UI.Views.Common;
using XDemo.UI.Extensions;
using XDemo.UI.ViewModels.Common;
using System;
using System.Reflection;
using System.Globalization;
using Prism.Mvvm;
using XDemo.Core.Shared;
using System.Threading;
using XDemo.UI.ViewModels;
using XDemo.UI.Views;
using XDemo.UI.Controls.ExtendedElements;
using Akavache;
using AutoMapper;
using XDemo.UI.Models;
using XDemo.Core.BusinessServices;

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
            /* ==================================================================================================
             * Register for app navigation
             * ================================================================================================*/
            RegisterNavigation(containerRegistry);
            /* ==================================================================================================
             * Init config auto mapper for UI level
             * ================================================================================================*/
            UIMapperSettup();
            /* ==================================================================================================
             * Register for serivices which used in app
             * This moved to inner call of project for Logic test.
             * ================================================================================================*/
            DependencyRegistrar.Current.Init(containerRegistry as IContainerExtension);
        }

        private void RegisterNavigation(IContainerRegistry containerRegistry)
        {
            /* ==================================================================================================
             * todo: moved to of UI project inner call => for future UI test
             * ================================================================================================*/

            /* ==================================================================================================
             * Main navaigation containers, dont has any viewmodels. 
             * Especially, we register without the viewmodel's name, it's implicit use view name.
             * ================================================================================================*/
            containerRegistry.RegisterForNavigation<PrismLifeCycleNavigationPage>(nameof(NavigationPage));

            /* ==================================================================================================
             * As our team-rule: all pages used in app will be registerd with their explicit 
             * viewmodel's name instead of view's name.
             * Using 'nameof' key word to restrict defined more constant string values
             * ================================================================================================*/
            containerRegistry.RegisterForNavigation<MenuPage, MenuPageViewModel>(nameof(MenuPageViewModel));
            containerRegistry.RegisterForNavigation<BottomTabPage, BottomTabPageViewModel>(nameof(BottomTabPageViewModel));
            containerRegistry.RegisterForNavigation<HomePage>(nameof(HomePageViewModel));
            containerRegistry.RegisterForNavigation<LoginPage>(nameof(LoginPageViewModel));
            containerRegistry.RegisterForNavigation<SettingPage>(nameof(SettingPageViewModel));
            containerRegistry.RegisterForNavigation<TransactionPage>(nameof(TransactionPageViewModel));
            containerRegistry.RegisterForNavigation<PhotoDetailPage>(nameof(PhotoDetailPageViewModel));
            containerRegistry.RegisterForNavigation<DetailAPage, DetailAPageViewModel>(nameof(DetailAPageViewModel));
            containerRegistry.RegisterForNavigation<DetailBPage, DetailBPageViewModel>(nameof(DetailBPageViewModel));
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
            /* ==================================================================================================
             * Init the first navigation of our app
             * ================================================================================================*/
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

        void UIMapperSettup()
        {
            Mapper.Initialize(cfg => {
                cfg.AddProfile<AutoMapperUIProfile>();
                cfg.AddProfile<AutoMapperCoreProfile>();
            });
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
