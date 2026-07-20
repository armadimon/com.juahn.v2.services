#nullable enable
using System;
using System.Collections.Generic;

namespace Juahn.V2.Services
{
    /// <inheritdoc cref="IServiceRegistry" />
    public sealed class ServiceContainer : IServiceRegistry
    {
        private readonly Dictionary<Type, object> _bindings = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<object>> _factories = new Dictionary<Type, Func<object>>();

        /// <inheritdoc />
        public IServiceRegistry Bind<T>(T instance) where T : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance), $"Cannot bind a null instance to {typeof(T)}.");
            }

            BindInternal(typeof(T), instance);

            return this;
        }

        /// <inheritdoc />
        public IServiceRegistry Bind<T1, T2>(object instance)
            where T1 : class
            where T2 : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance), $"Cannot bind a null instance to {typeof(T1)} and {typeof(T2)}.");
            }

            EnsureAssignable<T1>(instance);
            EnsureAssignable<T2>(instance);

            BindInternal(typeof(T1), instance);
            BindInternal(typeof(T2), instance);

            return this;
        }

        /// <inheritdoc />
        public IServiceRegistry Bind<T1, T2, T3>(object instance)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance),
                    $"Cannot bind a null instance to {typeof(T1)}, {typeof(T2)} and {typeof(T3)}.");
            }

            EnsureAssignable<T1>(instance);
            EnsureAssignable<T2>(instance);
            EnsureAssignable<T3>(instance);

            BindInternal(typeof(T1), instance);
            BindInternal(typeof(T2), instance);
            BindInternal(typeof(T3), instance);

            return this;
        }

        /// <inheritdoc />
        public IServiceRegistry BindFactory<T>(Func<T> factory) where T : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory), $"Cannot bind a null factory to {typeof(T)}.");
            }

            var type = typeof(T);

            if (!type.IsInterface)
            {
                throw new ArgumentException($"Cannot bind factory because {type} is not an interface.");
            }

            if (_bindings.ContainsKey(type) || _factories.ContainsKey(type))
            {
                throw new ArgumentException($"The type {type} is already bound.");
            }

            _factories.Add(type, () => factory());

            return this;
        }

        /// <inheritdoc />
        public T Resolve<T>() where T : class
        {
            if (TryResolve<T>(out var instance) && instance != null)
            {
                return instance;
            }

            throw new InvalidOperationException(
                $"The type {typeof(T)} is not bound. Bind it through an {nameof(IServiceInstaller)} " +
                $"before resolving, or verify the {nameof(ServiceBootstrapper)} ran.");
        }

        /// <inheritdoc />
        public bool TryResolve<T>(out T? instance) where T : class
        {
            var type = typeof(T);

            if (_bindings.TryGetValue(type, out var bound))
            {
                instance = (T)bound;
                return true;
            }

            if (_factories.TryGetValue(type, out var factory))
            {
                var created = factory()
                    ?? throw new InvalidOperationException($"The factory bound to {type} returned null.");

                _bindings.Add(type, created);
                _factories.Remove(type);

                instance = (T)created;
                return true;
            }

            instance = null;
            return false;
        }

        /// <inheritdoc />
        public bool Clean<T>() where T : class
        {
            var type = typeof(T);
            var removedBinding = _bindings.Remove(type);
            var removedFactory = _factories.Remove(type);

            return removedBinding || removedFactory;
        }

        /// <inheritdoc />
        public void Clean()
        {
            _bindings.Clear();
            _factories.Clear();
        }

        private void BindInternal(Type type, object instance)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException($"Cannot bind {instance} because {type} is not an interface.");
            }

            if (_bindings.ContainsKey(type) || _factories.ContainsKey(type))
            {
                throw new ArgumentException($"The type {type} is already bound.");
            }

            _bindings.Add(type, instance);
        }

        private static void EnsureAssignable<T>(object instance)
        {
            if (!(instance is T))
            {
                throw new ArgumentException($"The instance {instance.GetType()} does not implement {typeof(T)}.");
            }
        }
    }
}
