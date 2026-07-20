#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Juahn.V2.Services
{
    /// <inheritdoc cref="ITickService" />
    public sealed class TickService : ITickService
    {
        private readonly TickServiceMonoBehaviour _tickObject;

        private readonly List<TickData> _onUpdateList = new List<TickData>();
        private readonly List<TickData> _onFixedUpdateList = new List<TickData>();
        private readonly List<TickData> _onLateUpdateList = new List<TickData>();

        private int _tickDataIdRef;

        public TickService()
        {
            var gameObject = new GameObject(nameof(TickServiceMonoBehaviour))
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            Object.DontDestroyOnLoad(gameObject);

            _tickObject = gameObject.AddComponent<TickServiceMonoBehaviour>();
            _tickObject.OnUpdateCallback = OnUpdate;
            _tickObject.OnFixedUpdateCallback = OnFixedUpdate;
            _tickObject.OnLateUpdateCallback = OnLateUpdate;
        }

        /// <summary>
        /// Stops all ticks and destroys the driving game object.
        /// </summary>
        public void Dispose()
        {
            if (_tickObject != null)
            {
                Object.Destroy(_tickObject.gameObject);
            }

            _onUpdateList.Clear();
            _onFixedUpdateList.Clear();
            _onLateUpdateList.Clear();
        }

        /// <inheritdoc />
        public void SubscribeOnUpdate(Action<float> action, float deltaTime = 0f, bool timeOverflowToNextTick = false, bool realTime = false)
        {
            _onUpdateList.Add(CreateTickData(action, deltaTime, timeOverflowToNextTick, realTime));
        }

        /// <inheritdoc />
        public void SubscribeOnLateUpdate(Action<float> action, float deltaTime = 0f, bool timeOverflowToNextTick = false, bool realTime = false)
        {
            _onLateUpdateList.Add(CreateTickData(action, deltaTime, timeOverflowToNextTick, realTime));
        }

        /// <inheritdoc />
        public void SubscribeOnFixedUpdate(Action<float> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Cannot subscribe a null fixed update action.");
            }

            _onFixedUpdateList.Add(new TickData
            {
                Id = ++_tickDataIdRef,
                Action = action,
                Subscriber = action.Target
            });
        }

        /// <inheritdoc />
        public void Unsubscribe(Action<float> action)
        {
            UnsubscribeOnUpdate(action);
            UnsubscribeOnFixedUpdate(action);
            UnsubscribeOnLateUpdate(action);
        }

        /// <inheritdoc />
        public void UnsubscribeOnUpdate(Action<float> action)
        {
            RemoveAction(_onUpdateList, action);
        }

        /// <inheritdoc />
        public void UnsubscribeOnFixedUpdate(Action<float> action)
        {
            RemoveAction(_onFixedUpdateList, action);
        }

        /// <inheritdoc />
        public void UnsubscribeOnLateUpdate(Action<float> action)
        {
            RemoveAction(_onLateUpdateList, action);
        }

        /// <inheritdoc />
        public void UnsubscribeAllOnUpdate(object? subscriber = null)
        {
            RemoveAll(_onUpdateList, subscriber);
        }

        /// <inheritdoc />
        public void UnsubscribeAllOnFixedUpdate(object? subscriber = null)
        {
            RemoveAll(_onFixedUpdateList, subscriber);
        }

        /// <inheritdoc />
        public void UnsubscribeAllOnLateUpdate(object? subscriber = null)
        {
            RemoveAll(_onLateUpdateList, subscriber);
        }

        /// <inheritdoc />
        public void UnsubscribeAll(object? subscriber = null)
        {
            UnsubscribeAllOnUpdate(subscriber);
            UnsubscribeAllOnFixedUpdate(subscriber);
            UnsubscribeAllOnLateUpdate(subscriber);
        }

        private TickData CreateTickData(Action<float> action, float deltaTime, bool timeOverflowToNextTick, bool realTime)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Cannot subscribe a null update action.");
            }

            return new TickData
            {
                Id = ++_tickDataIdRef,
                Action = action,
                DeltaTime = deltaTime,
                TimeOverflowToNextTick = timeOverflowToNextTick,
                RealTime = realTime,
                LastTickTime = realTime ? Time.unscaledTime : Time.time,
                Subscriber = action.Target
            };
        }

        private static void RemoveAction(List<TickData> list, Action<float> action)
        {
            if (action == null)
            {
                return;
            }

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].Action == action && list[i].Subscriber == action.Target)
                {
                    list.RemoveAt(i);
                    return;
                }
            }
        }

        private static void RemoveAll(List<TickData> list, object? subscriber)
        {
            if (subscriber == null)
            {
                list.Clear();
                return;
            }

            list.RemoveAll(data => data.Subscriber == subscriber);
        }

        private void OnUpdate()
        {
            Tick(_onUpdateList);
        }

        private void OnLateUpdate()
        {
            Tick(_onLateUpdateList);
        }

        private void OnFixedUpdate()
        {
            if (_onFixedUpdateList.Count == 0)
            {
                return;
            }

            // Iterate backwards to allow safe mutation during iteration.
            for (var i = _onFixedUpdateList.Count - 1; i >= 0; i--)
            {
                if (i >= _onFixedUpdateList.Count)
                {
                    continue;
                }

                _onFixedUpdateList[i].Action(Time.fixedDeltaTime);
            }
        }

        private static void Tick(List<TickData> list)
        {
            if (list.Count == 0)
            {
                return;
            }

            // Iterate backwards to allow safe mutation during iteration.
            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (i >= list.Count)
                {
                    continue;
                }

                var tickData = list[i];
                var time = tickData.RealTime ? Time.unscaledTime : Time.time;

                if (time < tickData.LastTickTime + tickData.DeltaTime)
                {
                    continue;
                }

                var deltaTime = time - tickData.LastTickTime;

                tickData.Action(deltaTime);

                // Only write back if the entry still exists and was not unsubscribed by the action.
                if (i < list.Count && list[i] == tickData)
                {
                    var overflow = tickData.DeltaTime == 0f ? 0f : deltaTime % tickData.DeltaTime;
                    tickData.LastTickTime = tickData.TimeOverflowToNextTick ? time - overflow : time;
                    list[i] = tickData;
                }
            }
        }

        private struct TickData : IEquatable<TickData>
        {
            public int Id;
            public Action<float> Action;
            public float DeltaTime;
            public bool TimeOverflowToNextTick;
            public bool RealTime;
            public float LastTickTime;
            public object? Subscriber;

            public bool Equals(TickData other) => other.Id == Id;

            public override bool Equals(object? other) => other is TickData data && Equals(data);

            public override int GetHashCode() => Id;

            public static bool operator ==(TickData a, TickData b) => a.Id == b.Id;

            public static bool operator !=(TickData a, TickData b) => a.Id != b.Id;
        }
    }
}
