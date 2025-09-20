using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Application.Abstractions;

namespace TodoListApp.Api.Controllers;

[ApiController]
[Route("api/v1/ip")]
[Authorize]
public sealed class IpController : ControllerBase
{
    private readonly IIpLocationService _ipLocationService;

    public IpController(IIpLocationService ipLocationService)
    {
        _ipLocationService = ipLocationService;
    }

    [HttpGet("location/{ipAddress}")]
    public async Task<IActionResult> GetLocation(string ipAddress, CancellationToken ct)
    {
        var location = await _ipLocationService.GetLocationAsync(ipAddress, ct);
        return Ok(location);
    }

    [HttpGet("location")]
    public async Task<IActionResult> GetGoogleDnsLocation(CancellationToken ct)
    {
        // Default to Google's DNS as per requirements
        var location = await _ipLocationService.GetLocationAsync("8.8.8.8", ct);
        return Ok(location);
    }
}
