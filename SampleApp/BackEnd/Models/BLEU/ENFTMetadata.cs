namespace BackEnd.Models.BLEU;

/// <summary>
/// ENFT (Enhanced Non-Fungible Token) metadata structure for BLEU Flame™ market tier
/// </summary>
public record ENFTMetadata
{
    public string TokenId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public ENFTTier Tier { get; init; }
    public ENFTAttributes Attributes { get; init; } = new();
    public OwnershipInfo Ownership { get; init; } = new();
    public MemoryLog[] MemoryLogs { get; init; } = Array.Empty<MemoryLog>();
    public DateTime CreatedAt { get; init; }
    public string IpfsHash { get; init; } = string.Empty;
}

/// <summary>
/// Three-tier system for BLEU Flame™ NFTs
/// </summary>
public enum ENFTTier
{
    PublicDrop = 1,      // 60% distribution
    EliteFounders = 2,   // 30% distribution
    GodTier = 3          // 10% distribution (1-of-1)
}

/// <summary>
/// ENFT attributes including SmartCeramic properties and thermal data
/// </summary>
public record ENFTAttributes
{
    public string SmartCeramicType { get; init; } = string.Empty;
    public double Temperature { get; init; }
    public double MemoryIndex { get; init; }
    public double ThermalCalibration { get; init; }
    public string[] Features { get; init; } = Array.Empty<string>();
    public Dictionary<string, object> CustomProperties { get; init; } = new();
}

/// <summary>
/// Ownership information for digital twin tracking
/// </summary>
public record OwnershipInfo
{
    public string CurrentOwner { get; init; } = string.Empty;
    public string OriginalMinter { get; init; } = string.Empty;
    public DateTime AcquiredAt { get; init; }
    public int TransferCount { get; init; }
}

/// <summary>
/// Memory log entries for tracking NFT history
/// </summary>
public record MemoryLog
{
    public DateTime Timestamp { get; init; }
    public string EventType { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Dictionary<string, object> Data { get; init; } = new();
}
