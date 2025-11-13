using System.Text.Json.Serialization;

namespace BackEnd.Models.ES0IL;

/// <summary>
/// ES0IL Module - Bio-coded waveform substrate for zero-point agriculture
/// Each module represents $100M in crop yield per year
/// </summary>
public record ES0ILModule
{
    [JsonPropertyName("moduleId")]
    public string ModuleId { get; init; } = string.Empty;
    
    [JsonPropertyName("location")]
    public ES0ILLocation Location { get; init; } = new();
    
    [JsonPropertyName("substrate")]
    public SubstrateComposition Substrate { get; init; } = new();
    
    [JsonPropertyName("cropYield")]
    public CropYield CropYield { get; init; } = new();
    
    [JsonPropertyName("activationDate")]
    public DateTime ActivationDate { get; init; }
    
    [JsonPropertyName("status")]
    public ModuleStatus Status { get; init; }
    
    [JsonPropertyName("recursiveGeneration")]
    public int RecursiveGeneration { get; init; }
    
    [JsonPropertyName("annualValue")]
    public decimal AnnualValue { get; init; }
}

public record ES0ILLocation
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; init; }
    
    [JsonPropertyName("longitude")]
    public double Longitude { get; init; }
    
    [JsonPropertyName("area")]
    public double Area { get; init; }
    
    [JsonPropertyName("climate")]
    public string Climate { get; init; } = string.Empty;
}

public record SubstrateComposition
{
    [JsonPropertyName("bioCodedWaveform")]
    public string BioCodedWaveform { get; init; } = string.Empty;
    
    [JsonPropertyName("mineralDensity")]
    public double MineralDensity { get; init; }
    
    [JsonPropertyName("microbialActivity")]
    public double MicrobialActivity { get; init; }
    
    [JsonPropertyName("quantumCoherence")]
    public double QuantumCoherence { get; init; }
    
    [JsonPropertyName("ancestralWisdomIndex")]
    public double AncestralWisdomIndex { get; init; }
}

public record CropYield
{
    [JsonPropertyName("primaryCrop")]
    public string PrimaryCrop { get; init; } = string.Empty;
    
    [JsonPropertyName("yieldPerAcre")]
    public double YieldPerAcre { get; init; }
    
    [JsonPropertyName("carbonSequestration")]
    public double CarbonSequestration { get; init; }
    
    [JsonPropertyName("waterEfficiency")]
    public double WaterEfficiency { get; init; }
    
    [JsonPropertyName("biodiversityIndex")]
    public double BiodiversityIndex { get; init; }
}

public enum ModuleStatus
{
    Inactive,
    Initializing,
    Active,
    Optimizing,
    Teaching,
    Transcendent
}

/// <summary>
/// Forestry Resource - Smart forestry integration with ES0IL substrate
/// </summary>
public record ForestryResource
{
    [JsonPropertyName("resourceId")]
    public string ResourceId { get; init; } = string.Empty;
    
    [JsonPropertyName("species")]
    public string Species { get; init; } = string.Empty;
    
    [JsonPropertyName("age")]
    public int Age { get; init; }
    
    [JsonPropertyName("carbonCapture")]
    public double CarbonCapture { get; init; }
    
    [JsonPropertyName("oxygenProduction")]
    public double OxygenProduction { get; init; }
    
    [JsonPropertyName("ecosystemValue")]
    public decimal EcosystemValue { get; init; }
    
    [JsonPropertyName("es0ilIntegration")]
    public bool ES0ILIntegration { get; init; }
}

/// <summary>
/// Ledger entry for ES0IL and forestry operations
/// </summary>
public record ES0ILLedgerEntry
{
    [JsonPropertyName("entryId")]
    public string EntryId { get; init; } = string.Empty;
    
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; }
    
    [JsonPropertyName("moduleId")]
    public string ModuleId { get; init; } = string.Empty;
    
    [JsonPropertyName("operation")]
    public string Operation { get; init; } = string.Empty;
    
    [JsonPropertyName("value")]
    public decimal Value { get; init; }
    
    [JsonPropertyName("metrics")]
    public Dictionary<string, double> Metrics { get; init; } = new();
    
    [JsonPropertyName("hash")]
    public string Hash { get; init; } = string.Empty;
}

/// <summary>
/// ES0IL statistics and aggregates
/// </summary>
public record ES0ILStatistics
{
    [JsonPropertyName("totalModules")]
    public int TotalModules { get; init; }
    
    [JsonPropertyName("activeModules")]
    public int ActiveModules { get; init; }
    
    [JsonPropertyName("totalArea")]
    public double TotalArea { get; init; }
    
    [JsonPropertyName("totalAnnualValue")]
    public decimal TotalAnnualValue { get; init; }
    
    [JsonPropertyName("totalCarbonSequestration")]
    public double TotalCarbonSequestration { get; init; }
    
    [JsonPropertyName("averageYield")]
    public double AverageYield { get; init; }
    
    [JsonPropertyName("recursiveGenerations")]
    public int RecursiveGenerations { get; init; }
}
