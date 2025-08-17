using Lunch.Api.Application.Kiosk;
using Lunch.Api.Contracts.Kiosk;
using Microsoft.AspNetCore.Mvc;

namespace Lunch.Api.Controllers
{
    [Route("api/v1/kiosk")]
    [ApiController]
    public sealed class KioskController : ControllerBase
    {
        private readonly IKioskService _svc;
        public KioskController(IKioskService svc) => _svc = svc;

        [HttpPost("consume")]
        public async Task<ActionResult<KioskConsumeResponse>> Consume([FromBody] KioskConsumeRequest request, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resp = await _svc.ConsumeAsync(request, ct);
            return Ok(resp);
        }

        [HttpGet("ping")]
        public ActionResult<string> Ping() => Ok("pong");
    }
}
