using System;
using System.Globalization;

namespace RKeeper.Core.Extensions;

public static class DateTimeExtensions
{
    public static DateTime FirstDayOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    public static DateTime LastDayOfMonth(this DateTime date)
    {
        return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
    }

    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }

    public static int WeeksInMonth(this DateTime date)
    {
        var firstDay = new DateTime(date.Year, date.Month, 1);
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

        switch (daysInMonth)
        {
            case >= 30 when firstDay.DayOfWeek == DayOfWeek.Saturday:
            case 31 when firstDay.DayOfWeek == DayOfWeek.Friday:
                return 6;
            case 28 when firstDay.DayOfWeek == DayOfWeek.Sunday:
                return 4;
            default:
                return 5;
        }
    }

    public static DateTime Max(DateTime d1, DateTime d2)
    {
        return d1 > d2 ? d1 : d2;
    }

    public static DateTime Min(DateTime d1, DateTime d2)
    {
        return d1 < d2 ? d1 : d2;
    }
}
