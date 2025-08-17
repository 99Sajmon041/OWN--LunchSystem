namespace Lunch.Api.Application.Kiosk
{
    public sealed class KioskServiceOptions
    {
        public TimeSpan StartLocal { get; set; } = TimeSpan.FromHours(11);
        public TimeSpan EndLocal { get; set; } = TimeSpan.FromHours(15);
        public TimeSpan IdempotencyTtl { get; set; } = TimeSpan.FromMinutes(2);
    }
}
