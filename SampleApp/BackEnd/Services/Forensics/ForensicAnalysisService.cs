using BackEnd.Models.Forensics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BackEnd.Services.Forensics;

/// <summary>
/// Blockchain Forensic Analysis Service for tracking asset movements,
/// generating chain-of-custody reports, and creating legal narratives
/// for arbitration, insurance, and recovery proceedings.
/// </summary>
public class ForensicAnalysisService
{
    private readonly ILogger<ForensicAnalysisService> _logger;
    private readonly List<ForensicCase> _cases = new();
    private readonly List<ForensicTransaction> _transactions = new();
    private readonly List<AssetFlow> _assetFlows = new();
    private readonly List<ContractInfo> _knownContracts = new();
    private readonly Dictionary<string, AddressAnalytics> _addressAnalytics = new();
    private int _caseCounter = 1;
    private int _flowCounter = 1;

    // Known burn addresses across networks
    private static readonly HashSet<string> BurnAddresses = new(StringComparer.OrdinalIgnoreCase)
    {
        "0x0000000000000000000000000000000000000000",
        "0x000000000000000000000000000000000000dEaD",
        "0xdead000000000000000000000000000000000000"
    };

    // Known mixer/tumbler contracts (example addresses for demonstration)
    private static readonly HashSet<string> MixerContracts = new(StringComparer.OrdinalIgnoreCase)
    {
        // Tornado Cash pools (example - not actual addresses)
        "0x47CE0C6eD5B0Ce3d3A51fdb1C52DC66a7c3c2936"
    };

    public ForensicAnalysisService(ILogger<ForensicAnalysisService> logger)
    {
        _logger = logger;
        InitializeKnownContracts();
    }

    /// <summary>
    /// Create a new forensic investigation case
    /// </summary>
    public Task<ForensicCase> CreateCase(string title, string description, List<string> addresses, List<BlockchainNetwork> networks)
    {
        var caseId = $"CASE-{_caseCounter:D6}";
        _caseCounter++;

        var trackedAddresses = addresses.Select(addr => new WalletAddress
        {
            Address = addr,
            Network = networks.FirstOrDefault(),
            IsBurnAddress = BurnAddresses.Contains(addr),
            IsMixerContract = MixerContracts.Contains(addr)
        }).ToList();

        var forensicCase = new ForensicCase
        {
            CaseId = caseId,
            CaseTitle = title,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            Status = "Open",
            TrackedAddresses = trackedAddresses,
            NetworksInvolved = networks,
            Transactions = new(),
            AssetFlows = new(),
            ChainOfCustody = new(),
            Findings = new()
        };

        _cases.Add(forensicCase);
        _logger.LogInformation("Created forensic case {CaseId}: {Title}", caseId, title);

        return Task.FromResult(forensicCase);
    }

    /// <summary>
    /// Record a transaction for forensic analysis
    /// </summary>
    public Task<ForensicTransaction> RecordTransaction(
        string txHash,
        long blockNumber,
        DateTime timestamp,
        BlockchainNetwork network,
        string from,
        string to,
        decimal value,
        string methodName,
        List<ContractEvent>? events = null)
    {
        var status = DetermineMovementStatus(to, events);

        var transaction = new ForensicTransaction
        {
            TransactionHash = txHash,
            BlockNumber = blockNumber,
            Timestamp = timestamp,
            Network = network,
            FromAddress = from,
            ToAddress = to,
            Value = value,
            IsSuccessful = true,
            MethodName = methodName,
            Events = events ?? new(),
            MovementStatus = status,
            ForensicNotes = GenerateTransactionNotes(from, to, value, methodName, events)
        };

        _transactions.Add(transaction);
        UpdateAddressAnalytics(from, to, value, network);
        _logger.LogInformation("Recorded transaction {TxHash} on {Network}", txHash, network);

        return Task.FromResult(transaction);
    }

    /// <summary>
    /// Trace asset flow from source to destination
    /// </summary>
    public Task<AssetFlow> TraceAssetFlow(
        string assetSymbol,
        string assetContract,
        string sourceAddress,
        string destinationAddress,
        decimal amount,
        BlockchainNetwork sourceNetwork,
        BlockchainNetwork? destinationNetwork = null,
        string? txHash = null)
    {
        var flowId = $"FLOW-{_flowCounter:D8}";
        _flowCounter++;

        var sourceWallet = AnalyzeAddress(sourceAddress, sourceNetwork);
        var destWallet = AnalyzeAddress(destinationAddress, destinationNetwork ?? sourceNetwork);

        var status = DetermineFlowStatus(destWallet);

        var flow = new AssetFlow
        {
            FlowId = flowId,
            AssetSymbol = assetSymbol,
            AssetContractAddress = assetContract,
            SourceNetwork = sourceNetwork,
            DestinationNetwork = destinationNetwork ?? sourceNetwork,
            SourceAddress = sourceWallet,
            DestinationAddress = destWallet,
            Amount = amount,
            Timestamp = DateTime.UtcNow,
            TransactionHash = txHash ?? string.Empty,
            Status = status,
            IsDirectTransfer = sourceNetwork == (destinationNetwork ?? sourceNetwork),
            HopsCount = 1,
            Narrative = GenerateFlowNarrative(sourceWallet, destWallet, amount, assetSymbol, status)
        };

        _assetFlows.Add(flow);
        _logger.LogInformation("Traced asset flow {FlowId}: {Amount} {Symbol} from {Source} to {Dest}",
            flowId, amount, assetSymbol, sourceAddress, destinationAddress);

        return Task.FromResult(flow);
    }

    /// <summary>
    /// Generate chain of custody for a case
    /// </summary>
    public Task<List<ChainOfCustodyEntry>> GenerateChainOfCustody(string caseId)
    {
        var forensicCase = _cases.FirstOrDefault(c => c.CaseId == caseId);
        if (forensicCase == null)
            return Task.FromResult(new List<ChainOfCustodyEntry>());

        var chainOfCustody = new List<ChainOfCustodyEntry>();
        var sequence = 1;
        decimal runningBalance = 0;

        var relevantFlows = _assetFlows
            .Where(f => forensicCase.TrackedAddresses.Any(a =>
                a.Address.Equals(f.SourceAddress.Address, StringComparison.OrdinalIgnoreCase) ||
                a.Address.Equals(f.DestinationAddress.Address, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(f => f.Timestamp)
            .ToList();

        foreach (var flow in relevantFlows)
        {
            var amountBefore = runningBalance;
            var isIncoming = forensicCase.TrackedAddresses.Any(a =>
                a.Address.Equals(flow.DestinationAddress.Address, StringComparison.OrdinalIgnoreCase));

            if (isIncoming)
                runningBalance += flow.Amount;
            else
                runningBalance -= flow.Amount;

            var entry = new ChainOfCustodyEntry
            {
                SequenceNumber = sequence++,
                Timestamp = flow.Timestamp,
                TransactionHash = flow.TransactionHash,
                Network = flow.SourceNetwork,
                HolderAddress = isIncoming ? flow.DestinationAddress.Address : flow.SourceAddress.Address,
                HolderLabel = isIncoming ? flow.DestinationAddress.Label : flow.SourceAddress.Label,
                ActionType = DetermineActionType(flow),
                AmountBefore = amountBefore,
                AmountAfter = runningBalance,
                AmountChanged = isIncoming ? flow.Amount : -flow.Amount,
                Description = flow.Narrative ?? $"{flow.Amount} {flow.AssetSymbol} {(isIncoming ? "received" : "sent")}",
                EvidenceUrl = GetExplorerUrl(flow.SourceNetwork, flow.TransactionHash),
                IsVerified = true
            };

            chainOfCustody.Add(entry);
        }

        return Task.FromResult(chainOfCustody);
    }

    /// <summary>
    /// Generate a comprehensive forensic report for legal/insurance proceedings
    /// </summary>
    public Task<ForensicReport> GenerateForensicReport(string caseId, string preparedBy)
    {
        var forensicCase = _cases.FirstOrDefault(c => c.CaseId == caseId);
        if (forensicCase == null)
            throw new KeyNotFoundException($"Case {caseId} not found");

        var reportId = $"RPT-{caseId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var chainOfCustody = GenerateChainOfCustody(caseId).Result;

        var relevantFlows = _assetFlows
            .Where(f => forensicCase.TrackedAddresses.Any(a =>
                a.Address.Equals(f.SourceAddress.Address, StringComparison.OrdinalIgnoreCase) ||
                a.Address.Equals(f.DestinationAddress.Address, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var relevantTransactions = _transactions
            .Where(t => forensicCase.TrackedAddresses.Any(a =>
                a.Address.Equals(t.FromAddress, StringComparison.OrdinalIgnoreCase) ||
                a.Address.Equals(t.ToAddress, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var timeline = GenerateTimeline(relevantFlows, relevantTransactions);
        var findings = AnalyzeForFindings(relevantFlows, relevantTransactions);
        var evidence = GenerateEvidenceReferences(relevantTransactions);

        var totalLost = relevantFlows
            .Where(f => f.Status == AssetMovementStatus.Lost || f.Status == AssetMovementStatus.Burned)
            .Sum(f => f.Amount);

        var report = new ForensicReport
        {
            ReportId = reportId,
            CaseId = caseId,
            Title = $"Forensic Analysis Report: {forensicCase.CaseTitle}",
            GeneratedAt = DateTime.UtcNow,
            PreparedBy = preparedBy,
            ExecutiveSummary = GenerateExecutiveSummary(forensicCase, relevantFlows, totalLost),
            Timeline = timeline,
            AssetFlows = relevantFlows,
            TotalAssetsAtRisk = relevantFlows.Sum(f => f.Amount),
            TotalAssetsLost = totalLost,
            ProvenanceTrail = chainOfCustody,
            TechnicalFindings = findings,
            Evidence = evidence,
            LegalNarrative = GenerateLegalNarrative(forensicCase, chainOfCustody, findings),
            Recommendations = GenerateRecommendations(findings),
            TransactionHashes = relevantTransactions.Select(t => t.TransactionHash).ToList(),
            ContractsAnalyzed = _knownContracts
                .Where(c => relevantTransactions.Any(t =>
                    t.ToAddress.Equals(c.ContractAddress, StringComparison.OrdinalIgnoreCase)))
                .ToList()
        };

        _logger.LogInformation("Generated forensic report {ReportId} for case {CaseId}", reportId, caseId);
        return Task.FromResult(report);
    }

    /// <summary>
    /// Analyze a swap transaction for potential issues
    /// </summary>
    public Task<SwapAnalysis> AnalyzeSwap(
        string txHash,
        string dexName,
        string routerAddress,
        BlockchainNetwork network,
        string tokenIn,
        string tokenOut,
        decimal amountIn,
        decimal amountOut,
        decimal expectedAmountOut,
        string swapMethod)
    {
        var slippage = expectedAmountOut > 0
            ? ((expectedAmountOut - amountOut) / expectedAmountOut) * 100
            : 0;

        var analysis = new SwapAnalysis
        {
            SwapId = $"SWAP-{Guid.NewGuid().ToString("N")[..12].ToUpper()}",
            DexName = dexName,
            RouterAddress = routerAddress,
            Network = network,
            TransactionHash = txHash,
            TokenIn = tokenIn,
            TokenOut = tokenOut,
            AmountIn = amountIn,
            AmountOut = amountOut,
            ExpectedAmountOut = expectedAmountOut,
            Slippage = slippage,
            IsSuccessful = amountOut > 0,
            SwapMethod = swapMethod
        };

        _logger.LogInformation("Analyzed swap {SwapId}: {AmountIn} {TokenIn} -> {AmountOut} {TokenOut} (Slippage: {Slippage}%)",
            analysis.SwapId, amountIn, tokenIn, amountOut, tokenOut, slippage);

        return Task.FromResult(analysis);
    }

    /// <summary>
    /// Analyze a bridge transaction
    /// </summary>
    public Task<BridgeAnalysis> AnalyzeBridge(
        string bridgeName,
        BlockchainNetwork sourceChain,
        BlockchainNetwork destinationChain,
        string sourceTxHash,
        string? destinationTxHash,
        decimal amountSent,
        decimal? amountReceived)
    {
        var isCompleted = !string.IsNullOrEmpty(destinationTxHash) && amountReceived.HasValue && amountReceived > 0;
        // A bridge is considered stuck if it's not completed and no destination tx exists
        // In production, this would compare against the source transaction timestamp
        var isStuck = !isCompleted && string.IsNullOrEmpty(destinationTxHash);

        var analysis = new BridgeAnalysis
        {
            BridgeId = $"BRIDGE-{Guid.NewGuid().ToString("N")[..12].ToUpper()}",
            BridgeName = bridgeName,
            SourceChain = sourceChain,
            DestinationChain = destinationChain,
            SourceTxHash = sourceTxHash,
            DestinationTxHash = destinationTxHash,
            AmountSent = amountSent,
            AmountReceived = amountReceived,
            IsCompleted = isCompleted,
            IsStuck = isStuck,
            Status = isCompleted ? "Completed" : (isStuck ? "Stuck" : "Pending"),
            FailureReason = !isCompleted && amountReceived == 0 ? "Bridge transaction failed or tokens not received" : null
        };

        _logger.LogInformation("Analyzed bridge {BridgeId}: {AmountSent} from {Source} to {Dest} - Status: {Status}",
            analysis.BridgeId, amountSent, sourceChain, destinationChain, analysis.Status);

        return Task.FromResult(analysis);
    }

    /// <summary>
    /// Get address analytics for forensic profiling
    /// </summary>
    public Task<AddressAnalytics> GetAddressAnalytics(string address, BlockchainNetwork network)
    {
        var key = $"{network}:{address}".ToLowerInvariant();
        if (_addressAnalytics.TryGetValue(key, out var analytics))
            return Task.FromResult(analytics);

        // Create new analytics for unknown address
        analytics = new AddressAnalytics
        {
            Address = address,
            Network = network,
            FirstActivity = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            IsSuspicious = BurnAddresses.Contains(address) || MixerContracts.Contains(address),
            Flags = new List<string>()
        };

        if (BurnAddresses.Contains(address))
            analytics = analytics with { Flags = analytics.Flags.Append("BURN_ADDRESS").ToList() };

        if (MixerContracts.Contains(address))
            analytics = analytics with { Flags = analytics.Flags.Append("MIXER_CONTRACT").ToList() };

        return Task.FromResult(analytics);
    }

    /// <summary>
    /// Get all cases
    /// </summary>
    public Task<List<ForensicCase>> GetAllCases()
    {
        return Task.FromResult(_cases.OrderByDescending(c => c.CreatedAt).ToList());
    }

    /// <summary>
    /// Get case by ID
    /// </summary>
    public Task<ForensicCase?> GetCase(string caseId)
    {
        return Task.FromResult(_cases.FirstOrDefault(c => c.CaseId == caseId));
    }

    /// <summary>
    /// Get all asset flows for a case
    /// </summary>
    public Task<List<AssetFlow>> GetAssetFlowsForCase(string caseId)
    {
        var forensicCase = _cases.FirstOrDefault(c => c.CaseId == caseId);
        if (forensicCase == null)
            return Task.FromResult(new List<AssetFlow>());

        var flows = _assetFlows
            .Where(f => forensicCase.TrackedAddresses.Any(a =>
                a.Address.Equals(f.SourceAddress.Address, StringComparison.OrdinalIgnoreCase) ||
                a.Address.Equals(f.DestinationAddress.Address, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(f => f.Timestamp)
            .ToList();

        return Task.FromResult(flows);
    }

    /// <summary>
    /// Get transactions by address
    /// </summary>
    public Task<List<ForensicTransaction>> GetTransactionsByAddress(string address)
    {
        var transactions = _transactions
            .Where(t => t.FromAddress.Equals(address, StringComparison.OrdinalIgnoreCase) ||
                       t.ToAddress.Equals(address, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.Timestamp)
            .ToList();

        return Task.FromResult(transactions);
    }

    /// <summary>
    /// Export case data to CSV for external analysis
    /// </summary>
    public Task<string> ExportCaseToCSV(string caseId)
    {
        var forensicCase = _cases.FirstOrDefault(c => c.CaseId == caseId);
        if (forensicCase == null)
            return Task.FromResult(string.Empty);

        var flows = GetAssetFlowsForCase(caseId).Result;
        var csv = new StringBuilder();
        csv.AppendLine("FlowId,Timestamp,AssetSymbol,Amount,SourceAddress,SourceNetwork,DestinationAddress,DestinationNetwork,Status,TransactionHash,Narrative");

        foreach (var flow in flows)
        {
            csv.AppendLine($"{flow.FlowId},{flow.Timestamp:O},{flow.AssetSymbol},{flow.Amount},{flow.SourceAddress.Address},{flow.SourceNetwork},{flow.DestinationAddress.Address},{flow.DestinationNetwork},{flow.Status},{flow.TransactionHash},\"{flow.Narrative?.Replace("\"", "\"\"")}\"");
        }

        return Task.FromResult(csv.ToString());
    }

    /// <summary>
    /// Get forensic statistics
    /// </summary>
    public Task<ForensicStatistics> GetStatistics()
    {
        var stats = new ForensicStatistics
        {
            TotalCases = _cases.Count,
            OpenCases = _cases.Count(c => c.Status == "Open"),
            ClosedCases = _cases.Count(c => c.Status == "Closed"),
            TotalTransactionsAnalyzed = _transactions.Count,
            TotalAssetFlowsTracked = _assetFlows.Count,
            TotalValueTracked = _assetFlows.Sum(f => f.Amount),
            TotalValueLost = _assetFlows.Where(f => f.Status == AssetMovementStatus.Lost).Sum(f => f.Amount),
            NetworksAnalyzed = _transactions.Select(t => t.Network).Distinct().ToList(),
            MostActiveNetwork = _transactions.GroupBy(t => t.Network).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? BlockchainNetwork.Ethereum
        };

        return Task.FromResult(stats);
    }

    #region Private Helper Methods

    private void InitializeKnownContracts()
    {
        // Add known DEX routers and common contracts
        _knownContracts.AddRange(new[]
        {
            new ContractInfo
            {
                ContractAddress = "0x7a250d5630B4cF539739dF2C5dAcb4c659F2488D",
                Network = BlockchainNetwork.Ethereum,
                Name = "Uniswap V2 Router",
                Symbol = "UNI-V2",
                ContractType = "DEX Router",
                IsVerified = true
            },
            new ContractInfo
            {
                ContractAddress = "0xE592427A0AEce92De3Edee1F18E0157C05861564",
                Network = BlockchainNetwork.Ethereum,
                Name = "Uniswap V3 Router",
                Symbol = "UNI-V3",
                ContractType = "DEX Router",
                IsVerified = true
            },
            new ContractInfo
            {
                ContractAddress = "0x60aE616a2155Ee3d9A68541Ba4544862310933d4",
                Network = BlockchainNetwork.Avalanche,
                Name = "Trader Joe Router",
                Symbol = "JOE",
                ContractType = "DEX Router",
                IsVerified = true
            }
        });
    }

    private WalletAddress AnalyzeAddress(string address, BlockchainNetwork network)
    {
        return new WalletAddress
        {
            Address = address,
            Network = network,
            IsBurnAddress = BurnAddresses.Contains(address),
            IsMixerContract = MixerContracts.Contains(address),
            IsContract = address.StartsWith("0x") && address.Length == 42,
            IsKnownExchange = false, // Would require external API
            Label = GetAddressLabel(address)
        };
    }

    private string? GetAddressLabel(string address)
    {
        if (BurnAddresses.Contains(address))
            return "Burn Address";
        if (MixerContracts.Contains(address))
            return "Mixer Contract";

        var knownContract = _knownContracts.FirstOrDefault(c =>
            c.ContractAddress.Equals(address, StringComparison.OrdinalIgnoreCase));
        return knownContract?.Name;
    }

    private AssetMovementStatus DetermineMovementStatus(string toAddress, List<ContractEvent>? events)
    {
        if (BurnAddresses.Contains(toAddress))
            return AssetMovementStatus.Burned;

        if (MixerContracts.Contains(toAddress))
            return AssetMovementStatus.Lost; // Consider mixer transfers as potentially lost

        if (events?.Any(e => e.EventType == ContractEventType.Bridge) == true)
            return AssetMovementStatus.Bridged;

        if (events?.Any(e => e.EventType == ContractEventType.Swap) == true)
            return AssetMovementStatus.Swapped;

        return AssetMovementStatus.Transferred;
    }

    private AssetMovementStatus DetermineFlowStatus(WalletAddress destination)
    {
        if (destination.IsBurnAddress)
            return AssetMovementStatus.Burned;
        if (destination.IsMixerContract)
            return AssetMovementStatus.Lost;
        if (destination.IsKnownExchange)
            return AssetMovementStatus.Transferred;
        return AssetMovementStatus.Transferred;
    }

    private ContractEventType DetermineActionType(AssetFlow flow)
    {
        if (flow.DestinationAddress.IsBurnAddress)
            return ContractEventType.Burn;
        if (flow.SourceNetwork != flow.DestinationNetwork)
            return ContractEventType.Bridge;
        return ContractEventType.Transfer;
    }

    private string GenerateTransactionNotes(string from, string to, decimal value, string methodName, List<ContractEvent>? events)
    {
        var notes = new StringBuilder();
        notes.Append($"Transaction of {value} from {from[..8]}...{from[^4..]} to {to[..8]}...{to[^4..]}");

        if (!string.IsNullOrEmpty(methodName))
            notes.Append($" via {methodName}");

        if (BurnAddresses.Contains(to))
            notes.Append(" [TOKENS BURNED]");

        if (MixerContracts.Contains(to))
            notes.Append(" [SENT TO MIXER - POTENTIAL LAUNDERING]");

        if (events?.Any(e => e.EventType == ContractEventType.Swap) == true)
            notes.Append(" [SWAP EXECUTED]");

        return notes.ToString();
    }

    private string GenerateFlowNarrative(WalletAddress source, WalletAddress dest, decimal amount, string symbol, AssetMovementStatus status)
    {
        var narrative = new StringBuilder();
        narrative.Append($"{amount} {symbol} moved from ");
        narrative.Append(source.Label ?? $"{source.Address[..8]}...{source.Address[^4..]}");
        narrative.Append(" to ");
        narrative.Append(dest.Label ?? $"{dest.Address[..8]}...{dest.Address[^4..]}");

        switch (status)
        {
            case AssetMovementStatus.Burned:
                narrative.Append(". Assets permanently burned (irrecoverable).");
                break;
            case AssetMovementStatus.Lost:
                narrative.Append(". Assets sent to mixer/tumbler (trail obscured, potentially lost).");
                break;
            case AssetMovementStatus.Bridged:
                narrative.Append(". Assets bridged to another chain.");
                break;
            case AssetMovementStatus.Swapped:
                narrative.Append(". Assets swapped for another token.");
                break;
        }

        return narrative.ToString();
    }

    private void UpdateAddressAnalytics(string from, string to, decimal value, BlockchainNetwork network)
    {
        UpdateSingleAddressAnalytics(from, value, network, isOutgoing: true);
        UpdateSingleAddressAnalytics(to, value, network, isOutgoing: false);
    }

    private void UpdateSingleAddressAnalytics(string address, decimal value, BlockchainNetwork network, bool isOutgoing)
    {
        var key = $"{network}:{address}".ToLowerInvariant();

        if (!_addressAnalytics.TryGetValue(key, out var analytics))
        {
            analytics = new AddressAnalytics
            {
                Address = address,
                Network = network,
                FirstActivity = DateTime.UtcNow,
                LastActivity = DateTime.UtcNow
            };
        }

        analytics = analytics with
        {
            TotalTransactions = analytics.TotalTransactions + 1,
            IncomingTransactions = analytics.IncomingTransactions + (isOutgoing ? 0 : 1),
            OutgoingTransactions = analytics.OutgoingTransactions + (isOutgoing ? 1 : 0),
            TotalValueReceived = analytics.TotalValueReceived + (isOutgoing ? 0 : value),
            TotalValueSent = analytics.TotalValueSent + (isOutgoing ? value : 0),
            LastActivity = DateTime.UtcNow
        };

        _addressAnalytics[key] = analytics;
    }

    private List<TimelineEvent> GenerateTimeline(List<AssetFlow> flows, List<ForensicTransaction> transactions)
    {
        var events = new List<TimelineEvent>();
        var order = 1;

        foreach (var flow in flows.OrderBy(f => f.Timestamp))
        {
            events.Add(new TimelineEvent
            {
                Order = order++,
                Timestamp = flow.Timestamp,
                Title = $"{flow.AssetSymbol} {flow.Status}",
                Description = flow.Narrative ?? "Asset movement detected",
                Network = flow.SourceNetwork,
                TransactionHash = flow.TransactionHash,
                ExplorerUrl = GetExplorerUrl(flow.SourceNetwork, flow.TransactionHash),
                ValueImpact = flow.Amount,
                Category = flow.Status.ToString()
            });
        }

        return events;
    }

    private List<TechnicalFinding> AnalyzeForFindings(List<AssetFlow> flows, List<ForensicTransaction> transactions)
    {
        var findings = new List<TechnicalFinding>();
        var findingCounter = 1;

        // Check for burn events
        var burnedAssets = flows.Where(f => f.Status == AssetMovementStatus.Burned).ToList();
        if (burnedAssets.Any())
        {
            findings.Add(new TechnicalFinding
            {
                FindingId = $"FIND-{findingCounter++:D4}",
                Title = "Assets Burned",
                Description = $"{burnedAssets.Count} transaction(s) resulted in assets being burned (sent to burn address). Total burned: {burnedAssets.Sum(f => f.Amount)}",
                Severity = "High",
                Category = "Protocol Design",
                Evidence = string.Join(", ", burnedAssets.Select(f => f.TransactionHash)),
                Impact = "Assets are permanently irrecoverable",
                AffectedTransactions = burnedAssets.Select(f => f.TransactionHash).ToList()
            });
        }

        // Check for mixer interactions
        var mixerInteractions = flows.Where(f => f.DestinationAddress.IsMixerContract).ToList();
        if (mixerInteractions.Any())
        {
            findings.Add(new TechnicalFinding
            {
                FindingId = $"FIND-{findingCounter++:D4}",
                Title = "Mixer/Tumbler Interaction Detected",
                Description = $"{mixerInteractions.Count} transaction(s) sent to known mixer contracts. This obscures the trail and may indicate money laundering.",
                Severity = "Critical",
                Category = "Potential Money Laundering",
                Evidence = string.Join(", ", mixerInteractions.Select(f => f.TransactionHash)),
                Impact = "Asset trail becomes difficult to trace",
                AffectedTransactions = mixerInteractions.Select(f => f.TransactionHash).ToList()
            });
        }

        // Check for cross-chain bridges
        var bridgedAssets = flows.Where(f => f.SourceNetwork != f.DestinationNetwork).ToList();
        if (bridgedAssets.Any())
        {
            findings.Add(new TechnicalFinding
            {
                FindingId = $"FIND-{findingCounter++:D4}",
                Title = "Cross-Chain Bridge Usage",
                Description = $"{bridgedAssets.Count} transaction(s) bridged assets across chains. Bridge transactions may fail or result in loss.",
                Severity = "Medium",
                Category = "Cross-Chain Risk",
                Evidence = string.Join(", ", bridgedAssets.Select(f => f.TransactionHash)),
                Impact = "Assets may be stuck in bridge or require manual recovery",
                AffectedTransactions = bridgedAssets.Select(f => f.TransactionHash).ToList()
            });
        }

        return findings;
    }

    private List<EvidenceReference> GenerateEvidenceReferences(List<ForensicTransaction> transactions)
    {
        return transactions.Select((t, i) => new EvidenceReference
        {
            EvidenceId = $"EVID-{i + 1:D6}",
            Type = "Transaction",
            Description = $"Transaction {t.TransactionHash[..10]}... on {t.Network}",
            Source = GetExplorerName(t.Network),
            Url = GetExplorerUrl(t.Network, t.TransactionHash),
            CapturedAt = DateTime.UtcNow,
            Hash = ComputeHash(t.TransactionHash)
        }).ToList();
    }

    private string GenerateExecutiveSummary(ForensicCase forensicCase, List<AssetFlow> flows, decimal totalLost)
    {
        return $@"This forensic analysis report documents the investigation of case ""{forensicCase.CaseTitle}"" (Case ID: {forensicCase.CaseId}).

The investigation tracked {flows.Count} asset movements across {forensicCase.NetworksInvolved.Count} blockchain network(s): {string.Join(", ", forensicCase.NetworksInvolved)}.

Key Findings:
- Total assets tracked: {flows.Sum(f => f.Amount):N2}
- Total assets potentially lost: {totalLost:N2}
- Tracked addresses: {forensicCase.TrackedAddresses.Count}
- Investigation status: {forensicCase.Status}

The chain of custody documentation provides a complete provenance trail suitable for legal proceedings, insurance claims, and regulatory filings.";
    }

    private string GenerateLegalNarrative(ForensicCase forensicCase, List<ChainOfCustodyEntry> custody, List<TechnicalFinding> findings)
    {
        var narrative = new StringBuilder();
        narrative.AppendLine($"LEGAL NARRATIVE FOR CASE {forensicCase.CaseId}");
        narrative.AppendLine(new string('=', 50));
        narrative.AppendLine();
        narrative.AppendLine($"Case: {forensicCase.CaseTitle}");
        narrative.AppendLine($"Created: {forensicCase.CreatedAt:yyyy-MM-dd HH:mm:ss UTC}");
        narrative.AppendLine($"Networks Involved: {string.Join(", ", forensicCase.NetworksInvolved)}");
        narrative.AppendLine();
        narrative.AppendLine("CHAIN OF CUSTODY:");
        narrative.AppendLine(new string('-', 40));

        foreach (var entry in custody)
        {
            narrative.AppendLine($"{entry.SequenceNumber}. [{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] {entry.ActionType}: {entry.Description}");
            narrative.AppendLine($"   Transaction: {entry.TransactionHash}");
            narrative.AppendLine($"   Evidence: {entry.EvidenceUrl}");
            narrative.AppendLine();
        }

        if (findings.Any())
        {
            narrative.AppendLine("TECHNICAL FINDINGS:");
            narrative.AppendLine(new string('-', 40));
            foreach (var finding in findings)
            {
                narrative.AppendLine($"[{finding.Severity}] {finding.Title}");
                narrative.AppendLine($"   {finding.Description}");
                narrative.AppendLine($"   Category: {finding.Category}");
                narrative.AppendLine($"   Impact: {finding.Impact}");
                narrative.AppendLine();
            }
        }

        narrative.AppendLine("CONCLUSION:");
        narrative.AppendLine(new string('-', 40));
        narrative.AppendLine("The blockchain ledger provides immutable evidence of the asset movements documented above.");
        narrative.AppendLine("All transaction hashes can be independently verified using the respective blockchain explorers.");

        return narrative.ToString();
    }

    private List<string> GenerateRecommendations(List<TechnicalFinding> findings)
    {
        var recommendations = new List<string>
        {
            "Preserve all blockchain evidence by archiving transaction data and block explorer screenshots",
            "Document the complete chain of custody for legal proceedings"
        };

        if (findings.Any(f => f.Category == "Potential Money Laundering"))
        {
            recommendations.Add("Report mixer/tumbler interactions to relevant regulatory authorities");
            recommendations.Add("Engage specialized blockchain forensics firm for deeper analysis");
        }

        if (findings.Any(f => f.Category == "Cross-Chain Risk"))
        {
            recommendations.Add("Contact bridge operators for stuck transaction recovery");
            recommendations.Add("Monitor destination chain for delayed asset arrival");
        }

        if (findings.Any(f => f.Severity == "Critical" || f.Severity == "High"))
        {
            recommendations.Add("Consider engaging legal counsel specializing in cryptocurrency disputes");
            recommendations.Add("File report with relevant law enforcement agencies if theft is suspected");
        }

        return recommendations;
    }

    private string GetExplorerUrl(BlockchainNetwork network, string txHash)
    {
        return network switch
        {
            BlockchainNetwork.Ethereum => $"https://etherscan.io/tx/{txHash}",
            BlockchainNetwork.Cronos => $"https://cronoscan.com/tx/{txHash}",
            BlockchainNetwork.Avalanche => $"https://snowtrace.io/tx/{txHash}",
            BlockchainNetwork.Base => $"https://basescan.org/tx/{txHash}",
            BlockchainNetwork.Polygon => $"https://polygonscan.com/tx/{txHash}",
            BlockchainNetwork.BNBChain => $"https://bscscan.com/tx/{txHash}",
            BlockchainNetwork.Arbitrum => $"https://arbiscan.io/tx/{txHash}",
            BlockchainNetwork.Optimism => $"https://optimistic.etherscan.io/tx/{txHash}",
            BlockchainNetwork.Fantom => $"https://ftmscan.com/tx/{txHash}",
            BlockchainNetwork.Solana => $"https://solscan.io/tx/{txHash}",
            _ => $"https://etherscan.io/tx/{txHash}"
        };
    }

    private string GetExplorerName(BlockchainNetwork network)
    {
        return network switch
        {
            BlockchainNetwork.Ethereum => "Etherscan",
            BlockchainNetwork.Cronos => "CronoScan",
            BlockchainNetwork.Avalanche => "Snowtrace",
            BlockchainNetwork.Base => "BaseScan",
            BlockchainNetwork.Polygon => "PolygonScan",
            BlockchainNetwork.BNBChain => "BscScan",
            BlockchainNetwork.Arbitrum => "Arbiscan",
            BlockchainNetwork.Optimism => "Optimistic Etherscan",
            BlockchainNetwork.Fantom => "FtmScan",
            BlockchainNetwork.Solana => "Solscan",
            _ => "Block Explorer"
        };
    }

    private string ComputeHash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }

    #endregion
}

/// <summary>
/// Forensic analysis statistics
/// </summary>
public record ForensicStatistics
{
    public int TotalCases { get; init; }
    public int OpenCases { get; init; }
    public int ClosedCases { get; init; }
    public int TotalTransactionsAnalyzed { get; init; }
    public int TotalAssetFlowsTracked { get; init; }
    public decimal TotalValueTracked { get; init; }
    public decimal TotalValueLost { get; init; }
    public List<BlockchainNetwork> NetworksAnalyzed { get; init; } = new();
    public BlockchainNetwork MostActiveNetwork { get; init; }
}
