using System;
using Prism.Ioc;

namespace XDemo.Core.Shared
{
    /// <summary>
    /// Helper context for us to resolve our managed services, types.
    /// </summary>
    public sealed class DependencyContext
    {
        private static readonly Lazy<DependencyContext> Lazy = new Lazy<DependencyContext>(() => new DependencyContext());
        public static DependencyContext Current => Lazy.Value;
        /* ==================================================================================================
         * use Pris,.Ioc.IContainerProvider instead of Autofac.IContainer.
         * BC in future, we can change dependency container easier, such as: Unity, DryIoc...
         * ================================================================================================*/
        private IContainerProvider _container;

        private DependencyContext()
        {
            //hidden ctor
        }

        /// <summary>
        /// Init this instance
        /// </summary>
        /// <param name="container">Container.</param>
        public void Init(IContainerProvider container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public object Resolve(Type type, string name)
        {
            if (_container == null)
                throw new Exception($"You must call {nameof(DependencyContext)}.{nameof(Init)}(container) before use!");
            return _container.Resolve(type, name);
        }

        public object Resolve(Type type)
        {
            if (_container == null)
                throw new Exception($"You must call {nameof(DependencyContext)}.{nameof(Init)}(container) before use!");
            return _container.Resolve(type);
        }

        public T Resolve<T>()
        {
            if (_container == null)
                throw new Exception($"You must call {nameof(DependencyContext)}.{nameof(Init)}(container) before use!");
            return _container.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            if (_container == null)
                throw new Exception($"You must call {nameof(DependencyContext)}.{nameof(Init)}(container) before use!");
            return _container.Resolve<T>(name);
        }
    }
}
