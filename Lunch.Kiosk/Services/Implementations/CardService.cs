using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using Lunch.Kiosk.Models;
using Lunch.Kiosk.Options;
using Lunch.Kiosk.Services.Interfaces;

public class CardService : ICardService
{
    private readonly HttpClient _http;
    private readonly string _kioskId;

    public CardService(HttpClient http, IOptions<KioskOptions> opt)
    {
        _http = http;
        _kioskId = opt.Value.KioskId ?? "";
    }

    public async Task<KioskConsumeResponse> CheckCardAsync(string cardId)
    {
        if (string.IsNullOrWhiteSpace(_kioskId))
            return new KioskConsumeResponse { Success = false, Message = "KioskId není nastaveno v appsettings.json." };

        try
        {
            var req = new { CardId = cardId, KioskId = _kioskId };
            using var resp = await _http.PostAsJsonAsync("api/v1/kiosk/consume", req);

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            if (resp.IsSuccessStatusCode)
            {
                var raw = await resp.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<KioskConsumeResponse>(raw, opts)
                          ?? new KioskConsumeResponse { Message = raw.Trim().Trim('"') };

                if (!dto.Success && !raw.Contains("\"success\"", StringComparison.OrdinalIgnoreCase))
                    dto.Success = true;

                return dto;
            }

            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var problem = await resp.Content.ReadFromJsonAsync<ValidationProblem>(opts);
                var msgs = problem?.Errors?.SelectMany(kv => kv.Value) ?? Array.Empty<string>();
                var joined = string.Join(" | ", msgs);
                return new KioskConsumeResponse { Success = false, Message = $"400: {joined}" };
            }

            var errTxt = await resp.Content.ReadAsStringAsync();
            return new KioskConsumeResponse { Success = false, Message = $"Chyba API: {(int)resp.StatusCode} {resp.ReasonPhrase} - {errTxt}" };
        }
        catch (TaskCanceledException)
        {
            return new KioskConsumeResponse { Success = false, Message = "Timeout – API nereaguje" };
        }
        catch (HttpRequestException ex)
        {
            return new KioskConsumeResponse { Success = false, Message = $"Síťová chyba: {ex.Message}" };
        }
        catch (Exception ex)
        {
            return new KioskConsumeResponse { Success = false, Message = $"Chyba klienta: {ex.Message}" };
        }
    }

    private sealed class ValidationProblem
    {
        public Dictionary<string, string[]>? Errors { get; set; }
    }

    public async Task<bool> PingAsync()
    {
        try
        {
            var s = await _http.GetStringAsync("api/v1/kiosk/ping");
            return s.Equals("pong", StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }
}
