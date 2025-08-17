using Lunch.Api.Contracts.Kiosk;

namespace Lunch.Api.Application.Kiosk
{
    public interface IKioskService
    {
        Task<KioskConsumeResponse> ConsumeAsync(KioskConsumeRequest req, CancellationToken ct = default);
    }
}
