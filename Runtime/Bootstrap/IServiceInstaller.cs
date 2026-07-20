#nullable enable

namespace Juahn.V2.Services
{
    /// <summary>
    /// A unit of installation logic that binds a set of services into a registry. Implementations are run by
    /// the <see cref="ServiceBootstrapper"/> composition root during startup, in order.
    /// </summary>
    public interface IServiceInstaller
    {
        /// <summary>
        /// Binds this installer's services into the given <paramref name="registry"/>.
        /// </summary>
        void InstallBindings(IServiceRegistry registry);
    }
}
