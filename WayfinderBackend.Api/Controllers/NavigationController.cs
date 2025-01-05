using Microsoft.AspNetCore.Mvc;
using WayfinderBackend.Api.Models;
using WayfinderBackend.Api.Services;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace WayfinderBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NavigationController : ControllerBase
    {
        private readonly INavigationService _navigationService;
        private readonly IConfiguration _configuration;

        public NavigationController(INavigationService navigationService, IConfiguration configuration)
        {
            _navigationService = navigationService;
            _configuration = configuration;
        }

        [HttpPost("create-route")]
        public async Task<ActionResult<NavigationSession>> CreateRoute([FromBody] CreateRouteRequest request)
        {
            if (request.StartPoint == null || request.EndPoint == null || request.Steps == null)
            {
                return BadRequest("StartPoint, EndPoint, and Steps are required");
            }

            var session = await _navigationService.CreateSessionAsync(
                request.StartPoint,
                request.EndPoint,
                request.Steps);

            return Ok(session);
        }

        [HttpGet("qr-code/{sessionId}")]
        public async Task<IActionResult> GenerateQRCode(string sessionId)
        {
            var session = await _navigationService.GetSessionAsync(sessionId);
            if (session == null)
            {
                return NotFound("Session not found or expired");
            }

            var baseUrl = _configuration.GetValue<string>("FrontendUrl");
            var navigationUrl = $"{baseUrl}/navigate/{sessionId}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(navigationUrl, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            
            return File(qrCodeBytes, "image/png");
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<NavigationSession>> GetSession(string sessionId)
        {
            var session = await _navigationService.GetSessionAsync(sessionId);
            if (session == null)
            {
                return NotFound("Session not found or expired");
            }

            return Ok(session);
        }
    }

    public class CreateRouteRequest
    {
        public string? StartPoint { get; set; }
        public string? EndPoint { get; set; }
        public List<NavigationStep>? Steps { get; set; }
    }
}
