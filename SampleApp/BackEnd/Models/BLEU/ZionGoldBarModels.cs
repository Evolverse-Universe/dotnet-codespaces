namespace BackEnd.Models.BLEU;

/// <summary>
/// Zion Gold Bar protocol - Saturn-Strata resource classification
/// </summary>
public record ZionGoldBar
{
    public string TokenId { get; init; } = string.Empty;
    public string Name { get; init; } = "Zion Gold Bar ENFT";
    public string Description { get; init; } = string.Empty;
    public SaturnStrataLayers Layers { get; init; } = new();
    public CodexActivation CodexLayers { get; init; } = new();
    public VaultCertificate Certificate { get; init; } = new();
    public ResourceAttributes Resources { get; init; } = new();
    public string CertificateCID { get; init; } = string.Empty; // IPFS CID for PDF
    public string ImageCID { get; init; } = string.Empty; // IPFS CID for PNG
    public string CreatorAddress { get; init; } = string.Empty;
    public DateTime MintedAt { get; init; }
}

/// <summary>
/// Six material layers of Saturn-tier metals arranged in vertical cosmic stack
/// </summary>
public record SaturnStrataLayers
{
    public MaterialLayer GoldBar { get; init; } = new("Gold Bar", "Foundation layer - Primary wealth store", 1);
    public MaterialLayer SilverRing { get; init; } = new("Silver Ring", "Lunar resonance - Secondary value circuit", 2);
    public MaterialLayer PyroRing { get; init; } = new("Pyro-Ring", "Thermal transformation - Energy catalyst", 3);
    public MaterialLayer MedicinalMineral { get; init; } = new("Medicinal Mineral", "Healing substrate - Regenerative compound", 4);
    public MaterialLayer ShellCoin { get; init; } = new("Shell Coin", "Ancient currency - Historical value marker", 5);
    public MaterialLayer ES0ILSubstrate { get; init; } = new("ES0IL Substrate", "Base layer - Elemental soil foundation", 6);

    public MaterialLayer[] GetAllLayers() => new[]
    {
        GoldBar, SilverRing, PyroRing, MedicinalMineral, ShellCoin, ES0ILSubstrate
    };
}

/// <summary>
/// Individual material layer in the cosmic stack
/// </summary>
public record MaterialLayer
{
    public string Name { get; init; }
    public string Description { get; init; }
    public int StackPosition { get; init; }
    public Dictionary<string, object> Properties { get; init; } = new();

    public MaterialLayer(string name, string description, int stackPosition)
    {
        Name = name;
        Description = description;
        StackPosition = stackPosition;
    }

    public MaterialLayer() : this(string.Empty, string.Empty, 0) { }
}

/// <summary>
/// Activated Codex layers for Zion Gold Bar
/// </summary>
public record CodexActivation
{
    public bool EVOLRosettaLedger { get; init; } = true;
    public bool AfroElohimNarrative { get; init; } = true;
    public bool SaturnWealthDecoded { get; init; } = true;
    public bool BLEUCodexIntegration { get; init; } = true;
    public string[] ActiveProtocols { get; init; } = new[]
    {
        "EV0L Rosetta Ledger Scroll",
        "Afro-Elohim Wealth Archive",
        "Saturn Strata Classification",
        "BLEU Vault Certificate System"
    };
}

/// <summary>
/// BLEU Vault Certificate information
/// </summary>
public record VaultCertificate
{
    public string CertificateId { get; init; } = string.Empty;
    public string Classification { get; init; } = "Saturn-Strata Resource Certificate";
    public DateTime IssuedDate { get; init; }
    public string Authority { get; init; } = "BLEU Vault Authority";
    public string Status { get; init; } = "Active";
    public Dictionary<string, string> Attestations { get; init; } = new();
}

/// <summary>
/// Resource attributes for Zion Gold Bar
/// </summary>
public record ResourceAttributes
{
    public string PrimaryMetal { get; init; } = "Gold";
    public decimal Weight { get; init; }
    public string WeightUnit { get; init; } = "troy ounces";
    public decimal Purity { get; init; } = 99.99m;
    public string Origin { get; init; } = "Saturn-Strata";
    public string Classification { get; init; } = "God-Tier Resource";
    public Dictionary<string, object> CompositionAnalysis { get; init; } = new();
}

/// <summary>
/// Zion Gold Bar NFT metadata structure (mint-ready for IPFS)
/// </summary>
public record ZionGoldBarMetadata
{
    public string Name { get; init; } = "Zion Gold Bar ENFT";
    public string Description { get; init; } = "A Saturn-Strata classified resource token representing the cosmic stack of six material layers, activated through the EV0L Rosetta Ledger and Afro-Elohim wealth narrative.";
    public string Image { get; init; } = string.Empty; // ipfs://CID
    public string ExternalUrl { get; init; } = "https://evolverse.universe/zion-gold-bar";
    public string AnimationUrl { get; init; } = string.Empty;
    public MetadataAttribute[] Attributes { get; init; } = Array.Empty<MetadataAttribute>();
    public string Certificate { get; init; } = string.Empty; // ipfs://CID for PDF
    public string Creator { get; init; } = string.Empty;
    public Properties Properties { get; init; } = new();
}

public record MetadataAttribute
{
    public string TraitType { get; init; } = string.Empty;
    public object Value { get; init; } = string.Empty;
    public string? DisplayType { get; init; }
}

public record Properties
{
    public string Category { get; init; } = "Zion Gold Bar";
    public string[] Creators { get; init; } = Array.Empty<string>();
    public Files Files { get; init; } = new();
}

public record Files
{
    public string Uri { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string? Cdn { get; init; }
}
