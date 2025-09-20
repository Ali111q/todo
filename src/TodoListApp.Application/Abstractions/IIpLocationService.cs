namespace TodoListApp.Application.Abstractions;

public interface IIpLocationService
{
    Task<IpLocationDto> GetLocationAsync(string ipAddress, CancellationToken cancellationToken = default);
}

public sealed record IpLocationDto(
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
