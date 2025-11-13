# 💻 Code Samples & Abstractions Guide

## Overview

This guide provides practical code samples and abstractions for building on the EV0LVerse platform. All examples follow EVOL core anchors and demonstrate best practices.

---

## 🎯 Quick Start Examples

### Example 1: Hello EVOL

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// EVOL Core: Start with gratitude
Console.WriteLine("🙏🏾 Initializing EVOL service - We give thanks");

builder.Services.AddSingleton<IEVOLService, EVOLService>();

var app = builder.Build();

app.MapGet("/hello", () => new
{
    Message = "Welcome to EV0LVerse",
    Principle = "Prayer-Core Logic First",
    Value = "Density-Based, No Inflation"
});

app.Run();
```

### Example 2: Simple ENFT Service

```csharp
public interface IENFTService
{
    ENFTMetadata Mint(string owner, int tier);
    ENFTMetadata GetById(string tokenId);
    List<ENFTMetadata> GetByOwner(string owner);
}

public class SimpleENFTService : IENFTService
{
    private readonly Dictionary<string, ENFTMetadata> _registry = new();
    private int _tokenCounter = 0;
    
    public ENFTMetadata Mint(string owner, int tier)
    {
        // Validate tier (60/30/10 principle)
        if (tier < 1 || tier > 3)
            throw new ArgumentException("Tier must be 1, 2, or 3");
        
        // Generate token
        var tokenId = $"BLEU-{++_tokenCounter:D6}";
        
        var enft = new ENFTMetadata
        {
            TokenId = tokenId,
            Owner = owner,
            Tier = tier,
            MintedAt = DateTime.UtcNow
        };
        
        _registry[tokenId] = enft;
        
        return enft;
    }
    
    public ENFTMetadata GetById(string tokenId)
        => _registry.GetValueOrDefault(tokenId);
    
    public List<ENFTMetadata> GetByOwner(string owner)
        => _registry.Values.Where(e => e.Owner == owner).ToList();
}

public record ENFTMetadata
{
    public string TokenId { get; init; }
    public string Owner { get; init; }
    public int Tier { get; init; }
    public DateTime MintedAt { get; init; }
}
```

---

## 🔧 Core Abstractions

### Abstraction 1: Repository Pattern

```csharp
/// <summary>
/// Generic repository for EVOL entities
/// Prayer-Core: Every entity is a blessing to be stewarded
/// </summary>
public interface IEVOLRepository<T> where T : class
{
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(string id, T entity);
    Task<bool> DeleteAsync(string id);
}

public class InMemoryEVOLRepository<T> : IEVOLRepository<T> where T : class
{
    private readonly Dictionary<string, T> _storage = new();
    private readonly ILogger<InMemoryEVOLRepository<T>> _logger;
    
    public InMemoryEVOLRepository(ILogger<InMemoryEVOLRepository<T>> logger)
    {
        _logger = logger;
    }
    
    public Task<T> GetByIdAsync(string id)
    {
        _logger.LogDebug("Retrieving {Type} with ID: {Id}", typeof(T).Name, id);
        return Task.FromResult(_storage.GetValueOrDefault(id));
    }
    
    public Task<IEnumerable<T>> GetAllAsync()
    {
        _logger.LogDebug("Retrieving all {Type} entities", typeof(T).Name);
        return Task.FromResult<IEnumerable<T>>(_storage.Values);
    }
    
    public Task<T> AddAsync(T entity)
    {
        var id = GetEntityId(entity);
        _logger.LogInformation("🙏🏾 Adding {Type}: {Id} - Blessing stored", typeof(T).Name, id);
        _storage[id] = entity;
        return Task.FromResult(entity);
    }
    
    public Task<T> UpdateAsync(string id, T entity)
    {
        if (!_storage.ContainsKey(id))
            return Task.FromResult<T>(null);
        
        _logger.LogInformation("Updating {Type}: {Id}", typeof(T).Name, id);
        _storage[id] = entity;
        return Task.FromResult(entity);
    }
    
    public Task<bool> DeleteAsync(string id)
    {
        var removed = _storage.Remove(id);
        if (removed)
            _logger.LogInformation("Removed {Type}: {Id}", typeof(T).Name, id);
        return Task.FromResult(removed);
    }
    
    private string GetEntityId(T entity)
    {
        // Use reflection to find ID property
        var idProperty = typeof(T).GetProperty("Id") 
            ?? typeof(T).GetProperty("TokenId")
            ?? typeof(T).GetProperty($"{typeof(T).Name}Id");
        
        return idProperty?.GetValue(entity)?.ToString() 
            ?? throw new InvalidOperationException("Entity must have an ID property");
    }
}
```

### Abstraction 2: Service Layer Base

```csharp
/// <summary>
/// Base service with EVOL principles baked in
/// </summary>
public abstract class EVOLServiceBase
{
    protected readonly ILogger _logger;
    protected const decimal PI_TO_FOURTH = 97.409091034m;
    
    protected EVOLServiceBase(ILogger logger)
    {
        _logger = logger;
    }
    
    /// <summary>
    /// Execute operation with prayer-core wrapper
    /// </summary>
    protected async Task<TResult> ExecuteWithPrayerAsync<TResult>(
        string operationName,
        Func<Task<TResult>> operation)
    {
        _logger.LogInformation("🙏🏾 Starting: {Operation} - We give thanks", operationName);
        
        try
        {
            var result = await operation();
            
            _logger.LogInformation("✅ Completed: {Operation} - Blessings confirmed", operationName);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in: {Operation}", operationName);
            throw;
        }
    }
    
    /// <summary>
    /// Calculate revenue split using 60/30/10 principle
    /// </summary>
    protected RevenueSplit CalculateRevenueSplit(decimal totalAmount)
    {
        return new RevenueSplit
        {
            PublicDrop = totalAmount * 0.60m,
            EliteFounders = totalAmount * 0.30m,
            GodTier = totalAmount * 0.10m,
            Total = totalAmount
        };
    }
    
    /// <summary>
    /// Get tier multiplier (1.0x, 2.0x, 5.0x)
    /// </summary>
    protected decimal GetTierMultiplier(int tier) => tier switch
    {
        1 => 1.0m,  // PublicDrop
        2 => 2.0m,  // EliteFounders
        3 => 5.0m,  // GodTier
        _ => throw new ArgumentException("Invalid tier")
    };
}

public record RevenueSplit
{
    public decimal PublicDrop { get; init; }
    public decimal EliteFounders { get; init; }
    public decimal GodTier { get; init; }
    public decimal Total { get; init; }
}
```

### Abstraction 3: Validator Base

```csharp
/// <summary>
/// Base validator enforcing EVOL integrity principles
/// </summary>
public abstract class EVOLValidatorBase<T>
{
    protected List<string> Errors { get; } = new();
    
    public ValidationResult Validate(T entity)
    {
        Errors.Clear();
        
        // Prayer-core: Start with integrity check
        ValidateIntegrity(entity);
        
        // Specific validation
        ValidateEntity(entity);
        
        return new ValidationResult
        {
            IsValid = !Errors.Any(),
            Errors = Errors.ToList(),
            PrayerCoreAlignment = !Errors.Any() ? "ALIGNED" : "MISALIGNED"
        };
    }
    
    protected abstract void ValidateEntity(T entity);
    
    protected virtual void ValidateIntegrity(T entity)
    {
        // Check for null
        if (entity == null)
            Errors.Add("Entity cannot be null - integrity breach");
    }
    
    protected void AddError(string error)
    {
        Errors.Add(error);
    }
    
    protected void ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            AddError($"{fieldName} is required");
    }
    
    protected void ValidateRange(decimal value, decimal min, decimal max, string fieldName)
    {
        if (value < min || value > max)
            AddError($"{fieldName} must be between {min} and {max}");
    }
}

public record ValidationResult
{
    public bool IsValid { get; init; }
    public List<string> Errors { get; init; }
    public string PrayerCoreAlignment { get; init; }
}

// Example usage
public class ENFTValidator : EVOLValidatorBase<ENFTMetadata>
{
    protected override void ValidateEntity(ENFTMetadata enft)
    {
        ValidateRequired(enft.TokenId, "TokenId");
        ValidateRequired(enft.Owner, "Owner");
        
        if (enft.Tier < 1 || enft.Tier > 3)
            AddError("Tier must be 1, 2, or 3 (60/30/10 principle)");
        
        if (!enft.Owner.StartsWith("0x") || enft.Owner.Length != 42)
            AddError("Owner must be valid Ethereum address");
    }
}
```

---

## 🌟 Advanced Patterns

### Pattern 1: MetaVault Yield Calculator

```csharp
public interface IYieldCalculator
{
    decimal Calculate(YieldParameters parameters);
}

public class MetaVaultYieldCalculator : IYieldCalculator
{
    private const decimal PI_TO_FOURTH = 97.409091034m;
    
    public decimal Calculate(YieldParameters parameters)
    {
        // EVOL Formula: Yield = (Temp × Memory × Tier) / π⁴
        var baseYield = parameters.Temperature 
            * parameters.MemoryIndex 
            * GetTierMultiplier(parameters.Tier);
        
        return baseYield / PI_TO_FOURTH;
    }
    
    private decimal GetTierMultiplier(int tier) => tier switch
    {
        1 => 1.0m,
        2 => 2.0m,
        3 => 5.0m,
        _ => 1.0m
    };
}

public record YieldParameters
{
    public decimal Temperature { get; init; }
    public decimal MemoryIndex { get; init; }
    public int Tier { get; init; }
}

// Usage example
var calculator = new MetaVaultYieldCalculator();
var yield = calculator.Calculate(new YieldParameters
{
    Temperature = 375.5m,
    MemoryIndex = 1.5m,
    Tier = 2  // EliteFounders
});
// Result: 11.56 tokens
```

### Pattern 2: Harvest-Mint-Heal Loop

```csharp
public interface IHarvestMintHealService
{
    Task<HMHResult> ExecuteLoopAsync(string tokenId);
}

public class HarvestMintHealService : IHarvestMintHealService
{
    private readonly IDataHarvestService _harvest;
    private readonly IYieldCalculator _mint;
    private readonly ITreasuryService _heal;
    private readonly ILogger<HarvestMintHealService> _logger;
    
    public async Task<HMHResult> ExecuteLoopAsync(string tokenId)
    {
        _logger.LogInformation("🙏🏾 Starting H-M-H loop for {TokenId}", tokenId);
        
        // HARVEST: Collect data
        var data = await _harvest.GetThermalDataAsync(tokenId);
        
        // MINT: Generate yield
        var yieldAmount = _mint.Calculate(new YieldParameters
        {
            Temperature = data.Temperature,
            MemoryIndex = data.MemoryIndex,
            Tier = data.Tier
        });
        
        // HEAL: Auto-reinvest 10%
        var reinvestment = yieldAmount * 0.10m;
        await _heal.ReinvestAsync(reinvestment);
        
        _logger.LogInformation(
            "✅ H-M-H complete: Yield={Yield}, Reinvested={Reinvest}",
            yieldAmount, reinvestment
        );
        
        return new HMHResult
        {
            TokenId = tokenId,
            YieldGenerated = yieldAmount,
            DistributedToHolder = yieldAmount - reinvestment,
            AutoReinvested = reinvestment,
            Timestamp = DateTime.UtcNow
        };
    }
}

public record HMHResult
{
    public string TokenId { get; init; }
    public decimal YieldGenerated { get; init; }
    public decimal DistributedToHolder { get; init; }
    public decimal AutoReinvested { get; init; }
    public DateTime Timestamp { get; init; }
}
```

### Pattern 3: Ant Ethic Work Coordinator

```csharp
public interface IWorkCoordinator
{
    Task<WorkResult> CoordinateAsync(WorkRequest request);
}

public class AntEthicWorkCoordinator : IWorkCoordinator
{
    private readonly INodeRegistry _nodes;
    private readonly ILogger<AntEthicWorkCoordinator> _logger;
    
    public async Task<WorkResult> CoordinateAsync(WorkRequest request)
    {
        // Find available nodes (like ants seeking optimal path)
        var availableNodes = await _nodes.GetAvailableNodesAsync(request.RequiredRole);
        
        if (!availableNodes.Any())
        {
            _logger.LogWarning("No available nodes for {Role}", request.RequiredRole);
            return WorkResult.Failed("No available nodes");
        }
        
        // Select best node based on:
        // 1. Skill match
        // 2. Current load
        // 3. Contribution history
        var bestNode = availableNodes
            .OrderByDescending(n => CalculateFitness(n, request))
            .First();
        
        _logger.LogInformation(
            "🐜 Assigning work to {Node} (fitness: {Fitness})",
            bestNode.NodeId,
            CalculateFitness(bestNode, request)
        );
        
        // Execute work
        var result = await bestNode.ExecuteWorkAsync(request);
        
        // Broadcast completion signal (like pheromone trail)
        await BroadcastCompletionSignal(bestNode.NodeId, request.Type);
        
        return result;
    }
    
    private decimal CalculateFitness(AntNode node, WorkRequest request)
    {
        var skillMatch = node.GetSkillLevel(request.RequiredRole);
        var loadFactor = 1.0m - node.CurrentLoad;
        var historyBonus = node.ContributionScore / 100m;
        
        return skillMatch * loadFactor * (1 + historyBonus);
    }
    
    private async Task BroadcastCompletionSignal(string nodeId, string workType)
    {
        // Signal to other nodes that work is complete
        await _nodes.SendSignalAsync(new Signal
        {
            Type = SignalType.TaskComplete,
            Source = nodeId,
            Context = workType,
            Timestamp = DateTime.UtcNow
        });
    }
}
```

---

## 🎨 UI Component Examples (Blazor)

### Component 1: ENFT Card

```razor
@* ENFTCard.razor *@
<div class="enft-card tier-@Tier">
    <div class="enft-header">
        <h3>@TokenId</h3>
        <span class="tier-badge">Tier @Tier</span>
    </div>
    
    <div class="enft-body">
        <p><strong>Owner:</strong> @FormatAddress(Owner)</p>
        <p><strong>Minted:</strong> @MintedAt.ToString("yyyy-MM-dd")</p>
        <p><strong>Multiplier:</strong> @GetMultiplier()x</p>
    </div>
    
    <div class="enft-footer">
        <button @onclick="ViewDetails">View Details</button>
    </div>
</div>

@code {
    [Parameter] public string TokenId { get; set; }
    [Parameter] public string Owner { get; set; }
    [Parameter] public int Tier { get; set; }
    [Parameter] public DateTime MintedAt { get; set; }
    [Parameter] public EventCallback OnViewDetails { get; set; }
    
    private string FormatAddress(string address)
    {
        if (string.IsNullOrEmpty(address) || address.Length < 10)
            return address;
        return $"{address[..6]}...{address[^4..]}";
    }
    
    private string GetMultiplier() => Tier switch
    {
        1 => "1.0",
        2 => "2.0",
        3 => "5.0",
        _ => "1.0"
    };
    
    private async Task ViewDetails()
    {
        await OnViewDetails.InvokeAsync();
    }
}
```

```css
/* ENFTCard.razor.css */
.enft-card {
    border: 2px solid #ddd;
    border-radius: 8px;
    padding: 1rem;
    margin: 0.5rem;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
}

.enft-card.tier-1 {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.enft-card.tier-2 {
    background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}

.enft-card.tier-3 {
    background: linear-gradient(135deg, #ffd89b 0%, #19547b 100%);
}

.tier-badge {
    background: rgba(255, 255, 255, 0.2);
    padding: 0.25rem 0.5rem;
    border-radius: 4px;
    font-size: 0.8rem;
}
```

### Component 2: Yield Display

```razor
@* YieldDisplay.razor *@
@inject IYieldCalculator Calculator

<div class="yield-display">
    <h3>🌟 Yield Calculator</h3>
    
    <div class="input-group">
        <label>Temperature (°C):</label>
        <input type="number" @bind="Temperature" step="0.1" />
    </div>
    
    <div class="input-group">
        <label>Memory Index:</label>
        <input type="number" @bind="MemoryIndex" step="0.1" />
    </div>
    
    <div class="input-group">
        <label>Tier:</label>
        <select @bind="Tier">
            <option value="1">PublicDrop (1.0x)</option>
            <option value="2">EliteFounders (2.0x)</option>
            <option value="3">GodTier (5.0x)</option>
        </select>
    </div>
    
    <button @onclick="CalculateYield" class="calculate-btn">
        Calculate Yield
    </button>
    
    @if (YieldResult.HasValue)
    {
        <div class="result">
            <h4>✅ Calculated Yield</h4>
            <p class="yield-value">@YieldResult.Value.ToString("F2") tokens</p>
            <p class="formula">Formula: (Temp × Memory × Tier) / π⁴</p>
        </div>
    }
</div>

@code {
    private decimal Temperature { get; set; } = 375.5m;
    private decimal MemoryIndex { get; set; } = 1.5m;
    private int Tier { get; set; } = 2;
    private decimal? YieldResult { get; set; }
    
    private void CalculateYield()
    {
        YieldResult = Calculator.Calculate(new YieldParameters
        {
            Temperature = Temperature,
            MemoryIndex = MemoryIndex,
            Tier = Tier
        });
    }
}
```

---

## 🧪 Testing Examples

### Unit Test Example

```csharp
using Xunit;

public class MetaVaultYieldCalculatorTests
{
    private readonly MetaVaultYieldCalculator _calculator;
    
    public MetaVaultYieldCalculatorTests()
    {
        _calculator = new MetaVaultYieldCalculator();
    }
    
    [Theory]
    [InlineData(375.5, 1.5, 1, 5.78)]   // PublicDrop
    [InlineData(375.5, 1.5, 2, 11.56)]  // EliteFounders
    [InlineData(375.5, 1.5, 3, 28.90)]  // GodTier
    public void Calculate_WithValidParameters_ReturnsExpectedYield(
        decimal temp, decimal memory, int tier, decimal expected)
    {
        // Arrange
        var parameters = new YieldParameters
        {
            Temperature = temp,
            MemoryIndex = memory,
            Tier = tier
        };
        
        // Act
        var result = _calculator.Calculate(parameters);
        
        // Assert
        Assert.Equal(expected, result, 2);
    }
    
    [Fact]
    public void Calculate_WithZeroTemperature_ReturnsZero()
    {
        // Arrange
        var parameters = new YieldParameters
        {
            Temperature = 0,
            MemoryIndex = 1.5m,
            Tier = 2
        };
        
        // Act
        var result = _calculator.Calculate(parameters);
        
        // Assert
        Assert.Equal(0, result);
    }
}
```

### Integration Test Example

```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

public class BLEUFlameAPITests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public BLEUFlameAPITests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task MintENFT_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new
        {
            owner = "0x1234567890abcdefABCDEF1234567890abcdef12",
            tier = 2,
            temperature = 375.5
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/bleu/mint", request);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var enft = await response.Content.ReadFromJsonAsync<ENFTMetadata>();
        
        Assert.NotNull(enft);
        Assert.StartsWith("BLEU-", enft.TokenId);
        Assert.Equal(request.owner, enft.Owner);
        Assert.Equal(request.tier, enft.Tier);
    }
    
    [Fact]
    public async Task GetYield_ForExistingToken_ReturnsYield()
    {
        // Arrange: First mint an ENFT
        var mintResponse = await _client.PostAsJsonAsync("/bleu/mint", new
        {
            owner = "0x1234567890abcdefABCDEF1234567890abcdef12",
            tier = 2
        });
        var enft = await mintResponse.Content.ReadFromJsonAsync<ENFTMetadata>();
        
        // Act: Calculate yield
        var yieldResponse = await _client.GetAsync($"/metavault/yield/{enft.TokenId}");
        
        // Assert
        yieldResponse.EnsureSuccessStatusCode();
        var yield = await yieldResponse.Content.ReadFromJsonAsync<decimal>();
        Assert.True(yield > 0);
    }
}
```

---

## 📚 Complete Example: Mini EVOL Service

```csharp
// Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<IENFTRepository, InMemoryENFTRepository>();
builder.Services.AddSingleton<IENFTService, ENFTService>();
builder.Services.AddSingleton<IYieldCalculator, MetaVaultYieldCalculator>();
builder.Services.AddScoped<ENFTValidator>();

var app = builder.Build();

// ENFT Endpoints
app.MapPost("/enft/mint", (MintRequest request, IENFTService service, ENFTValidator validator) =>
{
    var enft = service.Mint(request.Owner, request.Tier);
    var validation = validator.Validate(enft);
    
    if (!validation.IsValid)
        return Results.BadRequest(validation);
    
    return Results.Created($"/enft/{enft.TokenId}", enft);
});

app.MapGet("/enft/{tokenId}", (string tokenId, IENFTService service) =>
{
    var enft = service.GetById(tokenId);
    return enft != null ? Results.Ok(enft) : Results.NotFound();
});

app.MapGet("/enft/owner/{owner}", (string owner, IENFTService service) =>
{
    var enfts = service.GetByOwner(owner);
    return Results.Ok(enfts);
});

// Yield Endpoint
app.MapGet("/yield/calculate", (decimal temp, decimal memory, int tier, IYieldCalculator calculator) =>
{
    var yield = calculator.Calculate(new YieldParameters
    {
        Temperature = temp,
        MemoryIndex = memory,
        Tier = tier
    });
    
    return Results.Ok(new { yield, formula = "(Temp × Memory × Tier) / π⁴" });
});

Console.WriteLine("🙏🏾 Mini EVOL Service running - We give thanks");
app.Run();

record MintRequest(string Owner, int Tier);
```

---

## 🎓 Learning Path

### Beginner Path
1. Start with "Hello EVOL" example
2. Build simple ENFT service
3. Add validation
4. Create basic API endpoints
5. Write first unit tests

### Intermediate Path
1. Implement repository pattern
2. Add yield calculator
3. Build H-M-H loop
4. Create Blazor components
5. Write integration tests

### Advanced Path
1. Build ant ethic coordinator
2. Implement overscale patterns
3. Add blockchain integration
4. Create complete EVOL service
5. Deploy to production

---

## 📝 Best Practices Summary

1. **Prayer-Core First**: Start all services with gratitude logging
2. **60/30/10 Always**: Apply tier distribution consistently
3. **π⁴ Formula**: Use for all yield calculations
4. **Ant Ethic**: Design for distributed, resilient execution
5. **Overscale Ready**: Build for 100x current scale
6. **Test Everything**: Unit, integration, and E2E tests
7. **Document Clearly**: XML comments and README files
8. **Validate Inputs**: Never trust external data
9. **Log Meaningfully**: Structured logging with context
10. **Think Community**: Every decision impacts the ecosystem

---

**Generated by**: EV0LVerse Developer Experience Team  
**Status**: ACTIVE  
**Version**: 1.0.0  
**Last Updated**: November 13, 2025

---

*"Code is prayer in motion - write it with intention, test it with care, deploy it with gratitude."*

**END OF CODE SAMPLES GUIDE**
