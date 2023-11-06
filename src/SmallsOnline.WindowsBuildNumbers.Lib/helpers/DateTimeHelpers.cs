namespace SmallsOnline.WindowsBuildNumbers.Lib.Helpers;

/// <summary>
/// Helper methods for working with date and time inputs.
/// </summary>
public static class DateTimeHelpers
{
    /// <summary>
    /// Get whether the input date is the second Tuesday of the month.
    /// </summary>
    /// <param name="inputDate">A date/time value.</param>
    /// <returns>Whether the input date is the second Tuesday of the month.</returns>
    public static bool IsSecondTuesdayOfTheMonth(DateTimeOffset inputDate)
    {
#if NETSTANDARD2_0
        DateTimeOffset inputDateOnly = new(inputDate.Year, inputDate.Month, inputDate.Day, 0, 0, 0, 0, inputDate.Offset);
#else
        DateOnly inputDateOnly = ConvertToDateOnly(inputDate);
#endif

        return inputDateOnly == GetSecondTuesdayOfTheMonth(inputDate);
    }

#if NETSTANDARD2_0
    /// <summary>
    /// Get the second Tuesday of a month.
    /// </summary>
    /// <param name="inputDate">The input date to get.</param>
    /// <returns>The second Tuesday of the month.</returns>
    public static DateTimeOffset GetSecondTuesdayOfTheMonth(DateTimeOffset inputDate)
    {
        // Create a new DateTimeOffset for the first of the month from the input date.
        DateTimeOffset firstDateOfMonth = new(inputDate.Year, inputDate.Month, 1, 0, 0, 0, 0, inputDate.Offset);

        // Generate a DateTimeOffset for the second Tuesday of the month.
        // This is done by determining if the first date of the month is a Tuesday or not.
        DateTimeOffset firstTuesdayOfMonth = (firstDateOfMonth.DayOfWeek == DayOfWeek.Tuesday) switch
        {
            // If the first date of the month is not a Tuesday,
            // subtract the DayOfWeek value for Tuesday from the DayOfWeek value for the first date of the month.
            // Then use that value to add/subtract days (Depending on the value).
            false => firstDateOfMonth.AddDays(DayOfWeek.Tuesday - firstDateOfMonth.DayOfWeek),

            // If the first date of the month is a Tuesday,
            // then set the first Tuesday of the month to the first date of the month.
            _ => firstDateOfMonth
        };

        // Generate a DateTimeOffset for the second Tuesday of the month.
        // This is done by determining if the first Tuesday of the month is less than the first date of the month.
        DateTimeOffset secondTuesdayOfMonth = (firstTuesdayOfMonth.Month < firstDateOfMonth.Month) switch
        {
            // If the first Tuesday of the month is not less than the first date of the month,
            // then add 7 days to the first Tuesday of the month.
            false => firstTuesdayOfMonth.AddDays(7),

            // If the first Tuesday of the month is less than the first date of the month,
            // then add 14 days to the first Tuesday of the month.
            _ => firstTuesdayOfMonth.AddDays(14)
        };

        return secondTuesdayOfMonth;
    }
#else
    /// <summary>
    /// Get the second Tuesday of a month.
    /// </summary>
    /// <param name="inputDate">The input date to get.</param>
    /// <returns>The second Tuesday of the month.</returns>
    public static DateOnly GetSecondTuesdayOfTheMonth(DateTimeOffset inputDate)
    {
        // Create a new DateTimeOffset for the first of the month from the input date.
        DateOnly firstDateOfMonth = new(inputDate.Year, inputDate.Month, 1);

        // Generate a DateTimeOffset for the second Tuesday of the month.
        // This is done by determining if the first date of the month is a Tuesday or not.
        DateOnly firstTuesdayOfMonth = (firstDateOfMonth.DayOfWeek == DayOfWeek.Tuesday) switch
        {
            // If the first date of the month is not a Tuesday,
            // subtract the DayOfWeek value for Tuesday from the DayOfWeek value for the first date of the month.
            // Then use that value to add/subtract days (Depending on the value).
            false => firstDateOfMonth.AddDays(DayOfWeek.Tuesday - firstDateOfMonth.DayOfWeek),
            
            // If the first date of the month is a Tuesday,
            // then set the first Tuesday of the month to the first date of the month.
            _ => firstDateOfMonth
        };

        // Generate a DateTimeOffset for the second Tuesday of the month.
        // This is done by determining if the first Tuesday of the month is less than the first date of the month.
        DateOnly secondTuesdayOfMonth = (firstTuesdayOfMonth.Month < firstDateOfMonth.Month) switch
        {
            // If the first Tuesday of the month is not less than the first date of the month,
            // then add 7 days to the first Tuesday of the month.
            false => firstTuesdayOfMonth.AddDays(7),

            // If the first Tuesday of the month is less than the first date of the month,
            // then add 14 days to the first Tuesday of the month.
            _ => firstTuesdayOfMonth.AddDays(14)
        };

        return secondTuesdayOfMonth;
    }
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// Convert a <see cref="DateTimeOffset"/> input to a <see cref="DateOnly"/> value.
    /// </summary>
    /// <param name="inputDate">A date/time value.</param>
    /// <returns>A <see cref="DateOnly"/> output of the input date.</returns>
    private static DateOnly ConvertToDateOnly(DateTimeOffset inputDate)
    {
        return new(inputDate.Year, inputDate.Month, inputDate.Day);
    }
#endif
}