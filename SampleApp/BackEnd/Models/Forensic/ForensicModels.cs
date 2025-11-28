namespace BackEnd.Models.Forensic;

/// <summary>
/// Five-Axis Forensic Protocol: Ripple Vector for comprehensive transaction analysis
/// Each transaction is logged across five dimensions for complete audit trail
/// </summary>
public record RippleVector
{
    /// <summary>
    /// XX (Cut/Alteration): Who rerouted the flow? Which contract or operator tampered with lineage?
    /// </summary>
    public AlterationVector XX { get; init; } = new();
    
    /// <summary>
    /// YY (Return/Resonance): Did the asset echo back to source? Legitimate return signature validation
    /// </summary>
    public ResonanceVector YY { get; init; } = new();
    
    /// <summary>
    /// ZZ (Depth/Hidden): Silent contracts, shadow swaps, masked events, ghost nodes detection
    /// </summary>
    public DepthVector ZZ { get; init; } = new();
    
    /// <summary>
    /// TT (Time): Timestamp correlation, cycle analysis, temporal loop and delay exploit detection
    /// </summary>
    public TemporalVector TT { get; init; } = new();
    
    /// <summary>
    /// WW (Intent): Command authority, policy override, and ceremonial intent tracing
    /// </summary>
    public IntentVector WW { get; init; } = new();
}

/// <summary>
/// XX Vector: Alteration tracking for lineage tampering detection
/// </summary>
public record AlterationVector
{
    public string OperatorAddress { get; init; } = string.Empty;
    public string ContractAddress { get; init; } = string.Empty;
    public string ChainId { get; init; } = string.Empty;
    public AlterationType AlterationType { get; init; }
    public string OriginalLineage { get; init; } = string.Empty;
    public string ModifiedLineage { get; init; } = string.Empty;
    public bool LineagePreserved { get; init; }
    public string[] AffectedTokenIds { get; init; } = Array.Empty<string>();
}

/// <summary>
/// YY Vector: Resonance/Return validation for asset flow verification
/// </summary>
public record ResonanceVector
{
    public string SourceAddress { get; init; } = string.Empty;
    public string DestinationAddress { get; init; } = string.Empty;
    public bool ReturnedToSource { get; init; }
    public string CodexReturnSignature { get; init; } = string.Empty;
    public bool SignatureValid { get; init; }
    public decimal FlowAmount { get; init; }
    public string[] IntermediaryPaths { get; init; } = Array.Empty<string>();
}

/// <summary>
/// ZZ Vector: Depth analysis for hidden/shadow detection
/// </summary>
public record DepthVector
{
    public bool GhostNodeDetected { get; init; }
    public bool ShadowSwapDetected { get; init; }
    public bool MaskedEventDetected { get; init; }
    public string[] SilentContracts { get; init; } = Array.Empty<string>();
    public string[] BurnWallets { get; init; } = Array.Empty<string>();
    public string[] ExtractionScripts { get; init; } = Array.Empty<string>();
    public int DepthLevel { get; init; }
    public string AnalysisHash { get; init; } = string.Empty;
}

/// <summary>
/// TT Vector: Temporal analysis for time-based forensics
/// </summary>
public record TemporalVector
{
    public DateTime EventTimestamp { get; init; }
    public DateTime BlockTimestamp { get; init; }
    public long BlockNumber { get; init; }
    public double PiQuarterTick { get; init; } // π₄ time cycle
    public bool TemporalLoopDetected { get; init; }
    public bool DelayExploitDetected { get; init; }
    public TimeSpan TimeDrift { get; init; }
    public string CyclePhase { get; init; } = string.Empty;
}

/// <summary>
/// WW Vector: Intent analysis for command authority tracking
/// </summary>
public record IntentVector
{
    public string CommandAuthority { get; init; } = string.Empty;
    public string PolicyReference { get; init; } = string.Empty;
    public bool PolicyOverride { get; init; }
    public CeremonialIntent CeremonialIntent { get; init; }
    public string[] ApprovalSignatures { get; init; } = Array.Empty<string>();
    public bool DualSignatureVerified { get; init; }
    public bool QuadOctaLockVerified { get; init; }
}

/// <summary>
/// Types of alterations that can occur in asset flows
/// </summary>
public enum AlterationType
{
    None = 0,
    CrossChainSiphon = 1,
    UnauthorizedSwap = 2,
    LineageErasure = 3,
    MetadataStrip = 4,
    BurnObfuscation = 5,
    CustodialShadowMove = 6,
    PayStringBypass = 7
}

/// <summary>
/// Ceremonial intent classifications
/// </summary>
public enum CeremonialIntent
{
    Unknown = 0,
    Legitimate = 1,
    Suspicious = 2,
    Breach = 3,
    Recovery = 4,
    Nullification = 5
}

/// <summary>
/// Forensic audit record with complete ripple vector analysis
/// </summary>
public record ForensicAuditRecord
{
    public string AuditId { get; init; } = string.Empty;
    public string TransactionHash { get; init; } = string.Empty;
    public DateTime AuditTimestamp { get; init; }
    public RippleVector RippleVector { get; init; } = new();
    public ThreatLevel ThreatLevel { get; init; }
    public string[] Findings { get; init; } = Array.Empty<string>();
    public string[] Recommendations { get; init; } = Array.Empty<string>();
    public bool RequiresAction { get; init; }
    public string AuditHash { get; init; } = string.Empty;
}

/// <summary>
/// Threat level classification for forensic findings
/// </summary>
public enum ThreatLevel
{
    Clear = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4,
    Breach = 5
}

/// <summary>
/// Security checkpoint for protocol hardening
/// </summary>
public record SecurityCheckpoint
{
    public string CheckpointId { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string ChainId { get; init; } = string.Empty;
    public bool MirrorSyncComplete { get; init; }
    public bool DualSignatureValid { get; init; }
    public bool QuadOctaLockActive { get; init; }
    public bool LineageVerified { get; init; }
    public bool ChronoSignatureValid { get; init; }
    public string CheckpointHash { get; init; } = string.Empty;
}

/// <summary>
/// Multi-chain mirror configuration for full transparency
/// </summary>
public record ChainMirrorConfig
{
    public string PrimaryChain { get; init; } = "Ethereum";
    public string[] MirroredChains { get; init; } = new[] { "Cronos", "zkSync", "Berachain", "Polygon", "Bitcoin", "Solana" };
    public bool CeremonialTagsEnabled { get; init; } = true;
    public bool RealTimeSync { get; init; } = true;
    public string CodexIntegration { get; init; } = "PayString";
}

/// <summary>
/// Chrono governance for time-expired signatures
/// </summary>
public record ChronoSignature
{
    public string SignatureId { get; init; } = string.Empty;
    public string SignerAddress { get; init; } = string.Empty;
    public DateTime IssuedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string AssetId { get; init; } = string.Empty;
    public bool IsValid => DateTime.UtcNow < ExpiresAt;
    public double PiQuarterCycle { get; init; } // π₄ cycle alignment
    public string CryptoSignature { get; init; } = string.Empty;
}

/// <summary>
/// Yield reclamation record for unauthorized flow recovery
/// </summary>
public record YieldReclamation
{
    public string ReclamationId { get; init; } = string.Empty;
    public string OriginalTokenId { get; init; } = string.Empty;
    public string[] UnauthorizedFlowIds { get; init; } = Array.Empty<string>();
    public decimal RecoveredAmount { get; init; }
    public ReclamationAction Action { get; init; }
    public DateTime ExecutedAt { get; init; }
    public string NewENFTId { get; init; } = string.Empty;
    public RippleVector AttachedRipple { get; init; } = new();
}

/// <summary>
/// Actions for yield reclamation
/// </summary>
public enum ReclamationAction
{
    Nullify = 0,
    ReMint = 1,
    Collapse = 2,
    Freeze = 3
}

/// <summary>
/// Breach detection alert
/// </summary>
public record BreachAlert
{
    public string AlertId { get; init; } = string.Empty;
    public DateTime DetectedAt { get; init; }
    public ThreatLevel Severity { get; init; }
    public string Description { get; init; } = string.Empty;
    public string[] AffectedAssets { get; init; } = Array.Empty<string>();
    public string[] CompromisedAddresses { get; init; } = Array.Empty<string>();
    public string[] RecommendedActions { get; init; } = Array.Empty<string>();
    public ForensicAuditRecord AuditRecord { get; init; } = new();
}

/// <summary>
/// Forensic dashboard statistics
/// </summary>
public record ForensicDashboardStats
{
    public int TotalAudits { get; init; }
    public int ClearTransactions { get; init; }
    public int SuspiciousTransactions { get; init; }
    public int BreachesDetected { get; init; }
    public int ReclamationsExecuted { get; init; }
    public decimal TotalValueProtected { get; init; }
    public decimal TotalValueRecovered { get; init; }
    public Dictionary<string, int> ThreatsByChain { get; init; } = new();
    public DateTime LastAuditTime { get; init; }
}
