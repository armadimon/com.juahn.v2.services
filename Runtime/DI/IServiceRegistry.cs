#nullable enable
using System;

namespace Juahn.V2.Services
{
    /// <summary>
    /// Write side of the service container. Marks the object as a container of binding installers
    /// and acts as the registry for all bound interfaces. Only interfaces may be bound.
    /// </summary>
    /// <remarks>
    /// Follows the "Inversion of Control" principle.
    /// </remarks>
    public interface IServiceRegistry : IServiceLocator
    {
        /// <summary>
        /// Binds the interface <typeparamref name="T"/> to the given <paramref name="instance"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if <typeparamref name="T"/> is not an interface or is already bound.
        /// </exception>
        /// <returns>This registry to allow chained calls.</returns>
        IServiceRegistry Bind<T>(T instance) where T : class;

        /// <summary>
        /// Binds two interfaces to the same <paramref name="instance"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if any bound type is not an interface or is already bound.
        /// </exception>
        /// <returns>This registry to allow chained calls.</returns>
        IServiceRegistry Bind<T1, T2>(object instance)
            where T1 : class
            where T2 : class;

        /// <summary>
        /// Binds three interfaces to the same <paramref name="instance"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if any bound type is not an interface or is already bound.
        /// </exception>
        /// <returns>This registry to allow chained calls.</returns>
        IServiceRegistry Bind<T1, T2, T3>(object instance)
            where T1 : class
            where T2 : class
            where T3 : class;

        /// <summary>
        /// Binds the interface <typeparamref name="T"/> to a lazily created instance.
        /// The <paramref name="factory"/> is invoked once on the first resolve and the result is cached.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if <typeparamref name="T"/> is not an interface or is already bound.
        /// </exception>
        /// <returns>This registry to allow chained calls.</returns>
        IServiceRegistry BindFactory<T>(Func<T> factory) where T : class;

        /// <summary>
        /// Removes the binding of the given type <typeparamref name="T"/>.
        /// Returns <c>true</c> when a binding was removed, <c>false</c> otherwise.
        /// </summary>
        bool Clean<T>() where T : class;

        /// <summary>
        /// Removes all bindings from the registry.
        /// </summary>
        void Clean();
    }
}
