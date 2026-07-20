#nullable enable
using System;
using UnityEngine;

namespace Juahn.V2.Services
{
    /// <inheritdoc cref="ITimeManipulator" />
    public sealed class TimeService : ITimeManipulator
    {
        private static readonly DateTime UnixInitialTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private float _initialUnityTime;
        private float _extraTime;
        private DateTime _initialTime;

        public TimeService()
        {
            _initialUnityTime = Time.unscaledTime;
            _initialTime = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public DateTime DateTimeUtcNow =>
            _initialTime
                .AddSeconds(Time.unscaledTime - _initialUnityTime)
                .AddSeconds(_extraTime);

        /// <inheritdoc />
        public float UnityTimeNow => Time.unscaledTime + _extraTime;

        /// <inheritdoc />
        public float UnityScaleTimeNow => Time.time + _extraTime;

        /// <inheritdoc />
        public long UnixTimeNow => (long)(DateTimeUtcNow - UnixInitialTime).TotalMilliseconds;

        /// <inheritdoc />
        public long UnixTimeFromDateTimeUtc(DateTime time)
        {
            return (long)(time.ToUniversalTime() - UnixInitialTime).TotalMilliseconds;
        }

        /// <inheritdoc />
        public long UnixTimeFromUnityTime(float time)
        {
            return UnixTimeFromDateTimeUtc(DateTimeUtcFromUnityTime(time));
        }

        /// <inheritdoc />
        public DateTime DateTimeUtcFromUnixTime(long time)
        {
            return UnixInitialTime.AddMilliseconds(time);
        }

        /// <inheritdoc />
        public DateTime DateTimeUtcFromUnityTime(float time)
        {
            return _initialTime.AddSeconds(time - _initialUnityTime);
        }

        /// <inheritdoc />
        public float UnityTimeFromDateTimeUtc(DateTime time)
        {
            return (float)(time.ToUniversalTime() - _initialTime).TotalSeconds + _initialUnityTime;
        }

        /// <inheritdoc />
        public float UnityTimeFromUnixTime(long time)
        {
            return UnityTimeFromDateTimeUtc(DateTimeUtcFromUnixTime(time));
        }

        /// <inheritdoc />
        public void AddTime(float timeInSeconds)
        {
            _extraTime += timeInSeconds;
        }

        /// <inheritdoc />
        public void SetInitialTime(DateTime initialTime)
        {
            _initialTime = initialTime.ToUniversalTime();
            _initialUnityTime = Time.unscaledTime;
            _extraTime = 0f;
        }
    }
}
