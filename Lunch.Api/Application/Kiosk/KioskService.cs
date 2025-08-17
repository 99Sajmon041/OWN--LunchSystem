using Lunch.Api.Contracts.Kiosk;
using Lunch.Api.Infrastructure.Idempotency;
using Lunch.Domain.Models;
using Lunch.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Lunch.Api.Application.Kiosk
{
    public sealed class KioskService : IKioskService
    {
        private readonly ApplicationDbContext _db;
        private readonly IIdempotencyStore _idem;
        private readonly TimeProvider _time;
        private readonly KioskServiceOptions _opt;
        private readonly ILogger<KioskService> _log;

        public KioskService(
            ApplicationDbContext db,
            IIdempotencyStore idem,
            TimeProvider time,
            IOptions<KioskServiceOptions> opt,
            ILogger<KioskService> log)
        {
            _db = db;
            _idem = idem;
            _time = time;
            _opt = opt.Value;
            _log = log;
        }

        public async Task<KioskConsumeResponse> ConsumeAsync(KioskConsumeRequest req, CancellationToken ct = default)
        {
            var nowUtc = _time.GetUtcNow().UtcDateTime;
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, TimeZoneInfo.Local); // spolehlivé i mimo .NET 8
            var serverTime = nowUtc;

            var user = await _db.Set<Customer>()
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.CardId == req.CardId, ct);

            if (user is null)
            {
                _log.LogWarning("Kiosk: user not found. Card:{Card} Kiosk:{Kiosk}", req.CardId, req.KioskId);
                return await SaveIdemAsync(req, new(
                    KioskOutcome.UserNotFound,
                    "Uživatel neexistuje.",
                    null, null, null, serverTime), ct);
            }

            if (!string.IsNullOrWhiteSpace(req.IdempotencyKey) && await _idem.ExistsAsync(req.IdempotencyKey!, ct))
            {
                _log.LogInformation("Kiosk: idempotent replay. Card:{Card} Kiosk:{Kiosk}", req.CardId, req.KioskId);
                return new(
                    KioskOutcome.AlreadyConsumed,
                    "Uživatel už dnes pípnul (idempotentní opakování).",
                    new(user.Id, user.FullName, user.Email),
                    null, null, serverTime);
            }

            var nowTod = localNow.TimeOfDay;
            if (!InWindow(nowTod, _opt.StartLocal, _opt.EndLocal))
            {
                var msg = $"Pípání je povoleno mezi {_opt.StartLocal:hh\\:mm}-{_opt.EndLocal:hh\\:mm}. Nyní je {localNow:HH\\:mm}.";
                _log.LogWarning("Kiosk: out of time window. Card:{Card} Kiosk:{Kiosk} Local:{Local}", req.CardId, req.KioskId, localNow);
                return await SaveIdemAsync(req, new(
                    KioskOutcome.OutOfTimeWindow,
                    msg,
                    null, null, null, serverTime), ct);
            }

            var todayLocalDate = localNow.Date;
            var paid = await _db.Set<PaidFood>()
                .Include(p => p.DailyOffer)
                .ThenInclude(o => o.Meal)
                .AsNoTracking()
                .SingleOrDefaultAsync(p =>
                    p.CustomerId == user.Id &&
                    p.DailyOffer.Date == DateOnly.FromDateTime(todayLocalDate),
                    ct);

            if (paid is null)
            {
                _log.LogInformation("Kiosk: no paid meal for today. User:{UserId} ({Email})", user.Id, user.Email);
                return await SaveIdemAsync(req, new(
                    KioskOutcome.NoPaidMeal,
                    "Dnes nemá zaplacený oběd.",
                    new(user.Id, user.FullName, user.Email),
                    null, null, serverTime), ct);
            }

            var startLocal = todayLocalDate; 
            var endLocal = todayLocalDate.AddDays(1);
            var startUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(startLocal, DateTimeKind.Local));
            var endUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(endLocal, DateTimeKind.Local));

            var already = await _db.Set<WrittenOffFood>()
                .AsNoTracking()
                .AnyAsync(w =>
                    w.CustomerId == user.Id &&
                    w.TimeOfCreate >= startUtc &&
                    w.TimeOfCreate < endUtc, ct);

            if (already)
            {
                _log.LogInformation("Kiosk: already consumed today. User:{UserId} ({Email})", user.Id, user.Email);
                return await SaveIdemAsync(req, new(
                    KioskOutcome.AlreadyConsumed,
                    "Uživatel už dnes pípnul (oběd byl odepsán).",
                    new(user.Id, user.FullName, user.Email),
                    paid.DailyOffer is null ? null : new(paid.DailyOffer.Id, paid.DailyOffer.Meal.FoodName, paid.FinalPaidPrice),
                    null,
                    serverTime), ct);
            }

            var priceAfterDiscount = paid.FinalPaidPrice;
            var w = new WrittenOffFood
            {
                CustomerId = user.Id,
                DailyOfferId = paid.DailyOfferId,
                TimeOfCreate = nowUtc,           // vždy ukládat v UTC
                PriceAfterDiscount = priceAfterDiscount,
                KioskId = req.KioskId
            };

            _db.Set<WrittenOffFood>().Add(w);
            await _db.SaveChangesAsync(ct);

            _log.LogInformation("Kiosk: consumed OK. User:{UserId} ({Email}) Kiosk:{Kiosk} WrittenOffId:{WId}",
                user.Id, user.Email, req.KioskId, w.Id);

            var resp = new KioskConsumeResponse(
                KioskOutcome.Ok,
                $"Uživatel {user.Email} má dnes {paid.DailyOffer?.Meal.FoodName} ({priceAfterDiscount:#,0.##} Kč).",
                new(user.Id, user.FullName, user.Email),
                paid.DailyOffer is null ? null :
                    new(paid.DailyOffer.Id, paid.DailyOffer.Meal.FoodName, priceAfterDiscount),
                w.Id,
                serverTime);

            return await SaveIdemAsync(req, resp, ct);
        }

        private async Task<KioskConsumeResponse> SaveIdemAsync(KioskConsumeRequest req, KioskConsumeResponse resp, CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(req.IdempotencyKey))
                await _idem.SetAsync(req.IdempotencyKey!, resp, _opt.IdempotencyTtl, ct);
            return resp;
        }

        private static bool InWindow(TimeSpan now, TimeSpan start, TimeSpan end)
            => start <= end ? (now >= start && now <= end)
                            : (now >= start || now <= end);
    }
}
