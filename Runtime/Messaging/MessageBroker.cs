#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Juahn.V2.Services
{
    /// <inheritdoc cref="IMessageBroker{T}" />
    public sealed class MessageBroker<T> : IMessageBroker<T> where T : IMessage
    {
        private const int MaxPublishDepth = 10;

        // Subscriber -> action. The subscriber key is held as a weak reference so that broker
        // registration alone does not keep dead subscribers alive.
        private readonly Dictionary<object, Entry> _subscriptions = new Dictionary<object, Entry>();

        private int _publishDepth;
        private bool _isPublishing;

        /// <inheritdoc />
        public void Publish(T message)
        {
            if (_publishDepth >= MaxPublishDepth)
            {
                LogDepthExceeded(nameof(Publish));
                return;
            }

            CleanupDeadSubscribers();

            if (_subscriptions.Count == 0)
            {
                return;
            }

            _publishDepth++;
            _isPublishing = true;

            try
            {
                foreach (var entry in _subscriptions.Values)
                {
                    if (entry.TryGetAction(out var action))
                    {
                        action(message);
                    }
                }
            }
            finally
            {
                _publishDepth--;
                if (_publishDepth == 0)
                {
                    _isPublishing = false;
                }
            }
        }

        /// <inheritdoc />
        public void PublishSafe(T message)
        {
            if (_publishDepth >= MaxPublishDepth)
            {
                LogDepthExceeded(nameof(PublishSafe));
                return;
            }

            CleanupDeadSubscribers();

            if (_subscriptions.Count == 0)
            {
                return;
            }

            _publishDepth++;

            try
            {
                var snapshot = new Entry[_subscriptions.Count];
                _subscriptions.Values.CopyTo(snapshot, 0);

                for (var i = 0; i < snapshot.Length; i++)
                {
                    if (snapshot[i].TryGetAction(out var action))
                    {
                        action(message);
                    }
                }
            }
            finally
            {
                _publishDepth--;
            }
        }

        /// <inheritdoc />
        public void Subscribe(Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), $"Cannot subscribe a null action to {typeof(T).Name}.");
            }

            var subscriber = action.Target;

            if (subscriber == null)
            {
                throw new ArgumentException(
                    $"Subscribing a static function to {typeof(T).Name} is not supported. " +
                    "Subscribe an instance method so the broker can track its owner.");
            }

            if (_isPublishing)
            {
                throw new InvalidOperationException(
                    $"Cannot subscribe to {typeof(T).Name} while it is being published. " +
                    $"Use {nameof(PublishSafe)} instead of {nameof(Publish)} when subscribers change during publishing.");
            }

            _subscriptions[subscriber] = new Entry(subscriber, action);
        }

        /// <inheritdoc />
        public void Unsubscribe(object? subscriber = null)
        {
            if (subscriber == null)
            {
                _subscriptions.Clear();
                return;
            }

            if (_isPublishing)
            {
                throw new InvalidOperationException(
                    $"Cannot unsubscribe from {typeof(T).Name} while it is being published. " +
                    $"Use {nameof(PublishSafe)} instead of {nameof(Publish)} when subscribers change during publishing.");
            }

            _subscriptions.Remove(subscriber);
        }

        private void CleanupDeadSubscribers()
        {
            if (_subscriptions.Count == 0)
            {
                return;
            }

            List<object>? dead = null;

            foreach (var pair in _subscriptions)
            {
                if (!pair.Value.IsAlive)
                {
                    dead ??= new List<object>();
                    dead.Add(pair.Key);
                }
            }

            if (dead == null)
            {
                return;
            }

            for (var i = 0; i < dead.Count; i++)
            {
                _subscriptions.Remove(dead[i]);
            }
        }

        private static void LogDepthExceeded(string method)
        {
            Debug.LogError(
                $"[MessageBroker] {method} depth exceeded ({MaxPublishDepth}): {typeof(T).Name} " +
                "- circular message chain detected. Breaking recursion.");
        }

        /// <summary>
        /// Holds an action plus a weak reference to its owner so dead subscribers can be pruned.
        /// </summary>
        private readonly struct Entry
        {
            private readonly WeakReference _subscriber;
            private readonly Action<T> _action;

            public Entry(object subscriber, Action<T> action)
            {
                _subscriber = new WeakReference(subscriber);
                _action = action;
            }

            public bool IsAlive => _subscriber.IsAlive;

            public bool TryGetAction(out Action<T> action)
            {
                if (_subscriber.IsAlive)
                {
                    action = _action;
                    return true;
                }

                action = null!;
                return false;
            }
        }
    }
}
