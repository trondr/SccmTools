using System;

namespace SccmTools.Library.Common.Caching
{
    public interface ICurrentDateTimeService
    {
        DateTime GetCurrentDateTime();
        DateTime GetCurrentDateTimeUtc();
    }
}