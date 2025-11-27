namespace BackEnd.Models.Forensics;

/// <summary>
/// Request DTO for creating a forensic case
/// </summary>
public record CreateForensicCaseRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Addresses { get; init; } = new();
    public List<BlockchainNetwork> Networks { get; init; } = new();
}

/// <summary>
/// Request DTO for recording a forensic transaction
/// </summary>
public record RecordTransactionRequest
{
    public string TxHash { get; init; } = string.Empty;
    public long BlockNumber { get; init; }
    public DateTime Timestamp { get; init; }
    public BlockchainNetwork Network { get; init; }
    public string From { get; init; } = string.Empty;
    public string To { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string MethodName { get; init; } = string.Empty;
}

/// <summary>
/// Request DTO for tracing asset flow
/// </summary>
public record TraceAssetFlowRequest
{
    public string AssetSymbol { get; init; } = string.Empty;
    public string AssetContract { get; init; } = string.Empty;
    public string SourceAddress { get; init; } = string.Empty;
    public string DestinationAddress { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public BlockchainNetwork SourceNetwork { get; init; }
    public BlockchainNetwork? DestinationNetwork { get; init; }
    public string? TxHash { get; init; }
}

/// <summary>
/// Request DTO for generating forensic report
/// </summary>
public record GenerateReportRequest
{
    public string CaseId { get; init; } = string.Empty;
    public string PreparedBy { get; init; } = string.Empty;
}

/// <summary>
/// Request DTO for analyzing swap transactions
/// </summary>
public record AnalyzeSwapRequest
{
    public string TxHash { get; init; } = string.Empty;
    public string DexName { get; init; } = string.Empty;
    public string RouterAddress { get; init; } = string.Empty;
    public BlockchainNetwork Network { get; init; }
    public string TokenIn { get; init; } = string.Empty;
    public string TokenOut { get; init; } = string.Empty;
    public decimal AmountIn { get; init; }
    public decimal AmountOut { get; init; }
    public decimal ExpectedAmountOut { get; init; }
    public string SwapMethod { get; init; } = string.Empty;
}

/// <summary>
/// Request DTO for analyzing bridge transactions
/// </summary>
public record AnalyzeBridgeRequest
{
    public string BridgeName { get; init; } = string.Empty;
    public BlockchainNetwork SourceChain { get; init; }
    public BlockchainNetwork DestinationChain { get; init; }
    public string SourceTxHash { get; init; } = string.Empty;
    public string? DestinationTxHash { get; init; }
    public decimal AmountSent { get; init; }
    public decimal? AmountReceived { get; init; }
}
