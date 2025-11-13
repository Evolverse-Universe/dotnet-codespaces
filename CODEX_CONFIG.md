# 🔧 EV0LVerse Codex/Config: Development Practices

## Overview

The **Codex/Config** system defines development standards, practices, and configurations that ensure all EV0LVerse implementations adhere to core principles while maintaining flexibility for innovation.

---

## 🎯 EVOL Core Anchors

All development must align with these foundational anchors:

### 1. **Prayer-Core Logic** 🙏🏾
- **Principle**: Wisdom and values first, then technology
- **Implementation**: Code comments reference spiritual principles
- **Practice**: Team alignment through shared values
- **Example**:
```csharp
/// <summary>
/// MetaVault yield calculation using π⁴ (divine proportion)
/// Foundation: Ancient mathematical wisdom in modern application
/// </summary>
public decimal CalculateYield(decimal temperature, decimal memoryIndex, int tier)
{
    // Prayer-core: Give thanks for abundance before calculation
    return (temperature * memoryIndex * GetTierMultiplier(tier)) / PI_TO_FOURTH;
}
```

### 2. **Density-Based Value** ⚖️
- **Principle**: Real-world measurements, no inflation
- **Implementation**: All values tied to physical anchors
- **Practice**: Weight, volume, temperature tracking
- **Example**:
```csharp
public record PhysicalAsset
{
    public decimal Weight { get; init; }      // In troy ounces
    public decimal Purity { get; init; }       // 99.99% = 0.9999
    public decimal Temperature { get; init; }  // In Celsius
    public decimal Volume { get; init; }       // In cubic cm
}
```

### 3. **Harvest-Mint-Heal Loop** 🔄
- **Principle**: Continuous value generation with auto-reinvestment
- **Implementation**: 10% automatic treasury allocation
- **Practice**: Compound growth at π⁴ rate
- **Example**:
```csharp
public class HarvestMintHeal
{
    public TokenYield Execute(string tokenId)
    {
        // HARVEST: Collect data
        var data = _service.GetThermalData(tokenId);
        
        // MINT: Generate yield
        var yield = CalculateYield(data);
        
        // HEAL: Auto-reinvest 10%
        var reinvestment = yield * 0.10m;
        _treasury.AutoReinvest(reinvestment);
        
        return new TokenYield(yield, reinvestment);
    }
}
```

### 4. **60/30/10 Distribution** 📊
- **Principle**: Balanced tier-based allocation
- **Implementation**: PublicDrop (60%), EliteFounders (30%), GodTier (10%)
- **Practice**: All revenue splits follow this pattern
- **Example**:
```csharp
public RevenueSplit CalculateSplit(decimal totalAmount)
{
    return new RevenueSplit
    {
        PublicDrop = totalAmount * 0.60m,
        EliteFounders = totalAmount * 0.30m,
        GodTier = totalAmount * 0.10m
    };
}
```

### 5. **Ancestral Integration** 🌍
- **Principle**: Ancient wisdom in modern systems
- **Implementation**: Afro-Elohim narratives, Saturn protocols
- **Practice**: Cultural context in all features
- **Example**:
```csharp
public record ZionGoldBar
{
    // Modern data
    public string TokenId { get; init; }
    public decimal Weight { get; init; }
    
    // Ancestral integration
    public string AncestralLineage { get; init; }
    public string AfroElohimNarrative { get; init; }
    public string SaturnAlignment { get; init; }
}
```

### 6. **Natural Testing Mandate** 🔬
- **Principle**: Everything testable, properly mandated
- **Implementation**: Real-world validation required
- **Practice**: Agricultural science basis, meteorology integration
- **Example**:
```csharp
[Fact]
public void ES0IL_Should_Increase_Crop_Yield_10_To_20_Percent()
{
    // Natural test: Actual agricultural measurement
    var control = MeasureCropYield(standardSoil);
    var enhanced = MeasureCropYield(es0ilSubstrate);
    
    var increase = (enhanced - control) / control;
    Assert.InRange(increase, 0.10m, 0.20m);
}
```

---

## 📐 Development Standards

### Code Organization

```
Project Structure (EVOL Standard):
├── Contracts/          # Smart contracts (Solidity)
├── Data/              # Metadata, documentation, ledgers
├── Models/            # Domain entities (records preferred)
│   ├── BLEU/         # BLEU Flame system models
│   ├── Zion/         # Zion Gold Bar models
│   └── Core/         # Shared models
├── Services/          # Business logic (single responsibility)
│   ├── BLEU/         # Service layer by domain
│   ├── Vaults/       # MetaVault services
│   └── Integration/  # External integrations
├── Tests/             # Test projects
│   ├── Unit/         # Unit tests
│   ├── Integration/  # Integration tests
│   └── E2E/          # End-to-end tests
└── Program.cs        # API endpoints and configuration
```

### Naming Conventions

**EVOL-Specific Naming**:
```csharp
// Constants: Use EVOL terminology
public const decimal PI_TO_FOURTH = 97.409091034m;
public const string BLEU_PREFIX = "BLEU-";
public const string ZION_PREFIX = "ZION-";

// Services: Domain + "Service"
public class BLEUFlameService { }
public class MetaVaultService { }
public class ZionGoldBarService { }

// Models: Clear, descriptive records
public record ENFTMetadata(string TokenId, int Tier);
public record ThermalEvent(decimal Temperature, DateTime Timestamp);

// Endpoints: Kebab-case, domain-grouped
app.MapGet("/bleu/metadata", ...);
app.MapPost("/metavault/stake", ...);
app.MapGet("/zion/layers", ...);
```

### Documentation Standards

**XML Comments (Required)**:
```csharp
/// <summary>
/// Calculates MetaVault yield using the divine proportion formula.
/// Foundation: π⁴ (pi to the fourth power) = 97.409091034
/// </summary>
/// <param name="temperature">Temperature in Celsius from thermal event</param>
/// <param name="memoryIndex">Usage multiplier based on history</param>
/// <param name="ownershipTier">1=PublicDrop, 2=EliteFounders, 3=GodTier</param>
/// <returns>Calculated yield in tokens</returns>
/// <remarks>
/// Formula: Yield = (Temperature × MemoryIndex × TierMultiplier) / π⁴
/// Prayer-core: This calculation honors the abundance principle
/// </remarks>
public decimal CalculateYield(decimal temperature, decimal memoryIndex, int ownershipTier)
{
    var tierMultiplier = GetTierMultiplier(ownershipTier);
    return (temperature * memoryIndex * tierMultiplier) / PI_TO_FOURTH;
}
```

**README Documentation**:
- Every feature requires a README in `/Data/` folder
- Include: Overview, Architecture, Usage, Examples
- Reference EVOL core anchors
- Link to related systems

### Error Handling

**EVOL Error Patterns**:
```csharp
// Custom exceptions with context
public class EVOLException : Exception
{
    public string Context { get; init; }
    public Dictionary<string, object> Metadata { get; init; }
    
    public EVOLException(string message, string context) 
        : base(message)
    {
        Context = context;
        Metadata = new();
    }
}

public class MetaVaultException : EVOLException
{
    public MetaVaultException(string message) 
        : base(message, "MetaVault") { }
}

// Usage in services
public TokenYield CalculateYield(string tokenId)
{
    if (!_registry.ContainsKey(tokenId))
    {
        throw new MetaVaultException($"Token {tokenId} not found in registry")
        {
            Metadata = { ["TokenId"] = tokenId, ["Action"] = "CalculateYield" }
        };
    }
    // ... calculation
}

// API error responses
app.MapGet("/metavault/yield/{tokenId}", (string tokenId) =>
{
    try
    {
        var yield = service.CalculateYield(tokenId);
        return Results.Ok(yield);
    }
    catch (MetaVaultException ex)
    {
        return Results.NotFound(new { 
            error = ex.Message, 
            context = ex.Context,
            metadata = ex.Metadata 
        });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            detail: "Internal error in MetaVault system",
            statusCode: 500
        );
    }
});
```

---

## 🔐 Security Configuration

### Input Validation

```csharp
public class EVOLValidator
{
    // Ethereum address validation
    public static bool IsValidEthereumAddress(string address)
    {
        return !string.IsNullOrWhiteSpace(address) 
            && address.StartsWith("0x") 
            && address.Length == 42
            && address[2..].All(c => char.IsDigit(c) || "abcdefABCDEF".Contains(c));
    }
    
    // Token ID validation
    public static bool IsValidTokenId(string tokenId, string prefix)
    {
        return !string.IsNullOrWhiteSpace(tokenId)
            && tokenId.StartsWith(prefix)
            && tokenId.Length > prefix.Length;
    }
    
    // Tier validation
    public static bool IsValidTier(int tier)
    {
        return tier >= 1 && tier <= 3;
    }
}

// Usage in endpoints
app.MapPost("/bleu/mint", (MintRequest request) =>
{
    if (!EVOLValidator.IsValidEthereumAddress(request.Owner))
        return Results.BadRequest("Invalid Ethereum address");
    
    if (!EVOLValidator.IsValidTier(request.Tier))
        return Results.BadRequest("Tier must be 1, 2, or 3");
    
    var enft = service.MintENFT(request);
    return Results.Created($"/bleu/nft/{enft.TokenId}", enft);
});
```

### Sanitization

```csharp
public class EVOLSanitizer
{
    // Sanitize for logging
    public static string SanitizeForLog(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "[empty]";
        
        // Remove control characters and limit length
        var sanitized = new string(input
            .Where(c => !char.IsControl(c))
            .Take(100)
            .ToArray());
        
        return string.IsNullOrWhiteSpace(sanitized) ? "[invalid]" : sanitized;
    }
    
    // Sanitize Ethereum address for display
    public static string SanitizeAddress(string address)
    {
        if (!EVOLValidator.IsValidEthereumAddress(address))
            return "[invalid-address]";
        
        // Show first 6 and last 4 characters
        return $"{address[..6]}...{address[^4..]}";
    }
}

// Usage in logging
_logger.LogInformation(
    "ENFT minted: {TokenId} for owner {Owner}",
    EVOLSanitizer.SanitizeForLog(tokenId),
    EVOLSanitizer.SanitizeAddress(owner)
);
```

---

## 🧪 Testing Configuration

### Test Categories

**Unit Tests** (Models, Services):
```csharp
namespace BackEnd.Tests.Unit;

public class MetaVaultYieldTests
{
    private readonly MetaVaultService _service = new();
    
    [Theory]
    [InlineData(375.5, 1.5, 2, 11.56)] // EliteFounders
    [InlineData(375.5, 1.5, 1, 5.78)]  // PublicDrop
    [InlineData(375.5, 1.5, 3, 28.90)] // GodTier
    public void CalculateYield_WithValidInput_ReturnsExpectedYield(
        decimal temp, decimal memory, int tier, decimal expected)
    {
        var result = _service.CalculateYield(temp, memory, tier);
        Assert.Equal(expected, result, 2);
    }
}
```

**Integration Tests** (API Endpoints):
```csharp
namespace BackEnd.Tests.Integration;

public class BLEUFlameAPITests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public BLEUFlameAPITests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task MintENFT_WithValidRequest_Returns201()
    {
        var request = new { owner = "0x1234567890abcdef", tier = 2 };
        var response = await _client.PostAsJsonAsync("/bleu/mint", request);
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var enft = await response.Content.ReadFromJsonAsync<ENFTMetadata>();
        Assert.NotNull(enft);
        Assert.StartsWith("BLEU-", enft.TokenId);
    }
}
```

**E2E Tests** (Full Workflows):
```csharp
namespace BackEnd.Tests.E2E;

public class HarvestMintHealWorkflowTests
{
    [Fact]
    public async Task CompleteWorkflow_MintStakeHarvestHeal_Success()
    {
        // ARRANGE: Mint ENFT
        var mintResponse = await _client.PostAsJsonAsync("/bleu/mint", 
            new { owner = "0xTestAddress", tier = 2 });
        var enft = await mintResponse.Content.ReadFromJsonAsync<ENFTMetadata>();
        
        // ACT: Stake
        await _client.PostAsJsonAsync("/metavault/stake", 
            new { address = "0xTestAddress", amount = 50 });
        
        // ACT: Trigger thermal event
        await _client.PostAsJsonAsync("/metavault/trigger", 
            new { tokenId = enft.TokenId, temperature = 375.5 });
        
        // ACT: Execute H-M-H loop
        var hhmResponse = await _client.PostAsJsonAsync("/metavault/harvest-mint-heal", 
            new { tokenId = enft.TokenId });
        
        // ASSERT: Verify yield and reinvestment
        var result = await hhmResponse.Content.ReadFromJsonAsync<HarvestMintHealResult>();
        Assert.True(result.YieldGenerated > 0);
        Assert.Equal(result.YieldGenerated * 0.10m, result.AutoReinvested);
    }
}
```

### Test Configuration

**appsettings.Testing.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "EVOL": {
    "Environment": "Testing",
    "EnableMetrics": false,
    "EnableDetailedErrors": true,
    "UseMockBlockchain": true,
    "UseMockIPFS": true
  }
}
```

---

## 🌐 API Configuration

### OpenAPI Standards

**Endpoint Documentation**:
```csharp
app.MapPost("/bleu/mint", async (MintRequest request, BLEUFlameService service) =>
{
    var enft = service.MintENFT(request.Owner, request.Tier, request.Temperature);
    return Results.Created($"/bleu/nft/{enft.TokenId}", enft);
})
.WithName("MintBLEUFlame")
.WithTags("BLEU Flame")
.WithSummary("Mint a new BLEU Flame ENFT")
.WithDescription(@"
    Creates a new Enhanced Non-Fungible Token (ENFT) in the BLEU Flame system.
    
    **Tiers**:
    - 1 = PublicDrop (60% distribution, 1.0x multiplier)
    - 2 = EliteFounders (30% distribution, 2.0x multiplier)
    - 3 = GodTier (10% distribution, 5.0x multiplier)
    
    **EVOL Anchor**: 60/30/10 distribution principle
")
.Produces<ENFTMetadata>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest)
.WithOpenApi();
```

### CORS Configuration

```csharp
// In Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("EVOLPolicy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:8081",  // Frontend
                "https://evolverse.io"     // Production
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

app.UseCors("EVOLPolicy");
```

### Rate Limiting

```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("EVOL", limiterOptions =>
    {
        limiterOptions.PermitLimit = 100;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 10;
    });
});

app.UseRateLimiter();

// Apply to endpoints
app.MapGet("/bleu/metadata", ...)
    .RequireRateLimiting("EVOL");
```

---

## 🏗️ Deployment Configuration

### Environment Configuration

**appsettings.Development.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "EVOL": {
    "Environment": "Development",
    "BlockchainNetwork": "localhost",
    "IPFSGateway": "http://localhost:5001",
    "EnableMockData": true,
    "EnableDetailedErrors": true
  }
}
```

**appsettings.Production.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "EVOL": {
    "Environment": "Production",
    "BlockchainNetwork": "mainnet",
    "IPFSGateway": "https://ipfs.infura.io:5001",
    "EnableMockData": false,
    "EnableDetailedErrors": false,
    "RequireAuthentication": true,
    "EnableRateLimiting": true
  }
}
```

### Container Configuration

**Dockerfile**:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["BackEnd/BackEnd.csproj", "BackEnd/"]
RUN dotnet restore "BackEnd/BackEnd.csproj"

# Copy source and build
COPY BackEnd/ BackEnd/
WORKDIR "/src/BackEnd"
RUN dotnet build "BackEnd.csproj" -c Release -o /app/build
RUN dotnet publish "BackEnd.csproj" -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

# EVOL configuration
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080
ENTRYPOINT ["dotnet", "BackEnd.dll"]
```

**docker-compose.yml**:
```yaml
version: '3.8'

services:
  backend:
    build:
      context: ./SampleApp
      dockerfile: BackEnd/Dockerfile
    ports:
      - "8080:8080"
    environment:
      - EVOL__Environment=Production
      - EVOL__BlockchainNetwork=mainnet
    volumes:
      - ./data:/app/data

  frontend:
    build:
      context: ./SampleApp
      dockerfile: FrontEnd/Dockerfile
    ports:
      - "8081:8081"
    environment:
      - BACKEND_URL=http://backend:8080
    depends_on:
      - backend
```

---

## 📊 Monitoring & Observability

### Logging Configuration

```csharp
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
    
    // Production: Add Application Insights
    if (builder.Environment.IsProduction())
    {
        logging.AddApplicationInsights();
    }
});

// Structured logging
public class MetaVaultService
{
    private readonly ILogger<MetaVaultService> _logger;
    
    public MetaVaultService(ILogger<MetaVaultService> logger)
    {
        _logger = logger;
    }
    
    public void TriggerVaultEvent(string tokenId, decimal temperature)
    {
        _logger.LogInformation(
            "Vault event triggered: TokenId={TokenId}, Temperature={Temperature}°C",
            tokenId, temperature
        );
        
        // ... event logic
        
        _logger.LogInformation(
            "Yield calculated: TokenId={TokenId}, Yield={Yield}",
            tokenId, yield
        );
    }
}
```

### Metrics

```csharp
// Custom metrics (using System.Diagnostics.Metrics)
public class EVOLMetrics
{
    private readonly Counter<int> _enftsMinted;
    private readonly Histogram<decimal> _yieldDistribution;
    private readonly ObservableGauge<int> _treasuryBalance;
    
    public EVOLMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("EVOL.Metrics");
        
        _enftsMinted = meter.CreateCounter<int>(
            "evol.enfts.minted",
            "Count of ENFTs minted"
        );
        
        _yieldDistribution = meter.CreateHistogram<decimal>(
            "evol.yield.distribution",
            "Distribution of yield values"
        );
        
        _treasuryBalance = meter.CreateObservableGauge<int>(
            "evol.treasury.balance",
            () => GetTreasuryBalance()
        );
    }
    
    public void RecordENFTMinted() => _enftsMinted.Add(1);
    public void RecordYield(decimal amount) => _yieldDistribution.Record(amount);
}
```

---

## 🔗 Integration Patterns

### Blockchain Integration

```csharp
public interface IBlockchainService
{
    Task<string> DeployContract(string contractCode);
    Task<TransactionReceipt> MintToken(string contractAddress, string recipient);
    Task<BigInteger> GetBalance(string address);
}

public class EthereumService : IBlockchainService
{
    private readonly Web3 _web3;
    
    public EthereumService(IConfiguration config)
    {
        var rpcUrl = config["EVOL:BlockchainRPC"];
        _web3 = new Web3(rpcUrl);
    }
    
    public async Task<string> DeployContract(string contractCode)
    {
        // Deploy smart contract
        // Return contract address
    }
    
    // ... implementation
}
```

### IPFS Integration

```csharp
public interface IIPFSService
{
    Task<string> UploadJson(object data);
    Task<string> UploadFile(byte[] content, string filename);
    Task<T> GetJson<T>(string cid);
}

public class IPFSService : IIPFSService
{
    private readonly HttpClient _client;
    
    public IPFSService(IConfiguration config)
    {
        var gateway = config["EVOL:IPFSGateway"];
        _client = new HttpClient { BaseAddress = new Uri(gateway) };
    }
    
    public async Task<string> UploadJson(object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/v0/add", content);
        
        var result = await response.Content.ReadFromJsonAsync<IPFSResponse>();
        return result.Hash; // Return CID
    }
    
    // ... implementation
}
```

---

## 🎓 Training & Onboarding

### Developer Onboarding Checklist

- [ ] Read [INVESTORS_AND_BUILDERS_GUIDE.md](INVESTORS_AND_BUILDERS_GUIDE.md)
- [ ] Review EVOL Core Anchors (this document)
- [ ] Study [BLEU_FLAME_README.md](SampleApp/BackEnd/Data/BLEU_FLAME_README.md)
- [ ] Study [ZION_GOLD_BAR_README.md](SampleApp/BackEnd/Data/ZION_GOLD_BAR_README.md)
- [ ] Set up development environment
- [ ] Build and run the solution
- [ ] Test all API endpoints via Scalar
- [ ] Review smart contract (BLEULION_TREASURY.sol)
- [ ] Complete first contribution (documentation or minor fix)
- [ ] Attend team alignment session (Prayer-core principle)

### Code Review Checklist

**EVOL Compliance**:
- [ ] Aligns with Prayer-core logic
- [ ] Uses density-based values (no inflation)
- [ ] Implements Harvest-Mint-Heal pattern (if applicable)
- [ ] Follows 60/30/10 distribution (if applicable)
- [ ] Includes ancestral integration (if applicable)
- [ ] Has natural testing mandate

**Technical Quality**:
- [ ] XML documentation complete
- [ ] Error handling comprehensive
- [ ] Input validation implemented
- [ ] OpenAPI documentation added
- [ ] Tests written (unit/integration)
- [ ] No security vulnerabilities
- [ ] Performance acceptable

---

## 📝 License & Governance

### Code License
This project follows the Microsoft Open Source Code of Conduct and uses the MIT license for code components.

### EVOL Principles License
The EVOL Core Anchors and spiritual principles are protected intellectual property of the EV0LVerse Nation and must be preserved in all derivative works.

### Governance
- **Code Changes**: Pull request with 2+ approvals
- **Architecture Changes**: Team consensus required
- **Core Anchor Changes**: Community vote required
- **Security Issues**: Immediate escalation to security team

---

**Generated by**: EV0LVerse Core Development Team  
**Status**: ACTIVE  
**Version**: 1.0.0  
**Last Updated**: November 13, 2025

---

*"We are the compats, we are the build, man, we are the constructive pieces that we need to be configurable for the build. That's iron sharpening iron."*

**END OF CODEX/CONFIG**
