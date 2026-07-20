#nullable enable
using System;

namespace Juahn.V2.Services
{
    /// <inheritdoc cref="ITimeService" />
    /// <remarks>
    /// Feeds and manipulates the simulation clock. Useful for speeding up the game or synchronizing the
    /// game's time with an external source.
    /// </remarks>
    public interface ITimeManipulator : ITimeService
    {
        /// <summary>
        /// Adds <paramref name="timeInSeconds"/> to the simulation clock. A positive value fast-forwards time,
        /// a negative value rewinds it.
        /// </summary>
        void AddTime(float timeInSeconds);

        /// <summary>
        /// Synchronizes the simulation clock's origin with <paramref name="initialTime"/> and resets the
        /// accumulated offset.
        /// </summary>
        void SetInitialTime(DateTime initialTime);
    }
}
