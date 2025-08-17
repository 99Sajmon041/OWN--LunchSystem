using System.Text.Json.Serialization;

namespace Lunch.Kiosk.Models
{
    public sealed class KioskConsumeResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
