
using Lunch.Kiosk.Models;

namespace Lunch.Kiosk.Services.Interfaces
{
    public interface ICardService
    {
        Task<KioskConsumeResponse> CheckCardAsync(string cardId);

        Task<bool> PingAsync();
    }
}
