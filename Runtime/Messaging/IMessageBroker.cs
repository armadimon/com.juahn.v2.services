#nullable enable
using System;

namespace Juahn.V2.Services
{
    /// <summary>
    /// A strongly typed message broker for a single message type <typeparamref name="T"/>.
    /// Provides a decoupled channel of communication by dispatching a message event to all its subscribers.
    /// </summary>
    /// <remarks>
    /// Follows the "Message Broker Pattern". Because the broker is generic per message type there is no
    /// runtime type dictionary lookup and no boxing of the message contract.
    /// </remarks>
    public interface IMessageBroker<T> where T : IMessage
    {
        /// <summary>
        /// Publishes a <paramref name="message"/> to all current subscribers.
        /// If nothing is subscribed nothing happens.
        /// </summary>
        /// <remarks>
        /// This iterates the live subscriber collection. Use <see cref="PublishSafe"/> when a subscriber
        /// may subscribe or unsubscribe during the publish (chained publishing).
        /// </remarks>
        void Publish(T message);

        /// <summary>
        /// Publishes a <paramref name="message"/> over a copied snapshot of the subscribers, which allows
        /// subscribers to subscribe or unsubscribe during the publish. A circular publish depth guard breaks
        /// runaway recursion.
        /// </summary>
        void PublishSafe(T message);

        /// <summary>
        /// Subscribes <paramref name="action"/> to receive published messages.
        /// A single subscriber object can only hold one action per broker; re-subscribing replaces it.
        /// </summary>
        void Subscribe(Action<T> action);

        /// <summary>
        /// Unsubscribes the action registered by <paramref name="subscriber"/>.
        /// If <paramref name="subscriber"/> is <c>null</c>, all subscribers are removed.
        /// </summary>
        void Unsubscribe(object? subscriber = null);
    }
}
