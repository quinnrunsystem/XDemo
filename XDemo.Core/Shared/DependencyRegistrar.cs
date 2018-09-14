using System;
using Prism.Ioc;
using XDemo.Core.BusinessServices.Implementations;
using XDemo.Core.BusinessServices.Implementations.Common;
using XDemo.Core.BusinessServices.Implementations.Patients;
using XDemo.Core.BusinessServices.Implementations.Photos;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.Core.BusinessServices.Interfaces.Patients;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using Prism.Autofac;

namespace XDemo.Core.Shared
{
    /// <summary>
    /// Helper context for us to resolve our managed services, types.
    /// </summary>
    public sealed class DependencyRegistrar
    {
        private static readonly Lazy<DependencyRegistrar> Lazy = new Lazy<DependencyRegistrar>(() => new DependencyRegistrar());

        public static DependencyRegistrar Current => Lazy.Value;

        /* ==================================================================================================
         * use Pris,.Ioc.IContainerProvider instead of Autofac.IContainer.
         * BC in future, we can change dependency container easier, such as: Unity, DryIoc...
         * ================================================================================================*/
        private IContainerProvider _containerProvider;

        private DependencyRegistrar()
        {
            //hidden ctor
        }

        /// <summary>
        /// Init this instance
        /// </summary>
        /// <param name="containerExtension">Container.</param>
        public void Init(IContainerExtension containerExtension)
        {
            /* ==================================================================================================
             * Register real services first
             * Then register mock. the service with mock implementation overwrite the real implementation
             * ================================================================================================*/
            RegisterServices(containerExtension);
            RegisterMockServices(containerExtension);
            /* ==================================================================================================
             * store the main container
             * ================================================================================================*/
            _containerProvider = containerExtension as IContainerProvider;
        }

        public object Resolve(Type type, string name)
        {
            if (_containerProvider == null)
                throw new Exception($"You must call {nameof(DependencyRegistrar)}.{nameof(Init)}(containerExtension) before use!");
            return _containerProvider.Resolve(type, name);
        }

        public object Resolve(Type type)
        {
            if (_containerProvider == null)
                throw new Exception($"You must call {nameof(DependencyRegistrar)}.{nameof(Init)}(containerExtension) before use!");
            return _containerProvider.Resolve(type);
        }

        public T Resolve<T>()
        {
            if (_containerProvider == null)
                throw new Exception($"You must call {nameof(DependencyRegistrar)}.{nameof(Init)}(containerExtension) before use!");
            return _containerProvider.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            if (_containerProvider == null)
                throw new Exception($"You must call {nameof(DependencyRegistrar)}.{nameof(Init)}(containerExtension) before use!");
            return _containerProvider.Resolve<T>(name);
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
    }
}
