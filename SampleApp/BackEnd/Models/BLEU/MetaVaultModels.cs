namespace BackEnd.Models.BLEU;

/// <summary>
/// MetaVault event triggered by plate interactions
/// </summary>
public record VaultEvent
{
    public string EventId { get; init; } = string.Empty;
    public string TokenId { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public double Temperature { get; init; }
    public double MemoryIndex { get; init; }
    public ENFTTier OwnershipTier { get; init; }
    public double CalculatedYield { get; init; }
}

/// <summary>
/// Yield calculation based on MetaVault formula: Yield = (Temperature × MemoryIndex × OwnershipTier) / π⁴
/// </summary>
public class YieldCalculator
{
    private const double PiFourth = 97.409091034; // π⁴

    /// <summary>
    /// Calculate yield for a vault event
    /// </summary>
    public static double CalculateYield(double temperature, double memoryIndex, ENFTTier ownershipTier)
    {
        double tierMultiplier = ownershipTier switch
        {
            ENFTTier.PublicDrop => 1.0,
            ENFTTier.EliteFounders => 2.0,
            ENFTTier.GodTier => 5.0,
            _ => 1.0
        };

        return (temperature * memoryIndex * tierMultiplier) / PiFourth;
    }

    /// <summary>
    /// Create a vault event with calculated yield
    /// </summary>
    public static VaultEvent CreateVaultEvent(string tokenId, double temperature, double memoryIndex, ENFTTier tier)
    {
        var calculatedYield = CalculateYield(temperature, memoryIndex, tier);
        
        return new VaultEvent
        {
            EventId = Guid.NewGuid().ToString(),
            TokenId = tokenId,
            Timestamp = DateTime.UtcNow,
            Temperature = temperature,
            MemoryIndex = memoryIndex,
            OwnershipTier = tier,
            CalculatedYield = calculatedYield
        };
    }
}

/// <summary>
/// Energy Treasury configuration for EGoin staking
/// Note: Using a class instead of record for mutable state management
/// </summary>
public class EnergyTreasury
{
    public decimal TotalStaked { get; set; }
    public double CompoundRate { get; set; } = 97.409091034; // π⁴ rate
    public DateTime LastCompoundTime { get; set; }
    public Dictionary<string, decimal> StakerBalances { get; set; } = new();
}
