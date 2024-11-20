using System.Runtime.CompilerServices;

namespace Dima.Core.Common.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime GetFirstDay(this DateTime date, int? year = null, int? month = null)
            => new DateTime(year ?? date.Year, month ?? date.Month, 1,0,0,0,DateTimeKind.Utc);

        public static DateTime GetLastDay(this DateTime date, int? year = null, int? month = null)
            => new DateTime(year ?? date.Year, month ?? date.Month, 1,0,0,0,  DateTimeKind.Utc).AddMonths(1).AddDays(-1);
    }
}
