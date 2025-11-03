namespace BackEnd.Models.BLEU;

/// <summary>
/// Revenue split configuration for BLEU Flame™ NFT sales
/// </summary>
public record RevenueSplit
{
    public decimal PublicDropPercentage { get; init; } = 60m;
    public decimal EliteFoundersPercentage { get; init; } = 30m;
    public decimal GodTierPercentage { get; init; } = 10m;

    public void Validate()
    {
        var total = PublicDropPercentage + EliteFoundersPercentage + GodTierPercentage;
        if (total != 100m)
            throw new InvalidOperationException($"Revenue split percentages must total 100%, got {total}%");
    }
}

/// <summary>
/// Cross-sector income route configuration
/// </summary>
public record IncomeSector
{
    public string Name { get; init; } = string.Empty;
    public string Mechanism { get; init; } = string.Empty;
    public string OutputCurrency { get; init; } = string.Empty;
    public decimal YieldPercentage { get; init; }
}

/// <summary>
/// Treasury allocation for different revenue streams
/// </summary>
public record TreasuryAllocation
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string TransactionHash { get; init; } = string.Empty;
}

/// <summary>
/// Available cryptocurrencies in the BLEU ecosystem
/// </summary>
public static class BLEUCurrencies
{
    public const string FlameCoin = "FlameCoin";
    public const string HarvestCoin = "HarvestCoin";
    public const string ScrollCoin = "ScrollCoin";
    public const string BluTillion = "Blu-Tillion";
    public const string EGoin = "EGoin";
}
