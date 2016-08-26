using System;

namespace SccmTools.Library.Common.Caching
{
    public class CurrentDateTimeService : ICurrentDateTimeService
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public DateTime GetCurrentDateTimeUtc()
        {
            return DateTime.UtcNow;
        }
    }
}