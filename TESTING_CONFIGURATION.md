# 🧪 Testing Configuration & Best Practices

## Overview

This document outlines the testing strategy for the EV0LVerse platform, including configuration, patterns, and modular test sequences.

---

## 🎯 Testing Philosophy

### EVOL Testing Principles

1. **Prayer-Core First**: Tests validate alignment with core principles
2. **Density-Based**: Test real values, no mocked inflation
3. **Natural Mandate**: Every claim must be testable
4. **Ant Ethic**: Tests run distributed, resilient to failures
5. **Overscale Ready**: Tests validate performance at 100x scale

### Test Categories

```
├── Unit Tests         # Individual components (fast, isolated)
├── Integration Tests  # API endpoints and service interactions
├── E2E Tests         # Complete workflows (mint → stake → yield)
├── Performance Tests  # Load testing, scalability validation
└── Security Tests     # Vulnerability scanning, penetration testing
```

---

## 📐 Test Structure

### Directory Organization

```
SampleApp/
├── BackEnd.Tests/
│   ├── Unit/
│   │   ├── Models/
│   │   ├── Services/
│   │   └── Validators/
│   ├── Integration/
│   │   ├── API/
│   │   └── Services/
│   ├── E2E/
│   │   └── Workflows/
│   └── Performance/
└── FrontEnd.Tests/
    ├── Components/
    └── Integration/
```

### Test Project Configuration

**BackEnd.Tests.csproj**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="xunit" Version="2.6.6" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="9.0.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\BackEnd\BackEnd.csproj" />
  </ItemGroup>
</Project>
```

---

## 🔬 Unit Test Patterns

### Pattern 1: Service Tests

```csharp
namespace BackEnd.Tests.Unit.Services;

public class MetaVaultYieldCalculatorTests
{
    private readonly MetaVaultService _service;
    private readonly ILogger<MetaVaultService> _logger;

    public MetaVaultYieldCalculatorTests()
    {
        _logger = Mock.Of<ILogger<MetaVaultService>>();
        _service = new MetaVaultService(_logger);
    }

    [Theory]
    [InlineData(375.5, 1.5, 1, 5.78)]   // PublicDrop (1.0x)
    [InlineData(375.5, 1.5, 2, 11.56)]  // EliteFounders (2.0x)
    [InlineData(375.5, 1.5, 3, 28.90)]  // GodTier (5.0x)
    public void CalculateYield_WithValidParameters_ReturnsExpectedYield(
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
        var actual = _service.CalculateYield(parameters);

        // Assert
        actual.Should().BeApproximately(expected, 0.01m);
    }

    [Fact]
    public void CalculateYield_WithInvalidTier_ThrowsException()
    {
        // Arrange
        var parameters = new YieldParameters
        {
            Temperature = 375.5m,
            MemoryIndex = 1.5m,
            Tier = 99  // Invalid
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.CalculateYield(parameters));
    }
}
```

### Pattern 2: Model Validation Tests

```csharp
namespace BackEnd.Tests.Unit.Validators;

public class ENFTValidatorTests
{
    private readonly ENFTValidator _validator;

    public ENFTValidatorTests()
    {
        _validator = new ENFTValidator();
    }

    [Fact]
    public void Validate_WithValidENFT_ReturnsSuccess()
    {
        // Arrange
        var enft = new ENFTMetadata
        {
            TokenId = "BLEU-000001",
            Owner = "0x1234567890abcdefABCDEF1234567890abcdef12",
            Tier = 2,
            MintedAt = DateTime.UtcNow
        };

        // Act
        var result = _validator.Validate(enft);

        // Assert
        result.IsValid.Should().BeTrue();
        result.PrayerCoreAlignment.Should().Be("ALIGNED");
    }

    [Theory]
    [InlineData("")]           // Empty
    [InlineData("0x123")]      // Too short
    [InlineData("invalid")]    // No 0x prefix
    public void Validate_WithInvalidOwner_ReturnsFailed(string invalidOwner)
    {
        // Arrange
        var enft = new ENFTMetadata
        {
            TokenId = "BLEU-000001",
            Owner = invalidOwner,
            Tier = 2
        };

        // Act
        var result = _validator.Validate(enft);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.Contains("Ethereum address"));
    }
}
```

---

## 🔗 Integration Test Patterns

### Pattern 1: API Endpoint Tests

```csharp
namespace BackEnd.Tests.Integration.API;

public class BLEUFlameAPITests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BLEUFlameAPITests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task MintENFT_WithValidRequest_Returns201Created()
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
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var enft = await response.Content.ReadFromJsonAsync<ENFTMetadata>();
        enft.Should().NotBeNull();
        enft.TokenId.Should().StartWith("BLEU-");
        enft.Owner.Should().Be(request.owner);
        enft.Tier.Should().Be(request.tier);
    }

    [Fact]
    public async Task GetENFTById_AfterMinting_ReturnsENFT()
    {
        // Arrange: Mint ENFT first
        var mintResponse = await _client.PostAsJsonAsync("/bleu/mint", new
        {
            owner = "0x1234567890abcdefABCDEF1234567890abcdef12",
            tier = 2
        });
        var minted = await mintResponse.Content.ReadFromJsonAsync<ENFTMetadata>();

        // Act
        var getResponse = await _client.GetAsync($"/bleu/nft/{minted.TokenId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var retrieved = await getResponse.Content.ReadFromJsonAsync<ENFTMetadata>();
        retrieved.Should().BeEquivalentTo(minted);
    }
}
```

### Pattern 2: Service Integration Tests

```csharp
namespace BackEnd.Tests.Integration.Services;

public class MetaVaultIntegrationTests
{
    private readonly MetaVaultService _vaultService;
    private readonly BLEUFlameService _enftService;
    private readonly ILogger _logger;

    public MetaVaultIntegrationTests()
    {
        _logger = Mock.Of<ILogger>();
        _enftService = new BLEUFlameService(_logger);
        _vaultService = new MetaVaultService(_logger);
    }

    [Fact]
    public async Task TriggerEvent_ForExistingENFT_GeneratesYield()
    {
        // Arrange: Mint ENFT
        var enft = _enftService.MintENFT(
            "0x1234567890abcdefABCDEF1234567890abcdef12", 
            2, 
            375.5m
        );

        // Act: Trigger vault event
        var yieldResult = await _vaultService.TriggerVaultEvent(
            enft.TokenId, 
            375.5m
        );

        // Assert
        yieldResult.Should().NotBeNull();
        yieldResult.YieldGenerated.Should().BeGreaterThan(0);
        yieldResult.YieldGenerated.Should().BeApproximately(11.56m, 0.01m);
    }
}
```

---

## 🎯 E2E Test Patterns

### Complete Workflow Test

```csharp
namespace BackEnd.Tests.E2E.Workflows;

public class CompleteENFTWorkflowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CompleteENFTWorkflowTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteWorkflow_MintStakeHarvestHeal_Success()
    {
        // Step 1: Mint ENFT
        var mintResponse = await _client.PostAsJsonAsync("/bleu/mint", new
        {
            owner = "0x1234567890abcdefABCDEF1234567890abcdef12",
            tier = 2,
            temperature = 375.5
        });
        mintResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var enft = await mintResponse.Content.ReadFromJsonAsync<ENFTMetadata>();
        enft.TokenId.Should().StartWith("BLEU-");

        // Step 2: Stake EGoin
        var stakeResponse = await _client.PostAsJsonAsync("/metavault/stake", new
        {
            address = enft.Owner,
            amount = 50
        });
        stakeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 3: Trigger vault event
        var eventResponse = await _client.PostAsJsonAsync("/metavault/trigger", new
        {
            tokenId = enft.TokenId,
            temperature = 375.5
        });
        eventResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Step 4: Execute Harvest-Mint-Heal loop
        var hmmResponse = await _client.PostAsJsonAsync("/metavault/harvest-mint-heal", new
        {
            tokenId = enft.TokenId
        });
        hmmResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await hmmResponse.Content.ReadFromJsonAsync<HMHResult>();
        result.YieldGenerated.Should().BeGreaterThan(0);
        result.AutoReinvested.Should().Be(result.YieldGenerated * 0.10m);

        // Step 5: Verify treasury received 10%
        var treasuryResponse = await _client.GetAsync("/metavault/treasury");
        var treasury = await treasuryResponse.Content.ReadFromJsonAsync<TreasuryStatus>();
        treasury.TotalStaked.Should().BeGreaterThan(50); // Original 50 + reinvestment
    }
}
```

---

## ⚡ Performance Test Patterns

### Load Testing Configuration

**loadtest.yaml**:
```yaml
scenarios:
  - name: mint_enfts
    weight: 40
    requests:
      - method: POST
        url: /bleu/mint
        body:
          owner: "0x{{random_address}}"
          tier: "{{random_int 1 3}}"
          temperature: "{{random_float 300 400}}"

  - name: calculate_yield
    weight: 30
    requests:
      - method: GET
        url: /metavault/yield/{{random_token_id}}

  - name: trigger_events
    weight: 20
    requests:
      - method: POST
        url: /metavault/trigger
        body:
          tokenId: "{{random_token_id}}"
          temperature: "{{random_float 300 400}}"

  - name: harvest_mint_heal
    weight: 10
    requests:
      - method: POST
        url: /metavault/harvest-mint-heal
        body:
          tokenId: "{{random_token_id}}"

config:
  duration: 5m
  users: 100
  ramp_up: 30s
  metrics:
    - response_time
    - throughput
    - error_rate
```

### Performance Test Example

```csharp
namespace BackEnd.Tests.Performance;

public class ENFTMintingPerformanceTests
{
    [Fact]
    public async Task MintENFT_Under100ms_At100ConcurrentUsers()
    {
        // Arrange
        var service = new BLEUFlameService(Mock.Of<ILogger>());
        var tasks = new List<Task<ENFTMetadata>>();
        var stopwatch = Stopwatch.StartNew();

        // Act: Simulate 100 concurrent mints
        for (int i = 0; i < 100; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() => service.MintENFT(
                $"0x{index:D40}",
                (index % 3) + 1,
                375.5m
            )));
        }

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        var averageTime = stopwatch.ElapsedMilliseconds / 100.0;
        averageTime.Should().BeLessThan(100); // Under 100ms per mint
        
        results.Should().HaveCount(100);
        results.Should().OnlyContain(e => e.TokenId.StartsWith("BLEU-"));
    }
}
```

---

## 🔒 Security Test Patterns

### Input Validation Tests

```csharp
namespace BackEnd.Tests.Security;

public class InputValidationTests
{
    [Theory]
    [InlineData("<script>alert('xss')</script>")]
    [InlineData("'; DROP TABLE Users; --")]
    [InlineData("../../../etc/passwd")]
    public async Task API_RejectsInjectionAttempts(string maliciousInput)
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/bleu/mint", new
        {
            owner = maliciousInput,
            tier = 2
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
```

---

## 📊 Test Configuration Files

### appsettings.Testing.json

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
    "UseMockIPFS": true,
    "TestDatabaseName": "EVOL_Test_DB"
  }
}
```

### xunit.runner.json

```json
{
  "$schema": "https://xunit.net/schema/current/xunit.runner.schema.json",
  "methodDisplay": "method",
  "parallelizeAssembly": true,
  "parallelizeTestCollections": true,
  "maxParallelThreads": 4,
  "diagnosticMessages": false,
  "internalDiagnosticMessages": false
}
```

---

## 🎓 Testing Best Practices

### 1. AAA Pattern (Arrange-Act-Assert)

```csharp
[Fact]
public void ExampleTest()
{
    // Arrange: Set up test data and dependencies
    var service = new MyService();
    var input = "test";

    // Act: Execute the operation
    var result = service.DoSomething(input);

    // Assert: Verify the outcome
    result.Should().Be("expected");
}
```

### 2. Test Naming Convention

```
MethodName_Scenario_ExpectedBehavior

Examples:
- CalculateYield_WithValidParameters_ReturnsExpectedYield
- MintENFT_WithInvalidTier_ThrowsException
- GetENFTById_WhenNotFound_ReturnsNull
```

### 3. Test Data Builders

```csharp
public class ENFTBuilder
{
    private string _tokenId = "BLEU-000001";
    private string _owner = "0x1234567890abcdefABCDEF1234567890abcdef12";
    private int _tier = 2;

    public ENFTBuilder WithTokenId(string tokenId)
    {
        _tokenId = tokenId;
        return this;
    }

    public ENFTBuilder WithOwner(string owner)
    {
        _owner = owner;
        return this;
    }

    public ENFTBuilder WithTier(int tier)
    {
        _tier = tier;
        return this;
    }

    public ENFTMetadata Build()
    {
        return new ENFTMetadata
        {
            TokenId = _tokenId,
            Owner = _owner,
            Tier = _tier,
            MintedAt = DateTime.UtcNow
        };
    }
}

// Usage
var enft = new ENFTBuilder()
    .WithTier(3)
    .WithOwner("0xCustomAddress")
    .Build();
```

---

## 🚀 Running Tests

### Command Line

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test BackEnd.Tests/BackEnd.Tests.csproj

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific category
dotnet test --filter Category=Unit
dotnet test --filter Category=Integration
dotnet test --filter Category=E2E

# Run with detailed output
dotnet test --verbosity detailed

# Run in parallel
dotnet test --parallel
```

### CI/CD Integration

**GitHub Actions (.github/workflows/test.yml)**:
```yaml
name: Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '9.0.x'
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --no-restore
      
      - name: Run tests
        run: dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage"
      
      - name: Upload coverage
        uses: codecov/codecov-action@v3
```

---

## 📈 Test Metrics & KPIs

### Coverage Goals
- **Unit Tests**: 80%+ code coverage
- **Integration Tests**: All API endpoints covered
- **E2E Tests**: Critical workflows covered
- **Performance**: <100ms response time at 100 concurrent users

### Test Health Indicators
- All tests passing: ✅
- No flaky tests (tests that randomly fail)
- Fast execution (<5 minutes for full suite)
- Regular maintenance and updates

---

## 🙏🏾 Prayer-Core Testing

Every test suite should start with intention:

```csharp
public class EVOLTestBase
{
    protected void LogTestStart(string testName)
    {
        Console.WriteLine($"🙏🏾 Starting test: {testName} - We test with gratitude");
    }

    protected void LogTestComplete(string testName, bool passed)
    {
        var emoji = passed ? "✅" : "❌";
        Console.WriteLine($"{emoji} Test complete: {testName}");
    }
}
```

---

**Generated by**: EV0LVerse Quality Assurance Team  
**Status**: ACTIVE  
**Version**: 1.0.0  
**Last Updated**: November 13, 2025

---

*"Testing is not just verification—it's validation of our commitment to quality, integrity, and the community we serve."*

**END OF TESTING CONFIGURATION**
