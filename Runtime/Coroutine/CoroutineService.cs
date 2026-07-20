#nullable enable
using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Juahn.V2.Services
{
    /// <inheritdoc cref="ICoroutineService" />
    public sealed class CoroutineService : ICoroutineService
    {
        private CoroutineServiceMonoBehaviour? _serviceObject;

        public CoroutineService()
        {
            var gameObject = new GameObject(nameof(CoroutineServiceMonoBehaviour))
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            _serviceObject = gameObject.AddComponent<CoroutineServiceMonoBehaviour>();

            Object.DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Stops all coroutines and destroys the hosting game object.
        /// </summary>
        public void Dispose()
        {
            if (_serviceObject == null)
            {
                return;
            }

            _serviceObject.StopAllCoroutines();
            Object.Destroy(_serviceObject.gameObject);
            _serviceObject = null;
        }

        /// <inheritdoc />
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return Host().ExternalStartCoroutine(routine);
        }

        /// <inheritdoc />
        public IAsyncCoroutine StartAsyncCoroutine(IEnumerator routine)
        {
            var asyncCoroutine = new AsyncCoroutine(this);

            asyncCoroutine.SetCoroutine(Host().ExternalStartCoroutine(InternalCoroutine(routine, asyncCoroutine)));

            return asyncCoroutine;
        }

        /// <inheritdoc />
        public IAsyncCoroutine<T> StartAsyncCoroutine<T>(IEnumerator routine, T data)
        {
            var asyncCoroutine = new AsyncCoroutine<T>(this, data);

            asyncCoroutine.SetCoroutine(Host().ExternalStartCoroutine(InternalCoroutine(routine, asyncCoroutine)));

            return asyncCoroutine;
        }

        /// <inheritdoc />
        public IAsyncCoroutine StartDelayCall(Action call, float delay)
        {
            var asyncCoroutine = new AsyncCoroutine(this);

            asyncCoroutine.OnComplete(call);
            asyncCoroutine.SetCoroutine(Host().ExternalStartCoroutine(InternalDelayCoroutine(delay, asyncCoroutine)));

            return asyncCoroutine;
        }

        /// <inheritdoc />
        public IAsyncCoroutine<T> StartDelayCall<T>(Action<T> call, T data, float delay)
        {
            var asyncCoroutine = new AsyncCoroutine<T>(this, data);

            asyncCoroutine.OnComplete(call);
            asyncCoroutine.SetCoroutine(Host().ExternalStartCoroutine(InternalDelayCoroutine(delay, asyncCoroutine)));

            return asyncCoroutine;
        }

        /// <inheritdoc />
        public void StopCoroutine(Coroutine? coroutine)
        {
            if (coroutine == null || _serviceObject == null)
            {
                return;
            }

            _serviceObject.ExternalStopCoroutine(coroutine);
        }

        /// <inheritdoc />
        public void StopAllCoroutines()
        {
            if (_serviceObject == null)
            {
                return;
            }

            _serviceObject.StopAllCoroutines();
        }

        private CoroutineServiceMonoBehaviour Host()
        {
            return _serviceObject
                ?? throw new ObjectDisposedException(nameof(CoroutineService),
                    "The coroutine service has been disposed and can no longer start coroutines.");
        }

        private static IEnumerator InternalCoroutine(IEnumerator routine, ICompleteCoroutine completed)
        {
            yield return routine;

            completed.Completed();
        }

        private static IEnumerator InternalDelayCoroutine(float delayInSeconds, ICompleteCoroutine completed)
        {
            yield return new WaitForSeconds(delayInSeconds);

            completed.Completed();
        }

        private interface ICompleteCoroutine
        {
            void Completed();
        }

        private class AsyncCoroutine : IAsyncCoroutine, ICompleteCoroutine
        {
            private readonly ICoroutineService _coroutineService;

            private Action? _onComplete;

            public bool IsRunning => Coroutine != null && !IsCompleted;
            public bool IsCompleted { get; private set; }
            public Coroutine? Coroutine { get; private set; }
            public float StartTime { get; } = Time.time;

            public AsyncCoroutine(ICoroutineService coroutineService)
            {
                _coroutineService = coroutineService;
            }

            public void SetCoroutine(Coroutine coroutine)
            {
                Coroutine = coroutine;
            }

            public void OnComplete(Action onComplete)
            {
                _onComplete = onComplete;
            }

            public void StopCoroutine(bool triggerOnComplete = false)
            {
                _coroutineService.StopCoroutine(Coroutine);
                Coroutine = null;

                if (triggerOnComplete)
                {
                    Completed();
                }
            }

            public void Completed()
            {
                if (IsCompleted)
                {
                    return;
                }

                IsCompleted = true;
                Coroutine = null;

                OnCompleteTrigger();
            }

            protected virtual void OnCompleteTrigger()
            {
                _onComplete?.Invoke();
            }
        }

        private sealed class AsyncCoroutine<T> : AsyncCoroutine, IAsyncCoroutine<T>
        {
            private Action<T>? _onComplete;

            public T Data { get; set; }

            public AsyncCoroutine(ICoroutineService coroutineService, T data) : base(coroutineService)
            {
                Data = data;
            }

            public void OnComplete(Action<T> onComplete)
            {
                _onComplete = onComplete;
            }

            protected override void OnCompleteTrigger()
            {
                base.OnCompleteTrigger();
                _onComplete?.Invoke(Data);
            }
        }
    }
}
