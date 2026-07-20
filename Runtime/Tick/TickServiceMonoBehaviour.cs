#nullable enable
using System;
using UnityEngine;

namespace Juahn.V2.Services
{
    /// <summary>
    /// The hidden MonoBehaviour that drives the <see cref="TickService"/>. It lives on a
    /// DontDestroyOnLoad game object and forwards Unity's update callbacks to the service.
    /// </summary>
    public sealed class TickServiceMonoBehaviour : MonoBehaviour
    {
        public Action? OnUpdateCallback;
        public Action? OnFixedUpdateCallback;
        public Action? OnLateUpdateCallback;

        private void Update()
        {
            OnUpdateCallback?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdateCallback?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdateCallback?.Invoke();
        }
    }
}
