using BackEnd.Models;

namespace BackEnd.Services;

/// <summary>
/// Service for managing Saturn Strata resources and Zion Gold Bar data
/// </summary>
public class SaturnResourceService
{
    private static readonly List<SaturnResource> _saturnResources = new()
    {
        new("⚜️ Zion Gold Bar", "🔱🍫", "Gold (Au) + Cocoa Matrix", "Value standard, ceremonial nutrient", "Finance, Ritual, Ceremony, Trade"),
        new("💍 Saturn Ring Silver", "♒️⚪️", "Ag ring composites", "Contracts, frequency tuning", "Jewelry, Governance, Resonance tech"),
        new("🔥 Pyro-Ring (Flamethrower Metal)", "🔥🧬", "Medical-grade plasma alloy", "Cancer targeting, VR weapons", "Oncology, Defense, Ritual fire"),
        new("💉 Gastric Cancer Mineral", "🧫🌡️", "Digestive-reactive compounds", "Smart meds & sensor embedding", "Medical, Consumer, Agriculture"),
        new("🪙 Shell Currency (₽)", "🐚 + CaCO₃", "Embedded calcium currency", "Barter, consumer-grade tokens", "Memorial sites, village trade"),
        new("🧬 ES0IL Substrate (∅)", "⟁🧬", "Bio-coded waveform math", "Zero-point agriculture", "Farming, SeedVaults, Terraforming")
    };

    public IEnumerable<SaturnResource> GetAllSaturnResources()
    {
        return _saturnResources;
    }

    public ZionGoldBarCertificate GenerateCertificate(string issuedTo, decimal value)
    {
        var certificateId = $"BLEU-{Guid.NewGuid():N}".ToUpper();
        return new ZionGoldBarCertificate(
            CertificateId: certificateId,
            Layer: "⚜️ Zion Gold Bar",
            Symbol: "🔱🍫",
            Resource: "Gold (Au) + Cocoa Matrix",
            IssuedDate: DateTime.UtcNow,
            IssuedTo: issuedTo,
            Value: value
        );
    }

    public EnftCodexEntry MintEnftToken(string layer, string memorialSite, string ancestralLineage)
    {
        var resource = _saturnResources.FirstOrDefault(r => r.Layer == layer) 
            ?? _saturnResources[0]; // Default to Zion Gold Bar
        
        var tokenId = $"ENFT-{Guid.NewGuid():N}".ToUpper();
        var hashSignature = GenerateHashSignature(tokenId, layer, memorialSite);
        
        return new EnftCodexEntry(
            TokenId: tokenId,
            Layer: resource.Layer,
            Symbol: resource.Symbol,
            Resource: resource.Resource,
            Function: resource.Function,
            SectoralUse: resource.SectoralUse,
            MemorialSite: memorialSite,
            AncestralLineage: ancestralLineage,
            MintedDate: DateTime.UtcNow,
            HashSignature: hashSignature
        );
    }

    private static string GenerateHashSignature(string tokenId, string layer, string memorialSite)
    {
        var input = $"{tokenId}{layer}{memorialSite}{DateTime.UtcNow:O}";
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}
