#nullable enable
using System;

namespace Juahn.V2.Services
{
    /// <summary>
    /// Read-only view of the service container.
    /// Allows resolving instances that were previously bound through an <see cref="IServiceRegistry"/>.
    /// </summary>
    /// <remarks>
    /// Follows the "Inversion of Control" principle.
    /// </remarks>
    public interface IServiceLocator
    {
        /// <summary>
        /// Requests the instance bound to the type <typeparamref name="T"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <typeparamref name="T"/> has not been bound yet.
        /// </exception>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Tries to resolve the instance bound to the type <typeparamref name="T"/>.
        /// Returns <c>true</c> when the instance is available, <c>false</c> otherwise.
        /// </summary>
        bool TryResolve<T>(out T? instance) where T : class;
    }
}
