namespace BackEnd.Models.Forensics;

/// <summary>
/// Blockchain network identifiers for cross-chain forensic analysis
/// </summary>
public enum BlockchainNetwork
{
    Ethereum,
    Cronos,
    Avalanche,
    Base,
    Polygon,
    BNBChain,
    Arbitrum,
    Optimism,
    Fantom,
    Solana
}

/// <summary>
/// Types of contract events tracked in forensic analysis
/// </summary>
public enum ContractEventType
{
    Transfer,
    Swap,
    Burn,
    Mint,
    Approve,
    Deposit,
    Withdraw,
    Bridge,
    Stake,
    Unstake,
    Claim,
    Unknown
}

/// <summary>
/// Asset movement status for tracking fund flow
/// </summary>
public enum AssetMovementStatus
{
    InControl,        // Asset still in user's control
    Transferred,      // Asset transferred to another address
    Swapped,          // Asset swapped for another token
    Burned,           // Asset sent to burn address
    Bridged,          // Asset bridged to another chain
    Lost,             // Asset sent to wrong address/contract
    Recovered,        // Asset recovered after incident
    Disputed          // Asset movement under dispute
}

/// <summary>
/// Wallet address with blockchain network context
/// </summary>
public record WalletAddress
{
    public string Address { get; init; } = string.Empty;
    public BlockchainNetwork Network { get; init; }
    public string? Label { get; init; }
    public bool IsContract { get; init; }
    public bool IsKnownExchange { get; init; }
    public bool IsBurnAddress { get; init; }
    public bool IsMixerContract { get; init; }
}

/// <summary>
/// Contract information for forensic analysis
/// </summary>
public record ContractInfo
{
    public string ContractAddress { get; init; } = string.Empty;
    public BlockchainNetwork Network { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Symbol { get; init; } = string.Empty;
    public string ContractType { get; init; } = string.Empty; // ERC20, ERC721, DEX Router, Bridge, etc.
    public bool IsVerified { get; init; }
    public string? AbiSource { get; init; }
    public string? GitHubUrl { get; init; }
    public string? AuditUrl { get; init; }
}

/// <summary>
/// Contract event for tracking on-chain activity
/// </summary>
public record ContractEvent
{
    public string EventId { get; init; } = string.Empty;
    public string TransactionHash { get; init; } = string.Empty;
    public long BlockNumber { get; init; }
    public DateTime Timestamp { get; init; }
    public ContractEventType EventType { get; init; }
    public BlockchainNetwork Network { get; init; }
    public string ContractAddress { get; init; } = string.Empty;
    public string FromAddress { get; init; } = string.Empty;
    public string ToAddress { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string TokenSymbol { get; init; } = string.Empty;
    public string MethodName { get; init; } = string.Empty;
    public Dictionary<string, object> EventData { get; init; } = new();
    public string RawData { get; init; } = string.Empty;
}

/// <summary>
/// Transaction record for forensic analysis with full details
/// </summary>
public record ForensicTransaction
{
    public string TransactionHash { get; init; } = string.Empty;
    public long BlockNumber { get; init; }
    public DateTime Timestamp { get; init; }
    public BlockchainNetwork Network { get; init; }
    public string FromAddress { get; init; } = string.Empty;
    public string ToAddress { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public decimal GasUsed { get; init; }
    public decimal GasPrice { get; init; }
    public bool IsSuccessful { get; init; }
    public string MethodId { get; init; } = string.Empty;
    public string MethodName { get; init; } = string.Empty;
    public string InputData { get; init; } = string.Empty;
    public List<ContractEvent> Events { get; init; } = new();
    public AssetMovementStatus MovementStatus { get; init; }
    public string? ForensicNotes { get; init; }
}

/// <summary>
/// Asset flow record tracing movement between addresses
/// </summary>
public record AssetFlow
{
    public string FlowId { get; init; } = string.Empty;
    public string AssetSymbol { get; init; } = string.Empty;
    public string AssetContractAddress { get; init; } = string.Empty;
    public BlockchainNetwork SourceNetwork { get; init; }
    public BlockchainNetwork DestinationNetwork { get; init; }
    public WalletAddress SourceAddress { get; init; } = new();
    public WalletAddress DestinationAddress { get; init; } = new();
    public decimal Amount { get; init; }
    public DateTime Timestamp { get; init; }
    public string TransactionHash { get; init; } = string.Empty;
    public AssetMovementStatus Status { get; init; }
    public List<string> IntermediaryAddresses { get; init; } = new();
    public int HopsCount { get; init; }
    public bool IsDirectTransfer { get; init; }
    public string? Narrative { get; init; }
}

/// <summary>
/// Chain of custody entry for legal documentation
/// </summary>
public record ChainOfCustodyEntry
{
    public int SequenceNumber { get; init; }
    public DateTime Timestamp { get; init; }
    public string TransactionHash { get; init; } = string.Empty;
    public BlockchainNetwork Network { get; init; }
    public string HolderAddress { get; init; } = string.Empty;
    public string? HolderLabel { get; init; }
    public ContractEventType ActionType { get; init; }
    public decimal AmountBefore { get; init; }
    public decimal AmountAfter { get; init; }
    public decimal AmountChanged { get; init; }
    public string Description { get; init; } = string.Empty;
    public string EvidenceUrl { get; init; } = string.Empty;
    public bool IsVerified { get; init; }
}

/// <summary>
/// Forensic analysis case for tracking investigations
/// </summary>
public record ForensicCase
{
    public string CaseId { get; init; } = string.Empty;
    public string CaseTitle { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public string Status { get; init; } = "Open"; // Open, InProgress, Closed, Escalated
    public List<WalletAddress> TrackedAddresses { get; init; } = new();
    public List<BlockchainNetwork> NetworksInvolved { get; init; } = new();
    public List<ForensicTransaction> Transactions { get; init; } = new();
    public List<AssetFlow> AssetFlows { get; init; } = new();
    public List<ChainOfCustodyEntry> ChainOfCustody { get; init; } = new();
    public decimal TotalValueTracked { get; init; }
    public decimal TotalValueLost { get; init; }
    public decimal TotalValueRecovered { get; init; }
    public List<string> Findings { get; init; } = new();
    public string? LegalNarrative { get; init; }
}

/// <summary>
/// Forensic report for legal and insurance proceedings
/// </summary>
public record ForensicReport
{
    public string ReportId { get; init; } = string.Empty;
    public string CaseId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DateTime GeneratedAt { get; init; }
    public string PreparedBy { get; init; } = string.Empty;
    
    // Executive Summary
    public string ExecutiveSummary { get; init; } = string.Empty;
    
    // Timeline of Events
    public List<TimelineEvent> Timeline { get; init; } = new();
    
    // Asset Tracking
    public List<AssetFlow> AssetFlows { get; init; } = new();
    public decimal TotalAssetsAtRisk { get; init; }
    public decimal TotalAssetsLost { get; init; }
    
    // Chain of Custody
    public List<ChainOfCustodyEntry> ProvenanceTrail { get; init; } = new();
    
    // Technical Findings
    public List<TechnicalFinding> TechnicalFindings { get; init; } = new();
    
    // Evidence References
    public List<EvidenceReference> Evidence { get; init; } = new();
    
    // Legal Narrative
    public string LegalNarrative { get; init; } = string.Empty;
    
    // Recommendations
    public List<string> Recommendations { get; init; } = new();
    
    // Appendices
    public List<string> TransactionHashes { get; init; } = new();
    public List<ContractInfo> ContractsAnalyzed { get; init; } = new();
}

/// <summary>
/// Timeline event for chronological narrative
/// </summary>
public record TimelineEvent
{
    public int Order { get; init; }
    public DateTime Timestamp { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public BlockchainNetwork Network { get; init; }
    public string TransactionHash { get; init; } = string.Empty;
    public string ExplorerUrl { get; init; } = string.Empty;
    public decimal? ValueImpact { get; init; }
    public string Category { get; init; } = string.Empty; // Initiation, Movement, Swap, Burn, Loss, Recovery
}

/// <summary>
/// Technical finding from forensic analysis
/// </summary>
public record TechnicalFinding
{
    public string FindingId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty; // Critical, High, Medium, Low, Informational
    public string Category { get; init; } = string.Empty; // Contract Exploit, Social Engineering, Protocol Design, User Error
    public string Evidence { get; init; } = string.Empty;
    public string Impact { get; init; } = string.Empty;
    public List<string> AffectedTransactions { get; init; } = new();
}

/// <summary>
/// Evidence reference for legal documentation
/// </summary>
public record EvidenceReference
{
    public string EvidenceId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty; // Transaction, Contract, Screenshot, API Response
    public string Description { get; init; } = string.Empty;
    public string Source { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public DateTime CapturedAt { get; init; }
    public string Hash { get; init; } = string.Empty; // For integrity verification
}

/// <summary>
/// Cross-chain bridge analysis record
/// </summary>
public record BridgeAnalysis
{
    public string BridgeId { get; init; } = string.Empty;
    public string BridgeName { get; init; } = string.Empty;
    public BlockchainNetwork SourceChain { get; init; }
    public BlockchainNetwork DestinationChain { get; init; }
    public string SourceTxHash { get; init; } = string.Empty;
    public string? DestinationTxHash { get; init; }
    public decimal AmountSent { get; init; }
    public decimal? AmountReceived { get; init; }
    public bool IsCompleted { get; init; }
    public bool IsStuck { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? FailureReason { get; init; }
}

/// <summary>
/// Swap analysis record for DEX transactions
/// </summary>
public record SwapAnalysis
{
    public string SwapId { get; init; } = string.Empty;
    public string DexName { get; init; } = string.Empty;
    public string RouterAddress { get; init; } = string.Empty;
    public BlockchainNetwork Network { get; init; }
    public string TransactionHash { get; init; } = string.Empty;
    public string TokenIn { get; init; } = string.Empty;
    public string TokenOut { get; init; } = string.Empty;
    public decimal AmountIn { get; init; }
    public decimal AmountOut { get; init; }
    public decimal ExpectedAmountOut { get; init; }
    public decimal Slippage { get; init; }
    public bool IsSuccessful { get; init; }
    public string SwapMethod { get; init; } = string.Empty; // swapExactTokensForETH, swapETHForExactTokens, etc.
}

/// <summary>
/// Address analytics for forensic profiling
/// </summary>
public record AddressAnalytics
{
    public string Address { get; init; } = string.Empty;
    public BlockchainNetwork Network { get; init; }
    public DateTime FirstActivity { get; init; }
    public DateTime LastActivity { get; init; }
    public int TotalTransactions { get; init; }
    public int IncomingTransactions { get; init; }
    public int OutgoingTransactions { get; init; }
    public decimal TotalValueReceived { get; init; }
    public decimal TotalValueSent { get; init; }
    public decimal CurrentBalance { get; init; }
    public List<string> InteractedContracts { get; init; } = new();
    public List<string> RelatedAddresses { get; init; } = new();
    public bool IsSuspicious { get; init; }
    public List<string> Flags { get; init; } = new();
}
