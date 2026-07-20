#nullable enable
using System;

namespace Juahn.V2.Services
{
    /// <summary>
    /// Provides updatable callbacks (Update, LateUpdate, FixedUpdate) to plain C# objects without each
    /// object needing its own MonoBehaviour. All callbacks are driven from a single hidden
    /// DontDestroyOnLoad game object, which survives scene loads.
    /// Call <see cref="IDisposable.Dispose"/> to tear the service down.
    /// </summary>
    public interface ITickService : IDisposable
    {
        /// <summary>
        /// Subscribes <paramref name="action"/> to the frame update with an optional <paramref name="deltaTime"/>
        /// throttle. When <paramref name="realTime"/> is true the service uses unscaled time so the callback runs
        /// even while the game is paused (timeScale 0). <paramref name="timeOverflowToNextTick"/> keeps the tick
        /// cadence stable when frames overshoot the interval.
        /// </summary>
        void SubscribeOnUpdate(Action<float> action, float deltaTime = 0f, bool timeOverflowToNextTick = false, bool realTime = false);

        /// <summary>
        /// Subscribes <paramref name="action"/> to the late update. See <see cref="SubscribeOnUpdate"/> for the
        /// meaning of the parameters.
        /// </summary>
        void SubscribeOnLateUpdate(Action<float> action, float deltaTime = 0f, bool timeOverflowToNextTick = false, bool realTime = false);

        /// <summary>
        /// Subscribes <paramref name="action"/> to the fixed update. The callback receives the fixed delta time.
        /// </summary>
        void SubscribeOnFixedUpdate(Action<float> action);

        /// <summary>Unsubscribes <paramref name="action"/> from every update phase.</summary>
        void Unsubscribe(Action<float> action);

        /// <summary>Unsubscribes <paramref name="action"/> from the frame update.</summary>
        void UnsubscribeOnUpdate(Action<float> action);

        /// <summary>Unsubscribes <paramref name="action"/> from the fixed update.</summary>
        void UnsubscribeOnFixedUpdate(Action<float> action);

        /// <summary>Unsubscribes <paramref name="action"/> from the late update.</summary>
        void UnsubscribeOnLateUpdate(Action<float> action);

        /// <summary>Unsubscribes all frame update callbacks, optionally scoped to a single <paramref name="subscriber"/>.</summary>
        void UnsubscribeAllOnUpdate(object? subscriber = null);

        /// <summary>Unsubscribes all fixed update callbacks, optionally scoped to a single <paramref name="subscriber"/>.</summary>
        void UnsubscribeAllOnFixedUpdate(object? subscriber = null);

        /// <summary>Unsubscribes all late update callbacks, optionally scoped to a single <paramref name="subscriber"/>.</summary>
        void UnsubscribeAllOnLateUpdate(object? subscriber = null);

        /// <summary>Unsubscribes from every update phase, optionally scoped to a single <paramref name="subscriber"/>.</summary>
        void UnsubscribeAll(object? subscriber = null);
    }
}
