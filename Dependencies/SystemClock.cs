using System;

namespace UrlShortnerApi.Dependencies
{
    public interface ISystemClock
    {
        Func<DateTime> Now { get; }
    }
    public class SystemClock : ISystemClock
    {
        public Func<DateTime> Now => GetCurrentDateTime;

        private static DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
    }
}