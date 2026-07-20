#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Juahn.V2.Services
{
    /// <summary>
    /// The composition root of the framework. Place one on a scene game object (or spawn it) and register
    /// <see cref="IServiceInstaller"/> implementations to bind services into the global
    /// <see cref="ServiceLocator"/> during startup. The bootstrapper survives scene loads.
    /// </summary>
    /// <remarks>
    /// Installers added in the inspector are not supported because <see cref="IServiceInstaller"/> is a plain
    /// interface; register them from code via <see cref="AddInstaller"/> before <c>Awake</c>, or subclass and
    /// override <see cref="CollectInstallers"/>.
    /// </remarks>
    [DisallowMultipleComponent]
    public class ServiceBootstrapper : MonoBehaviour
    {
        [Tooltip("Install the built-in tick, coroutine and time services automatically.")]
        [SerializeField] private bool _installCoreServices = true;

        private readonly List<IServiceInstaller> _installers = new List<IServiceInstaller>();

        private bool _installed;

        /// <summary>The registry this bootstrapper installs into. Defaults to the global service locator.</summary>
        public IServiceRegistry Registry { get; private set; } = ServiceLocator.Instance;

        /// <summary>
        /// Adds an installer to be run when the bootstrapper installs bindings. Must be called before the
        /// bindings are installed (before <c>Awake</c> completes) to take effect.
        /// </summary>
        public ServiceBootstrapper AddInstaller(IServiceInstaller installer)
        {
            if (installer == null)
            {
                throw new ArgumentNullException(nameof(installer), "Cannot add a null service installer.");
            }

            if (_installed)
            {
                throw new InvalidOperationException(
                    "Cannot add installers after bindings have already been installed. " +
                    "Add installers before the bootstrapper's Awake runs.");
            }

            _installers.Add(installer);

            return this;
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Install();
        }

        protected virtual void OnDestroy()
        {
            if (_installed)
            {
                DisposeCoreServices();
            }
        }

        /// <summary>
        /// Override to supply installers from a subclass. Called once, before installers added through
        /// <see cref="AddInstaller"/> are run.
        /// </summary>
        protected virtual IEnumerable<IServiceInstaller> CollectInstallers()
        {
            yield break;
        }

        private void Install()
        {
            if (_installed)
            {
                return;
            }

            if (_installCoreServices)
            {
                new CoreServicesInstaller().InstallBindings(Registry);
            }

            foreach (var installer in CollectInstallers())
            {
                RunInstaller(installer);
            }

            for (var i = 0; i < _installers.Count; i++)
            {
                RunInstaller(_installers[i]);
            }

            _installed = true;
        }

        private void RunInstaller(IServiceInstaller installer)
        {
            if (installer == null)
            {
                return;
            }

            try
            {
                installer.InstallBindings(Registry);
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[ServiceBootstrapper] Installer {installer.GetType().Name} failed during " +
                    $"InstallBindings: {exception.Message}");
                throw;
            }
        }

        private void DisposeCoreServices()
        {
            if (Registry.TryResolve<ITickService>(out var tick) && tick != null)
            {
                tick.Dispose();
            }

            if (Registry.TryResolve<ICoroutineService>(out var coroutine) && coroutine != null)
            {
                coroutine.Dispose();
            }
        }
    }
}
