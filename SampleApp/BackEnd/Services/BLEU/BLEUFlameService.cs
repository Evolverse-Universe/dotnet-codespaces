using BackEnd.Models.BLEU;
using System.Text.Json;

namespace BackEnd.Services.BLEU;

/// <summary>
/// Service for managing BLEU Flame™ ENFTs and related operations
/// </summary>
public class BLEUFlameService
{
    private readonly ILogger<BLEUFlameService> _logger;
    private readonly string _metadataPath;
    private readonly List<ENFTMetadata> _nftRegistry = new();

    public BLEUFlameService(ILogger<BLEUFlameService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _metadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "BLEU_Flame_Metadata.json");
    }

    /// <summary>
    /// Mint a new BLEU Flame™ ENFT
    /// </summary>
    public Task<ENFTMetadata> MintENFT(ENFTTier tier, string owner, ENFTAttributes attributes)
    {
        var tokenId = $"BLEU-{_nftRegistry.Count + 1:D6}";
        // NOTE: In production, this should be a real IPFS CID after uploading metadata to IPFS
        // This is a simulated CID for demonstration purposes only
        var ipfsHash = $"Qm{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")[..12]}"; // Simulated IPFS hash

        var metadata = new ENFTMetadata
        {
            TokenId = tokenId,
            Name = $"BLEU Flame™ {tier} #{_nftRegistry.Count + 1}",
            Description = GetTierDescription(tier),
            Image = $"ipfs://{ipfsHash}/image.png",
            Tier = tier,
            Attributes = attributes,
            Ownership = new OwnershipInfo
            {
                CurrentOwner = owner,
                OriginalMinter = owner,
                AcquiredAt = DateTime.UtcNow,
                TransferCount = 0
            },
            MemoryLogs = new[]
            {
                new MemoryLog
                {
                    Timestamp = DateTime.UtcNow,
                    EventType = "Mint",
                    Description = "ENFT minted",
                    Data = new Dictionary<string, object>
                    {
                        ["tier"] = tier.ToString(),
                        ["minter"] = owner
                    }
                }
            },
            CreatedAt = DateTime.UtcNow,
            IpfsHash = ipfsHash
        };

        _nftRegistry.Add(metadata);
        _logger.LogInformation("Minted ENFT {TokenId} for owner {Owner}", tokenId, owner);

        return Task.FromResult(metadata);
    }

    /// <summary>
    /// Get ENFT metadata by token ID
    /// </summary>
    public Task<ENFTMetadata?> GetENFTMetadata(string tokenId)
    {
        var nft = _nftRegistry.FirstOrDefault(n => n.TokenId == tokenId);
        return Task.FromResult(nft);
    }

    /// <summary>
    /// Get all ENFTs by tier
    /// </summary>
    public Task<IEnumerable<ENFTMetadata>> GetENFTsByTier(ENFTTier tier)
    {
        var nfts = _nftRegistry.Where(n => n.Tier == tier);
        return Task.FromResult(nfts);
    }

    /// <summary>
    /// Get all ENFTs owned by an address
    /// </summary>
    public Task<IEnumerable<ENFTMetadata>> GetENFTsByOwner(string owner)
    {
        var nfts = _nftRegistry.Where(n => n.Ownership.CurrentOwner == owner);
        return Task.FromResult(nfts);
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
                _logger.LogWarning("Metadata file not found at {Path}", _metadataPath);
                return null;
            }

            var json = await File.ReadAllTextAsync(_metadataPath);
            return JsonDocument.Parse(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading collection metadata");
            return null;
        }
    }

    /// <summary>
    /// Calculate revenue split for a given sale amount
    /// </summary>
    public (decimal publicDrop, decimal eliteFounders, decimal godTier) CalculateRevenueSplit(decimal saleAmount)
    {
        var split = new RevenueSplit();
        split.Validate();

        return (
            saleAmount * split.PublicDropPercentage / 100m,
            saleAmount * split.EliteFoundersPercentage / 100m,
            saleAmount * split.GodTierPercentage / 100m
        );
    }

    /// <summary>
    /// Get tier-specific description
    /// </summary>
    private static string GetTierDescription(ENFTTier tier) => tier switch
    {
        ENFTTier.PublicDrop => "Standard BLEU Flame™ SmartCeramic plate with base thermal calibration",
        ENFTTier.EliteFounders => "Enhanced SmartCeramic plate with advanced features and 2x yield multiplier",
        ENFTTier.GodTier => "Ultra-rare 1-of-1 SmartCeramic masterpiece with maximum yield and exclusive benefits",
        _ => "BLEU Flame™ SmartCeramic ENFT"
    };

    /// <summary>
    /// Get current registry statistics
    /// </summary>
    public Task<(int total, int publicDrop, int eliteFounders, int godTier)> GetRegistryStats()
    {
        var stats = (
            total: _nftRegistry.Count,
            publicDrop: _nftRegistry.Count(n => n.Tier == ENFTTier.PublicDrop),
            eliteFounders: _nftRegistry.Count(n => n.Tier == ENFTTier.EliteFounders),
            godTier: _nftRegistry.Count(n => n.Tier == ENFTTier.GodTier)
        );
        return Task.FromResult(stats);
    }
}
