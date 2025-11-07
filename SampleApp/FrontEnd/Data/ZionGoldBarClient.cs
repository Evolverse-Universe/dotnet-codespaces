using System.Net.Http.Json;

namespace FrontEnd.Data;

public record SaturnResource(string Layer, string Symbol, string Resource, string Function, string SectoralUse);

public record EnftCodexEntry(
    string TokenId,
    string Layer,
    string Symbol,
    string Resource,
    string Function,
    string SectoralUse,
    string MemorialSite,
    string AncestralLineage,
    DateTime MintedDate,
    string HashSignature
);

public class ZionGoldBarClient
{
    private readonly HttpClient _httpClient;

    public ZionGoldBarClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SaturnResource[]> GetSaturnResourcesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<SaturnResource[]>("saturn-resources") ?? Array.Empty<SaturnResource>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching Saturn resources: {ex.Message}");
            return Array.Empty<SaturnResource>();
        }
    }

    public async Task<string> GenerateCertificateAsync(string issuedTo, decimal value)
    {
        var response = await _httpClient.PostAsync(
            $"certificate/generate?issuedTo={Uri.EscapeDataString(issuedTo)}&value={value}",
            null
        );
        response.EnsureSuccessStatusCode();
        
        // Return the URL for direct download
        return $"{_httpClient.BaseAddress}certificate/generate?issuedTo={Uri.EscapeDataString(issuedTo)}&value={value}";
    }

    public async Task<EnftCodexEntry?> MintEnftAsync(string layer, string memorialSite, string ancestralLineage)
    {
        var response = await _httpClient.PostAsync(
            $"enft/mint?layer={Uri.EscapeDataString(layer)}&memorialSite={Uri.EscapeDataString(memorialSite)}&ancestralLineage={Uri.EscapeDataString(ancestralLineage)}",
            null
        );
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EnftCodexEntry>();
    }
}
