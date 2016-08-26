using System;

namespace SccmTools.Library.Module.Common.Caching
{
    public interface ICurrentDateTimeService
    {
        DateTime GetCurrentDateTime();
        DateTime GetCurrentDateTimeUtc();
    }
}