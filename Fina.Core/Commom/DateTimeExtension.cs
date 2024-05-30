﻿namespace Fina.Core.Commom
{
    public static class DateTimeExtension
    {
        public static DateTime GetFirstDay(
            this DateTime date, int? month = null, int? year = null)
        {
            return new DateTime(year ?? date.Year, month ?? date.Month, 1);
        }
        public static DateTime GetLastDay(
            this DateTime date, int? month = null, int? year = null)
        {
            return new DateTime(year ?? date.Year, month ?? date.Month, 1)
                .AddMonths(1)
                .AddDays(-1);
        }
    }
}
