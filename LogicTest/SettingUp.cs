using NUnit.Framework;
using Prism.Ioc;
using Autofac;
using Prism.Autofac;
using Prism.Services;
using Prism.Behaviors;
using XDemo.Core.Shared;
using LogicTest.MockSerivces;
using AutoMapper;
using XDemo.Core.BusinessServices;

namespace LogicTest
{
    [TestFixture]
    public abstract class SettingUp
    {
        private IContainerExtension _containerExtension;

        [SetUp]
        protected virtual void SetUp()
        {
            /* ==================================================================================================
             * setting up auto mapper
             * ================================================================================================*/
            AutoMapperSettup();

            /* ==================================================================================================
             * Init container (use Autofac)
             * ================================================================================================*/
            _containerExtension = new AutofacContainerExtension(new ContainerBuilder());

            /* ==================================================================================================
             * Registers the required types of prism.
             * ================================================================================================*/
            RegisterRequiredTypes(_containerExtension);

            /* ==================================================================================================
             * Register for our types which defined in Core project
             * ================================================================================================*/
            DependencyRegistrar.Current.Init(_containerExtension);

            /* ==================================================================================================
             * finish setting
             * ================================================================================================*/
            _containerExtension.FinalizeExtension();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerRegistry">Container registry.</param>
        protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance(_containerExtension);
            containerRegistry.RegisterSingleton<IDependencyService, DependencyService>();
            /* ==================================================================================================
             * use mock service to handle expected reaction
             * ================================================================================================*/
            containerRegistry.RegisterSingleton<IPageDialogService, MockPageDialogService>();

            /* ==================================================================================================
             * todo: register other prism built-in types if needed
             * ....
             * ================================================================================================*/
        }

        void AutoMapperSettup()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperCoreProfile>());
        }
    }
}
