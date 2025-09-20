using System.Text.Json;
using TodoListApp.Application.Abstractions;

namespace TodoListApp.Infrastructure.Services;

public sealed class IpLocationService : IIpLocationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public IpLocationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IpLocationDto> GetLocationAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"http://ip-api.com/json/{ipAddress}", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var locationData = JsonSerializer.Deserialize<IpApiResponse>(jsonContent, _jsonOptions);

            if (locationData == null || locationData.Status != "success")
            {
                throw new InvalidOperationException($"Failed to get location data for IP: {ipAddress}");
            }

            return new IpLocationDto(
                locationData.Country,
                locationData.CountryCode,
                locationData.Region,
                locationData.RegionName,
                locationData.City,
                locationData.Zip,
                locationData.Lat,
                locationData.Lon,
                locationData.Timezone,
                locationData.Isp,
                locationData.Org,
                locationData.As,
                locationData.Query,
                locationData.Status);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"HTTP error while fetching location data: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new InvalidOperationException($"Request timeout while fetching location data: {ex.Message}", ex);
        }
    }

    private sealed record IpApiResponse(
        string Country,
        string CountryCode,
        string Region,
        string RegionName,
        string City,
        string Zip,
        double Lat,
        double Lon,
        string Timezone,
        string Isp,
        string Org,
        string As,
        string Query,
        string Status);
}
