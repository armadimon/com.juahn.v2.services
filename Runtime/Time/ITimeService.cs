#nullable enable
using System;

namespace Juahn.V2.Services
{
    /// <summary>
    /// Provides time relative to the game's simulation clock. The clock abstracts scaled versus unscaled
    /// Unity time and can be manipulated through <see cref="ITimeManipulator"/> (for example to fast-forward
    /// the simulation or to synchronize with a server).
    /// </summary>
    public interface ITimeService
    {
        /// <summary>The current Gregorian UTC time relative to the game.</summary>
        DateTime DateTimeUtcNow { get; }

        /// <summary>Unscaled Unity time since startup, including any manipulation.</summary>
        float UnityTimeNow { get; }

        /// <summary>Scaled Unity time (affected by <c>Time.timeScale</c>), including any manipulation.</summary>
        float UnityScaleTimeNow { get; }

        /// <summary>The current Unix time in milliseconds.</summary>
        long UnixTimeNow { get; }

        /// <summary>Converts a Gregorian UTC <paramref name="time"/> to Unix time in milliseconds.</summary>
        long UnixTimeFromDateTimeUtc(DateTime time);

        /// <summary>Converts a Unity <paramref name="time"/> to Unix time in milliseconds.</summary>
        long UnixTimeFromUnityTime(float time);

        /// <summary>Converts a Unix <paramref name="time"/> in milliseconds to Gregorian UTC.</summary>
        DateTime DateTimeUtcFromUnixTime(long time);

        /// <summary>Converts a Unity <paramref name="time"/> to Gregorian UTC.</summary>
        DateTime DateTimeUtcFromUnityTime(float time);

        /// <summary>Converts a Gregorian UTC <paramref name="time"/> to Unity time.</summary>
        float UnityTimeFromDateTimeUtc(DateTime time);

        /// <summary>Converts a Unix <paramref name="time"/> in milliseconds to Unity time.</summary>
        float UnityTimeFromUnixTime(long time);
    }
}
