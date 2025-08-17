namespace Lunch.Shared.Policies
{
    public static class WeekEditPolicy
    {
        private static readonly DateOnly _launchDate = new DateOnly(2025, 6, 30);
        private static readonly DateOnly _launchWeekStart = NormalizeToMonday(_launchDate);
        public static DateOnly GetLaunchWeekStart() => _launchWeekStart;
        public static bool CanEditWeek(DateOnly startOfWeek, DateOnly today)
        {
            var current = NormalizeToMonday(today);
            var firstEditable = current.AddDays(14);
            return startOfWeek >= firstEditable;
        }

        public static DateOnly GetLockedWeekStart(DateOnly today)
        {
            var current = NormalizeToMonday(today);
            return current.AddDays(7);
        }

        public static DateOnly GetFirstEditableWeekStart(DateOnly today)
        {
            var current = NormalizeToMonday(today);
            return current.AddDays(14);
        }

        public static DateOnly NormalizeToMonday(DateOnly date)
        {
            int offset = (int)date.DayOfWeek - (int)DayOfWeek.Monday;
            if (offset < 0) offset += 7;
            return date.AddDays(-offset);
        }

        public static DateOnly GetMondayOfWeek(DateOnly date)
        {
            int difference = ((int)date.DayOfWeek + 6) % 7;
            return date.AddDays(-difference);
        }

        public static DateOnly GetTodayInPrague()
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Prague"); // Linux/macOS
                return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz).Date);
            }
            catch (TimeZoneNotFoundException)
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time"); // Windows
                return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz).Date);
            }
        }
    }
}
