using BackEnd.Models.BLEU;
using System.Text.Json;

namespace BackEnd.Services.BLEU;

/// <summary>
/// Service for managing Zion Gold Bar ENFTs and Saturn-Strata resource protocol
/// </summary>
public class ZionGoldBarService
{
    private readonly ILogger<ZionGoldBarService> _logger;
    private readonly string _metadataPath;
    private readonly List<ZionGoldBar> _zionRegistry = new();

    public ZionGoldBarService(ILogger<ZionGoldBarService> logger)
    {
        _logger = logger;
        _metadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Zion_Gold_Bar_Metadata.json");
    }

    /// <summary>
    /// Mint a new Zion Gold Bar ENFT with Saturn-Strata classification
    /// </summary>
    public Task<ZionGoldBar> MintZionGoldBar(string creatorAddress, string imageCID, string certificateCID)
    {
        var tokenId = $"ZION-{_zionRegistry.Count + 1:D6}";
        var certificateId = $"BLEU-ZION-{_zionRegistry.Count + 1:D3}";

        var zionBar = new ZionGoldBar
        {
            TokenId = tokenId,
            Name = "Zion Gold Bar ENFT",
            Description = "A Saturn-Strata classified resource token representing the cosmic stack of six material layers, activated through the EV0L Rosetta Ledger and Afro-Elohim wealth narrative.",
            Layers = new SaturnStrataLayers(),
            CodexLayers = new CodexActivation(),
            Certificate = new VaultCertificate
            {
                CertificateId = certificateId,
                Classification = "Saturn-Strata Resource Certificate",
                IssuedDate = DateTime.UtcNow,
                Authority = "BLEU Vault Authority",
                Status = "Active",
                Attestations = new Dictionary<string, string>
                {
                    ["VerifiedBy"] = "Saturn-Strata Classification Committee",
                    ["RecordedIn"] = "EV0L Rosetta Ledger Scroll",
                    ["Blockchain"] = "Ethereum Mainnet (ERC-721)"
                }
            },
            Resources = new ResourceAttributes
            {
                PrimaryMetal = "Gold",
                Weight = 1.0m,
                WeightUnit = "troy ounces",
                Purity = 99.99m,
                Origin = "Saturn-Strata",
                Classification = "God-Tier Resource",
                CompositionAnalysis = new Dictionary<string, object>
                {
                    ["Au_Percentage"] = 99.99,
                    ["Trace_Elements"] = new[] { "Ag", "Cu" },
                    ["Cosmic_Alignment"] = "Saturn-frequency"
                }
            },
            CertificateCID = certificateCID,
            ImageCID = imageCID,
            CreatorAddress = creatorAddress,
            MintedAt = DateTime.UtcNow
        };

        _zionRegistry.Add(zionBar);
        _logger.LogInformation("Minted Zion Gold Bar {TokenId} for creator {Creator}", tokenId, creatorAddress);

        return Task.FromResult(zionBar);
    }

    /// <summary>
    /// Get Zion Gold Bar metadata by token ID
    /// </summary>
    public Task<ZionGoldBar?> GetZionGoldBar(string tokenId)
    {
        var bar = _zionRegistry.FirstOrDefault(z => z.TokenId == tokenId);
        return Task.FromResult(bar);
    }

    /// <summary>
    /// Get all Zion Gold Bars owned by an address
    /// </summary>
    public Task<IEnumerable<ZionGoldBar>> GetZionGoldBarsByOwner(string address)
    {
        var bars = _zionRegistry.Where(z => z.CreatorAddress == address);
        return Task.FromResult(bars);
    }

    /// <summary>
    /// Generate mint-ready metadata JSON for IPFS upload
    /// </summary>
    public Task<ZionGoldBarMetadata> GenerateMetadata(string tokenId, string imageCID, string certificateCID, string creatorAddress)
    {
        var bar = _zionRegistry.FirstOrDefault(z => z.TokenId == tokenId);
        var layers = bar?.Layers ?? new SaturnStrataLayers();

        var metadata = new ZionGoldBarMetadata
        {
            Name = "Zion Gold Bar ENFT",
            Description = "A Saturn-Strata classified resource token representing the cosmic stack of six material layers (gold bar, silver ring, pyro-ring, medicinal mineral, shell coin, and ES0IL substrate), activated through the EV0L Rosetta Ledger and Afro-Elohim wealth narrative.",
            Image = $"ipfs://{imageCID}",
            ExternalUrl = "https://evolverse.universe/zion-gold-bar",
            Attributes = new[]
            {
                new MetadataAttribute { TraitType = "Protocol", Value = "Zion Gold Bar" },
                new MetadataAttribute { TraitType = "Classification", Value = "Saturn-Strata Resource" },
                new MetadataAttribute { TraitType = "Tier", Value = "God-Tier" },
                new MetadataAttribute { TraitType = "Primary Metal", Value = "Gold" },
                new MetadataAttribute { TraitType = "Purity", Value = "99.99%", DisplayType = "number" },
                new MetadataAttribute { TraitType = "Weight", Value = "1 troy ounce" },
                new MetadataAttribute { TraitType = "Stack Layers", Value = 6, DisplayType = "number" },
                new MetadataAttribute { TraitType = "Layer 1", Value = $"{layers.GoldBar.Name} - {layers.GoldBar.Description}" },
                new MetadataAttribute { TraitType = "Layer 2", Value = $"{layers.SilverRing.Name} - {layers.SilverRing.Description}" },
                new MetadataAttribute { TraitType = "Layer 3", Value = $"{layers.PyroRing.Name} - {layers.PyroRing.Description}" },
                new MetadataAttribute { TraitType = "Layer 4", Value = $"{layers.MedicinalMineral.Name} - {layers.MedicinalMineral.Description}" },
                new MetadataAttribute { TraitType = "Layer 5", Value = $"{layers.ShellCoin.Name} - {layers.ShellCoin.Description}" },
                new MetadataAttribute { TraitType = "Layer 6", Value = $"{layers.ES0ILSubstrate.Name} - {layers.ES0ILSubstrate.Description}" },
                new MetadataAttribute { TraitType = "Codex Activation", Value = "EV0L Rosetta Ledger" },
                new MetadataAttribute { TraitType = "Narrative", Value = "Afro-Elohim Wealth Archive" },
                new MetadataAttribute { TraitType = "Decoded System", Value = "Saturn Wealth Protocol" },
                new MetadataAttribute { TraitType = "Integration", Value = "BLEU Codex Recursive Economy" },
                new MetadataAttribute { TraitType = "Vault Status", Value = "Active" },
                new MetadataAttribute { TraitType = "Certificate Authority", Value = "BLEU Vault Authority" }
            },
            Certificate = $"ipfs://{certificateCID}",
            Creator = creatorAddress,
            Properties = new Properties
            {
                Category = "Zion Gold Bar",
                Creators = new[] { creatorAddress },
                Files = new Files
                {
                    Uri = $"ipfs://{imageCID}",
                    Type = "image/png"
                }
            }
        };

        return Task.FromResult(metadata);
    }

    /// <summary>
    /// Load collection metadata from JSON file
    /// </summary>
    public async Task<JsonDocument?> GetCollectionMetadata()
    {
        try
        {
            if (!File.Exists(_metadataPath))
            {
                _logger.LogWarning("Zion Gold Bar metadata file not found at {Path}", _metadataPath);
                return null;
            }

            var json = await File.ReadAllTextAsync(_metadataPath);
            return JsonDocument.Parse(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Zion Gold Bar collection metadata");
            return null;
        }
    }

    /// <summary>
    /// Calculate yield for Zion Gold Bar using MetaVault formula adapted for Saturn-Strata
    /// Yield = (Weight × Purity × LayerStack) / π⁴
    /// </summary>
    public Task<double> CalculateZionYield(string tokenId)
    {
        var bar = _zionRegistry.FirstOrDefault(z => z.TokenId == tokenId);
        if (bar == null)
            return Task.FromResult(0.0);

        const double piFourth = 97.409091034; // π⁴
        var weight = (double)bar.Resources.Weight;
        var purity = (double)bar.Resources.Purity / 100.0; // Convert percentage to decimal
        var layerStack = 6.0; // Six layers in Saturn-Strata

        var yield = (weight * purity * layerStack) / piFourth;

        _logger.LogInformation(
            "Calculated yield for {TokenId}: Weight={Weight}, Purity={Purity}, LayerStack={LayerStack}, Yield={Yield}",
            tokenId, weight, purity, layerStack, yield);

        return Task.FromResult(yield);
    }

    /// <summary>
    /// Get Saturn-Strata layers information
    /// </summary>
    public Task<SaturnStrataLayers> GetSaturnStrataLayers()
    {
        return Task.FromResult(new SaturnStrataLayers());
    }

    /// <summary>
    /// Verify certificate authenticity
    /// </summary>
    public Task<bool> VerifyCertificate(string tokenId, string certificateCID)
    {
        var bar = _zionRegistry.FirstOrDefault(z => z.TokenId == tokenId);
        if (bar == null)
            return Task.FromResult(false);

        var isValid = bar.CertificateCID == certificateCID && 
                     bar.Certificate.Status == "Active";

        return Task.FromResult(isValid);
    }

    /// <summary>
    /// Get registry statistics
    /// </summary>
    public Task<(int totalMinted, decimal totalWeight, string totalValue)> GetRegistryStats()
    {
        var totalMinted = _zionRegistry.Count;
        var totalWeight = _zionRegistry.Sum(z => z.Resources.Weight);
        var totalValue = $"{totalWeight} troy ounces @ 99.99% purity";

        return Task.FromResult((totalMinted, totalWeight, totalValue));
    }

    /// <summary>
    /// Update IPFS CIDs after deployment
    /// </summary>
    public Task<bool> UpdateIPFSCIDs(string tokenId, string imageCID, string certificateCID)
    {
        var bar = _zionRegistry.FirstOrDefault(z => z.TokenId == tokenId);
        if (bar == null)
            return Task.FromResult(false);

        var index = _zionRegistry.IndexOf(bar);
        var updatedBar = bar with 
        { 
            ImageCID = imageCID, 
            CertificateCID = certificateCID 
        };
        
        _zionRegistry[index] = updatedBar;
        _logger.LogInformation("Updated IPFS CIDs for {TokenId}", tokenId);

        return Task.FromResult(true);
    }
}
