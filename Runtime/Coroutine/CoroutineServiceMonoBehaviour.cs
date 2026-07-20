#nullable enable
using System.Collections;
using UnityEngine;

namespace Juahn.V2.Services
{
    /// <summary>
    /// The hidden MonoBehaviour that hosts every coroutine started through the <see cref="CoroutineService"/>.
    /// Lives on a DontDestroyOnLoad game object.
    /// </summary>
    public sealed class CoroutineServiceMonoBehaviour : MonoBehaviour
    {
        /// <inheritdoc cref="ICoroutineService.StartCoroutine(IEnumerator)" />
        public Coroutine ExternalStartCoroutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }

        /// <inheritdoc cref="ICoroutineService.StopCoroutine(Coroutine)" />
        public void ExternalStopCoroutine(Coroutine coroutine)
        {
            StopCoroutine(coroutine);
        }
    }
}
