#nullable enable
using System;

namespace Juahn.V2.Services
{
    /// <summary>
    /// Static access point to the global service container.
    /// Use this for bindings that live for the entire scope of the game.
    /// </summary>
    /// <remarks>
    /// The composition root (<see cref="ServiceBootstrapper"/>) is responsible for populating
    /// the container. Prefer injecting <see cref="IServiceLocator"/> where possible over the static access.
    /// </remarks>
    public static class ServiceLocator
    {
        private static readonly ServiceContainer Container = new ServiceContainer();

        /// <summary>
        /// The global service registry instance.
        /// </summary>
        public static IServiceRegistry Instance => Container;

        /// <inheritdoc cref="IServiceLocator.Resolve{T}" />
        public static T Resolve<T>() where T : class
        {
            return Container.Resolve<T>();
        }

        /// <inheritdoc cref="IServiceLocator.TryResolve{T}" />
        public static bool TryResolve<T>(out T? instance) where T : class
        {
            return Container.TryResolve(out instance);
        }

        /// <inheritdoc cref="IServiceRegistry.Bind{T}" />
        public static IServiceRegistry Bind<T>(T instance) where T : class
        {
            return Container.Bind(instance);
        }

        /// <inheritdoc cref="IServiceRegistry.Clean{T}" />
        public static bool Clean<T>() where T : class
        {
            return Container.Clean<T>();
        }

        /// <summary>
        /// Resolves and disposes the bound <see cref="IDisposable"/> of type <typeparamref name="T"/>,
        /// then removes it from the container.
        /// </summary>
        public static bool CleanDispose<T>() where T : class, IDisposable
        {
            if (Container.TryResolve<T>(out var instance) && instance != null)
            {
                instance.Dispose();
            }

            return Container.Clean<T>();
        }

        /// <inheritdoc cref="IServiceRegistry.Clean()" />
        public static void Clean()
        {
            Container.Clean();
        }
    }
}
