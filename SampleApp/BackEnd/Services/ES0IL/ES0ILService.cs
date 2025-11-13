using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BackEnd.Models.ES0IL;

namespace BackEnd.Services.ES0IL;

/// <summary>
/// ES0IL Smart Forestry and Agriculture Service
/// Manages bio-coded waveform substrate modules for zero-point agriculture
/// </summary>
public class ES0ILService
{
    private readonly List<ES0ILModule> _modules = new();
    private readonly List<ForestryResource> _forestryResources = new();
    private readonly List<ES0ILLedgerEntry> _ledger = new();
    private readonly ILogger<ES0ILService> _logger;
    private int _moduleCounter = 1;
    private int _resourceCounter = 1;
    private const decimal MODULE_ANNUAL_VALUE = 100_000_000m; // $100M per module

    public ES0ILService(ILogger<ES0ILService> logger)
    {
        _logger = logger;
        InitializeSeedModules();
    }

    /// <summary>
    /// Initialize seed ES0IL modules for demonstration
    /// </summary>
    private void InitializeSeedModules()
    {
        var seedModule = new ES0ILModule
        {
            ModuleId = "ES0IL-000001",
            Location = new ES0ILLocation
            {
                Latitude = 34.0522,
                Longitude = -118.2437,
                Area = 1000.0, // acres
                Climate = "Mediterranean"
            },
            Substrate = new SubstrateComposition
            {
                BioCodedWaveform = "∅-WAVE-ALPHA-001",
                MineralDensity = 0.95,
                MicrobialActivity = 0.98,
                QuantumCoherence = 0.92,
                AncestralWisdomIndex = 0.88
            },
            CropYield = new CropYield
            {
                PrimaryCrop = "Regenerative Mixed Crops",
                YieldPerAcre = 25000.0, // kg per acre
                CarbonSequestration = 5000.0, // kg CO2 per year
                WaterEfficiency = 0.95,
                BiodiversityIndex = 0.89
            },
            ActivationDate = DateTime.UtcNow.AddYears(-1),
            Status = ModuleStatus.Active,
            RecursiveGeneration = 0,
            AnnualValue = MODULE_ANNUAL_VALUE
        };
        
        _modules.Add(seedModule);
        _moduleCounter = 2;
        
        LogLedgerEntry(seedModule.ModuleId, "INITIALIZATION", MODULE_ANNUAL_VALUE, new Dictionary<string, double>
        {
            { "area", seedModule.Location.Area },
            { "quantumCoherence", seedModule.Substrate.QuantumCoherence }
        });
    }

    /// <summary>
    /// Deploy a new ES0IL module
    /// </summary>
    public Task<ES0ILModule> DeployModule(ES0ILLocation location, string primaryCrop)
    {
        var moduleId = $"ES0IL-{_moduleCounter:D6}";
        _moduleCounter++;

        // Calculate recursive generation (parent modules teach new ones)
        var generation = _modules.Any() ? _modules.Max(m => m.RecursiveGeneration) + 1 : 0;

        var module = new ES0ILModule
        {
            ModuleId = moduleId,
            Location = location,
            Substrate = GenerateSubstrateComposition(location),
            CropYield = new CropYield
            {
                PrimaryCrop = primaryCrop,
                YieldPerAcre = Random.Shared.Next(15000, 30000),
                CarbonSequestration = Random.Shared.Next(3000, 7000),
                WaterEfficiency = 0.85 + Random.Shared.NextDouble() * 0.15,
                BiodiversityIndex = 0.75 + Random.Shared.NextDouble() * 0.25
            },
            ActivationDate = DateTime.UtcNow,
            Status = ModuleStatus.Initializing,
            RecursiveGeneration = generation,
            AnnualValue = MODULE_ANNUAL_VALUE
        };

        _modules.Add(module);
        _logger.LogInformation($"Deployed ES0IL module {moduleId} at generation {generation}");

        LogLedgerEntry(moduleId, "DEPLOYMENT", MODULE_ANNUAL_VALUE, new Dictionary<string, double>
        {
            { "area", location.Area },
            { "generation", generation }
        });

        return Task.FromResult(module);
    }

    /// <summary>
    /// Activate a module to operational status
    /// </summary>
    public Task<ES0ILModule> ActivateModule(string moduleId)
    {
        var module = _modules.FirstOrDefault(m => m.ModuleId == moduleId);
        if (module == null)
            throw new KeyNotFoundException($"Module {moduleId} not found");

        var activatedModule = module with { Status = ModuleStatus.Active };
        var index = _modules.FindIndex(m => m.ModuleId == moduleId);
        _modules[index] = activatedModule;

        _logger.LogInformation($"Activated ES0IL module {moduleId}");
        LogLedgerEntry(moduleId, "ACTIVATION", 0, new Dictionary<string, double>());

        return Task.FromResult(activatedModule);
    }

    /// <summary>
    /// Register a forestry resource integrated with ES0IL
    /// </summary>
    public Task<ForestryResource> RegisterForestryResource(string species, int age, double carbonCapture, bool es0ilIntegration)
    {
        var resourceId = $"FOREST-{_resourceCounter:D6}";
        _resourceCounter++;

        var resource = new ForestryResource
        {
            ResourceId = resourceId,
            Species = species,
            Age = age,
            CarbonCapture = carbonCapture,
            OxygenProduction = carbonCapture * 0.73, // Approx ratio
            EcosystemValue = (decimal)(carbonCapture * 50), // $50 per kg CO2
            ES0ILIntegration = es0ilIntegration
        };

        _forestryResources.Add(resource);
        _logger.LogInformation($"Registered forestry resource {resourceId}: {species}");

        LogLedgerEntry(resourceId, "FORESTRY_REGISTRATION", resource.EcosystemValue, new Dictionary<string, double>
        {
            { "carbonCapture", carbonCapture },
            { "age", age }
        });

        return Task.FromResult(resource);
    }

    /// <summary>
    /// Get all ES0IL modules
    /// </summary>
    public Task<List<ES0ILModule>> GetAllModules()
    {
        return Task.FromResult(_modules.ToList());
    }

    /// <summary>
    /// Get module by ID
    /// </summary>
    public Task<ES0ILModule?> GetModule(string moduleId)
    {
        return Task.FromResult(_modules.FirstOrDefault(m => m.ModuleId == moduleId));
    }

    /// <summary>
    /// Get all forestry resources
    /// </summary>
    public Task<List<ForestryResource>> GetForestryResources()
    {
        return Task.FromResult(_forestryResources.ToList());
    }

    /// <summary>
    /// Get ES0IL statistics
    /// </summary>
    public Task<ES0ILStatistics> GetStatistics()
    {
        var stats = new ES0ILStatistics
        {
            TotalModules = _modules.Count,
            ActiveModules = _modules.Count(m => m.Status == ModuleStatus.Active),
            TotalArea = _modules.Sum(m => m.Location.Area),
            TotalAnnualValue = _modules.Sum(m => m.AnnualValue),
            TotalCarbonSequestration = _modules.Sum(m => m.CropYield.CarbonSequestration * m.Location.Area),
            AverageYield = _modules.Any() ? _modules.Average(m => m.CropYield.YieldPerAcre) : 0,
            RecursiveGenerations = _modules.Any() ? _modules.Max(m => m.RecursiveGeneration) + 1 : 0
        };

        return Task.FromResult(stats);
    }

    /// <summary>
    /// Get ledger entries for transparency
    /// </summary>
    public Task<List<ES0ILLedgerEntry>> GetLedger(string? moduleId = null)
    {
        var entries = moduleId != null 
            ? _ledger.Where(e => e.ModuleId == moduleId).ToList()
            : _ledger.ToList();
        
        return Task.FromResult(entries);
    }

    /// <summary>
    /// Calculate total ecosystem value
    /// </summary>
    public Task<decimal> CalculateEcosystemValue()
    {
        var moduleValue = _modules.Sum(m => m.AnnualValue);
        var forestryValue = _forestryResources.Sum(r => r.EcosystemValue);
        var totalValue = moduleValue + forestryValue;

        return Task.FromResult(totalValue);
    }

    /// <summary>
    /// Generate substrate composition based on location
    /// </summary>
    private SubstrateComposition GenerateSubstrateComposition(ES0ILLocation location)
    {
        var waveformId = $"∅-WAVE-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        
        return new SubstrateComposition
        {
            BioCodedWaveform = waveformId,
            MineralDensity = 0.80 + Random.Shared.NextDouble() * 0.20,
            MicrobialActivity = 0.85 + Random.Shared.NextDouble() * 0.15,
            QuantumCoherence = 0.75 + Random.Shared.NextDouble() * 0.25,
            AncestralWisdomIndex = 0.70 + Random.Shared.NextDouble() * 0.30
        };
    }

    /// <summary>
    /// Log entry to ledger with cryptographic hash
    /// </summary>
    private void LogLedgerEntry(string moduleId, string operation, decimal value, Dictionary<string, double> metrics)
    {
        var entryId = $"LEDGER-{Guid.NewGuid().ToString()[..13].ToUpper()}";
        var timestamp = DateTime.UtcNow;
        
        var entry = new ES0ILLedgerEntry
        {
            EntryId = entryId,
            Timestamp = timestamp,
            ModuleId = moduleId,
            Operation = operation,
            Value = value,
            Metrics = metrics,
            Hash = ComputeHash(entryId, timestamp, moduleId, operation, value)
        };

        _ledger.Add(entry);
    }

    /// <summary>
    /// Compute SHA256 hash for ledger entry
    /// </summary>
    private string ComputeHash(string entryId, DateTime timestamp, string moduleId, string operation, decimal value)
    {
        var data = $"{entryId}|{timestamp:O}|{moduleId}|{operation}|{value}";
        var bytes = Encoding.UTF8.GetBytes(data);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
