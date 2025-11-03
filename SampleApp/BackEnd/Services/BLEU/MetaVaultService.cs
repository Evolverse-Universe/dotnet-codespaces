using BackEnd.Models.BLEU;

namespace BackEnd.Services.BLEU;

/// <summary>
/// Service for managing MetaVault operations and yield calculations
/// </summary>
public class MetaVaultService
{
    private readonly ILogger<MetaVaultService> _logger;
    private readonly List<VaultEvent> _vaultEvents = new();
    private readonly EnergyTreasury _energyTreasury;

    public MetaVaultService(ILogger<MetaVaultService> logger)
    {
        _logger = logger;
        _energyTreasury = new EnergyTreasury
        {
            TotalStaked = 0m,
            CompoundRate = 97.409091034,
            LastCompoundTime = DateTime.UtcNow,
            StakerBalances = new Dictionary<string, decimal>()
        };
    }

    /// <summary>
    /// Trigger a vault event for an ENFT plate interaction
    /// </summary>
    public Task<VaultEvent> TriggerVaultEvent(string tokenId, double temperature, double memoryIndex, ENFTTier tier)
    {
        var vaultEvent = YieldCalculator.CreateVaultEvent(tokenId, temperature, memoryIndex, tier);
        _vaultEvents.Add(vaultEvent);

        _logger.LogInformation(
            "Vault event triggered for {TokenId}: Temperature={Temperature}, MemoryIndex={MemoryIndex}, Yield={Yield}",
            tokenId, temperature, memoryIndex, vaultEvent.CalculatedYield);

        return Task.FromResult(vaultEvent);
    }

    /// <summary>
    /// Get all vault events for a specific token
    /// </summary>
    public Task<IEnumerable<VaultEvent>> GetVaultEventsByToken(string tokenId)
    {
        var events = _vaultEvents.Where(e => e.TokenId == tokenId);
        return Task.FromResult(events);
    }

    /// <summary>
    /// Get total yield generated for a token
    /// </summary>
    public Task<double> GetTotalYieldForToken(string tokenId)
    {
        var totalYield = _vaultEvents
            .Where(e => e.TokenId == tokenId)
            .Sum(e => e.CalculatedYield);
        return Task.FromResult(totalYield);
    }

    /// <summary>
    /// Stake EGoin tokens in the Energy Treasury
    /// </summary>
    public Task<bool> StakeEGoin(string address, decimal amount)
    {
        if (amount <= 0)
        {
            _logger.LogWarning("Invalid stake amount: {Amount}", amount);
            return Task.FromResult(false);
        }

        if (!_energyTreasury.StakerBalances.ContainsKey(address))
        {
            _energyTreasury.StakerBalances[address] = 0m;
        }

        _energyTreasury.StakerBalances[address] += amount;
        var newTreasury = _energyTreasury with 
        { 
            TotalStaked = _energyTreasury.TotalStaked + amount,
            LastCompoundTime = DateTime.UtcNow
        };

        _logger.LogInformation("Staked {Amount} EGoin for address {Address}", amount, address);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Calculate compound yield for staked EGoin
    /// Compounds daily at π⁴ rate
    /// </summary>
    public Task<decimal> CalculateCompoundYield(string address, DateTime untilDate)
    {
        if (!_energyTreasury.StakerBalances.TryGetValue(address, out var stakedBalance))
        {
            return Task.FromResult(0m);
        }

        var timeStaked = untilDate - _energyTreasury.LastCompoundTime;
        var daysStaked = timeStaked.TotalDays;

        // Simplified compound calculation: A = P(1 + r)^t
        // Using π⁴ as daily rate percentage
        var rate = (decimal)_energyTreasury.CompoundRate / 100m;
        var compoundYield = stakedBalance * (decimal)Math.Pow((double)(1 + rate / 365m), daysStaked) - stakedBalance;

        return Task.FromResult(compoundYield);
    }

    /// <summary>
    /// Get Energy Treasury status
    /// </summary>
    public Task<EnergyTreasury> GetEnergyTreasuryStatus()
    {
        return Task.FromResult(_energyTreasury);
    }

    /// <summary>
    /// Get vault event statistics
    /// </summary>
    public Task<(int totalEvents, double totalYield, double averageYield)> GetVaultStatistics()
    {
        var totalEvents = _vaultEvents.Count;
        var totalYield = _vaultEvents.Sum(e => e.CalculatedYield);
        var averageYield = totalEvents > 0 ? totalYield / totalEvents : 0;

        return Task.FromResult((totalEvents, totalYield, averageYield));
    }

    /// <summary>
    /// Implement Harvest-Mint-Heal loop
    /// </summary>
    public async Task<string> ExecuteHarvestMintHealLoop(string tokenId, double temperature, double memoryIndex, ENFTTier tier)
    {
        // HARVEST: Collect thermal data and usage metrics
        _logger.LogInformation("HARVEST: Collecting thermal data for {TokenId}", tokenId);
        
        // MINT: Generate yield tokens based on vault event
        var vaultEvent = await TriggerVaultEvent(tokenId, temperature, memoryIndex, tier);
        _logger.LogInformation("MINT: Generated yield of {Yield} for {TokenId}", vaultEvent.CalculatedYield, tokenId);
        
        // HEAL: Reinvest into Energy Treasury for compound growth
        var reinvestAmount = (decimal)vaultEvent.CalculatedYield * 0.1m; // 10% auto-reinvest
        await StakeEGoin(tokenId, reinvestAmount);
        _logger.LogInformation("HEAL: Reinvested {Amount} into Energy Treasury", reinvestAmount);
        
        return $"Harvest-Mint-Heal loop completed for {tokenId}. Yield: {vaultEvent.CalculatedYield}, Reinvested: {reinvestAmount}";
    }
}
