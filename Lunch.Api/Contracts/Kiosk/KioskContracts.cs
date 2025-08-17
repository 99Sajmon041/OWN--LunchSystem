using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Lunch.Api.Contracts.Kiosk
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KioskOutcome
    {
        Ok,
        UserNotFound,
        NoPaidMeal,
        AlreadyConsumed,
        OutOfTimeWindow,
        InvalidRequest
    }

    public sealed record KioskConsumeRequest
    {
        [Required, StringLength(64)]
        public string CardId { get; init; } = default!;


        [Required, StringLength(64)]
        public string KioskId { get; init; } = default!;

        [Required]
        public DateTime TimestampUtc { get; init; } = default!;


        [StringLength(64)]
        public string? IdempotencyKey { get; init; }
    }

    public sealed record SimpleUser(
        string Id,
        string Display,
        string? Email);

    public sealed record SimpleMeal(
        int DailyOfferId,
        string Name,
        decimal Price);

    public sealed record KioskConsumeResponse(
        KioskOutcome Outcome,
        string Message,
        SimpleUser? User,
        SimpleMeal? Meal,
        int? WrittenOffId,
        DateTime ServerTimeUtc);
}
