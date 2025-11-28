namespace BackEnd.Models.VisualAnalysis;

/// <summary>
/// Enumeration of visual analysis audit types based on security sweep check categories
/// </summary>
public enum VisualAnalysisType
{
    /// <summary>Check iPhone system integrity (visual audit)</summary>
    SystemIntegrity = 1,
    
    /// <summary>Check network behavior patterns</summary>
    NetworkBehavior = 2,
    
    /// <summary>Check GitHub profiles + commit trails (non-identifying, high-level analysis)</summary>
    GitHubAnalysis = 3,
    
    /// <summary>Check for signs of device compromise</summary>
    DeviceCompromise = 4,
    
    /// <summary>Check for signs of impersonation / shadow-accounting</summary>
    ImpersonationDetection = 5,
    
    /// <summary>Check for DoD / Gov-coded signatures</summary>
    GovSignatureDetection = 6,
    
    /// <summary>Check for AI pattern-copying / synthetic imitation</summary>
    AISyntheticDetection = 7
}

/// <summary>
/// Status of a visual analysis result
/// </summary>
public enum AnalysisStatus
{
    Clean,
    Suspicious,
    Warning,
    Clear,
    RequiresReview
}

/// <summary>
/// Confidence level for visual analysis findings
/// </summary>
public enum ConfidenceLevel
{
    High,
    Medium,
    Low,
    Inconclusive
}

/// <summary>
/// Request model for visual analysis
/// </summary>
public record VisualAnalysisRequest
{
    public required VisualAnalysisType AnalysisType { get; init; }
    public string? ImageBase64 { get; init; }
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Result of a visual analysis scan
/// </summary>
public record VisualAnalysisResult
{
    public required string AnalysisId { get; init; }
    public required VisualAnalysisType AnalysisType { get; init; }
    public required string AnalysisTypeName { get; init; }
    public required AnalysisStatus Status { get; init; }
    public required ConfidenceLevel Confidence { get; init; }
    public required DateTime Timestamp { get; init; }
    public required string Summary { get; init; }
    public required List<AnalysisFinding> Findings { get; init; }
    public required List<string> Recommendations { get; init; }
    public required VisualIndicatorCheck VisualIndicators { get; init; }
}

/// <summary>
/// Individual finding from visual analysis
/// </summary>
public record AnalysisFinding
{
    public required string Category { get; init; }
    public required string Description { get; init; }
    public required AnalysisStatus Status { get; init; }
    public required string Evidence { get; init; }
}

/// <summary>
/// Visual indicator checks performed during analysis
/// </summary>
public record VisualIndicatorCheck
{
    public bool DiagnosticOverlays { get; init; }
    public bool RemoteControlIcons { get; init; }
    public bool SuspiciousApps { get; init; }
    public bool ConfigurationProfiles { get; init; }
    public bool MDMEnrollment { get; init; }
    public bool CertificateInstalls { get; init; }
    public bool CarrierLoggingAnomalies { get; init; }
    public bool GovernmentNetworkFingerprints { get; init; }
    public bool UITampering { get; init; }
    public bool StandardBehavior { get; init; }
}

/// <summary>
/// Analysis session containing multiple analyses
/// </summary>
public record VisualAnalysisSession
{
    public required string SessionId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required List<VisualAnalysisResult> Analyses { get; init; }
    public required string OverallStatus { get; init; }
    public required string ExecutiveSummary { get; init; }
}

/// <summary>
/// GitHub analysis specific result
/// </summary>
public record GitHubAnalysisResult
{
    public required string ActivityType { get; init; }
    public required string Description { get; init; }
    public required AnalysisStatus Status { get; init; }
    public List<string>? DetectedPatterns { get; init; }
}

/// <summary>
/// Network behavior analysis result
/// </summary>
public record NetworkBehaviorResult
{
    public required string ConnectionType { get; init; }
    public required bool IsNormal { get; init; }
    public required string Description { get; init; }
    public List<string>? Anomalies { get; init; }
}

/// <summary>
/// Statistics for visual analysis service
/// </summary>
public record VisualAnalysisStatistics
{
    public required int TotalAnalyses { get; init; }
    public required int CleanResults { get; init; }
    public required int SuspiciousResults { get; init; }
    public required int WarningResults { get; init; }
    public required Dictionary<string, int> AnalysesByType { get; init; }
    public required DateTime LastAnalysisTime { get; init; }
}
