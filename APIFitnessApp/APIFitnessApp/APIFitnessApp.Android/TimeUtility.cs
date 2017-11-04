using System;

namespace APIFitnessApp.Droid
{
    class TimeUtility
    {
        private const string Tag = "TimeUtility";

        // Start of POSIX time
        private static readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // A week in milliseconds
        private const long WeekInMillis = 1000 * 60 * 60 * 24 * 7;

        // 48 hours in milliseconds
        private const long TwoDaysInMillis = 1000 * 60 * 60 * 24 * 2;

        // Current POSIX time
        public static long CurrentMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        // A week ago in POSIX time
        public static long WeekAgoMillis()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds - WeekInMillis;
        }

        // 48 hours ago in POSIX time
        public static long TwoDaysAgoMillis()
        {
            // We add a 1 to ensure only two buckets of 24 hours apiece are created
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds - TwoDaysInMillis + 1;
        }

        // Converts POSIX time to DateTime
        public static DateTime FromMillis(long millis)
        {
            return UnixEpoch.AddMilliseconds(millis);
        }

        // Calculate how many days in the past a day is from today
        public static uint DaysInPast(DateTime dateTime)
        {
            return (uint)(DateTime.Now.Date - dateTime.Date).Days;
        }
    }
}