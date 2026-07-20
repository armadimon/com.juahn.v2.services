#nullable enable
using System;
using System.Collections;
using UnityEngine;

namespace Juahn.V2.Services
{
    /// <summary>
    /// Runs coroutines outside the scope of a specific game object, letting plain C# classes start and stop
    /// coroutines. Coroutines run on a hidden DontDestroyOnLoad game object and survive scene loads.
    /// Call <see cref="IDisposable.Dispose"/> to tear the service down.
    /// </summary>
    public interface ICoroutineService : IDisposable
    {
        /// <summary>Starts a coroutine and returns the Unity handle, like <see cref="MonoBehaviour.StartCoroutine(IEnumerator)"/>.</summary>
        Coroutine StartCoroutine(IEnumerator routine);

        /// <summary>Starts a coroutine and returns an <see cref="IAsyncCoroutine"/> with an on-complete callback.</summary>
        IAsyncCoroutine StartAsyncCoroutine(IEnumerator routine);

        /// <summary>Starts a coroutine carrying <paramref name="data"/> and returns an <see cref="IAsyncCoroutine{T}"/>.</summary>
        IAsyncCoroutine<T> StartAsyncCoroutine<T>(IEnumerator routine, T data);

        /// <summary>Invokes <paramref name="call"/> after <paramref name="delay"/> seconds.</summary>
        IAsyncCoroutine StartDelayCall(Action call, float delay);

        /// <summary>Invokes <paramref name="call"/> with <paramref name="data"/> after <paramref name="delay"/> seconds.</summary>
        IAsyncCoroutine<T> StartDelayCall<T>(Action<T> call, T data, float delay);

        /// <inheritdoc cref="MonoBehaviour.StopCoroutine(Coroutine)" />
        void StopCoroutine(Coroutine? coroutine);

        /// <inheritdoc cref="MonoBehaviour.StopAllCoroutines" />
        void StopAllCoroutines();
    }
}
