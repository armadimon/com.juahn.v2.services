#nullable enable

namespace Juahn.V2.Services
{
    /// <summary>
    /// Installs the built-in Layer 1 infrastructure services (tick, coroutine and time) into a registry.
    /// Add this to a <see cref="ServiceBootstrapper"/> to get the framework's core services with a single line.
    /// </summary>
    public sealed class CoreServicesInstaller : IServiceInstaller
    {
        /// <inheritdoc />
        public void InstallBindings(IServiceRegistry registry)
        {
            registry.Bind<ITickService>(new TickService());
            registry.Bind<ICoroutineService>(new CoroutineService());
            registry.Bind<ITimeService, ITimeManipulator>(new TimeService());
        }
    }
}
