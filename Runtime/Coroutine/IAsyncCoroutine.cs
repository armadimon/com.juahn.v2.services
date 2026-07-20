#nullable enable
using System;
using UnityEngine;

namespace Juahn.V2.Services
{
    /// <summary>
    /// A handle to a coroutine running through the <see cref="ICoroutineService"/> that allows passively
    /// waiting for completion and registering an on-complete callback.
    /// </summary>
    public interface IAsyncCoroutine
    {
        /// <summary>Whether the coroutine is currently running.</summary>
        bool IsRunning { get; }

        /// <summary>Whether the coroutine has completed.</summary>
        bool IsCompleted { get; }

        /// <summary>The underlying Unity coroutine, or <c>null</c> after completion.</summary>
        Coroutine? Coroutine { get; }

        /// <summary>The Unity time the coroutine started.</summary>
        float StartTime { get; }

        /// <summary>Registers a callback invoked once the coroutine completes.</summary>
        void OnComplete(Action onComplete);

        /// <summary>
        /// Stops this coroutine. When <paramref name="triggerOnComplete"/> is true the on-complete
        /// callback is still invoked.
        /// </summary>
        void StopCoroutine(bool triggerOnComplete = false);
    }

    /// <inheritdoc />
    public interface IAsyncCoroutine<T> : IAsyncCoroutine
    {
        /// <summary>The data associated with the coroutine, delivered to the typed on-complete callback.</summary>
        T Data { get; set; }

        /// <summary>Registers a callback invoked with <see cref="Data"/> once the coroutine completes.</summary>
        void OnComplete(Action<T> onComplete);
    }
}
