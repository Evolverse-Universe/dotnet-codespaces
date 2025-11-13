using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace BackEnd.Services;

/// <summary>
/// Unified Economic Ledger for full transparency across all EV0LVerse systems
/// Integrates BLEU Flame, Zion Gold Bar, ES0IL, and MetaVault transactions
/// </summary>
public class UnifiedEconomicLedgerService
{
    private readonly List<LedgerTransaction> _transactions = new();
    private readonly ILogger<UnifiedEconomicLedgerService> _logger;
    private int _transactionCounter = 1;

    public UnifiedEconomicLedgerService(ILogger<UnifiedEconomicLedgerService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Record a transaction in the unified ledger
    /// </summary>
    public Task<LedgerTransaction> RecordTransaction(
        string system,
        string operation,
        string entityId,
        decimal value,
        Dictionary<string, object>? metadata = null)
    {
        var transactionId = $"TXN-{_transactionCounter:D10}";
        _transactionCounter++;

        var transaction = new LedgerTransaction
        {
            TransactionId = transactionId,
            Timestamp = DateTime.UtcNow,
            System = system,
            Operation = operation,
            EntityId = entityId,
            Value = value,
            Metadata = metadata ?? new Dictionary<string, object>(),
            Hash = ComputeTransactionHash(transactionId, system, operation, entityId, value),
            Verified = true
        };

        _transactions.Add(transaction);
        _logger.LogInformation($"Recorded transaction {transactionId} for {system}: {operation}");

        return Task.FromResult(transaction);
    }

    /// <summary>
    /// Get all transactions
    /// </summary>
    public Task<List<LedgerTransaction>> GetAllTransactions()
    {
        return Task.FromResult(_transactions.OrderByDescending(t => t.Timestamp).ToList());
    }

    /// <summary>
    /// Get transactions by system
    /// </summary>
    public Task<List<LedgerTransaction>> GetTransactionsBySystem(string system)
    {
        var transactions = _transactions
            .Where(t => t.System.Equals(system, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.Timestamp)
            .ToList();

        return Task.FromResult(transactions);
    }

    /// <summary>
    /// Get transactions by entity ID
    /// </summary>
    public Task<List<LedgerTransaction>> GetTransactionsByEntity(string entityId)
    {
        var transactions = _transactions
            .Where(t => t.EntityId.Equals(entityId, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(t => t.Timestamp)
            .ToList();

        return Task.FromResult(transactions);
    }

    /// <summary>
    /// Get ledger statistics
    /// </summary>
    public Task<LedgerStatistics> GetStatistics()
    {
        var stats = new LedgerStatistics
        {
            TotalTransactions = _transactions.Count,
            TotalValue = _transactions.Sum(t => t.Value),
            SystemBreakdown = _transactions
                .GroupBy(t => t.System)
                .ToDictionary(g => g.Key, g => new SystemStats
                {
                    TransactionCount = g.Count(),
                    TotalValue = g.Sum(t => t.Value),
                    LastTransaction = g.Max(t => t.Timestamp)
                }),
            FirstTransaction = _transactions.Any() ? _transactions.Min(t => t.Timestamp) : DateTime.UtcNow,
            LastTransaction = _transactions.Any() ? _transactions.Max(t => t.Timestamp) : DateTime.UtcNow
        };

        return Task.FromResult(stats);
    }

    /// <summary>
    /// Verify transaction integrity
    /// </summary>
    public Task<bool> VerifyTransaction(string transactionId)
    {
        var transaction = _transactions.FirstOrDefault(t => t.TransactionId == transactionId);
        if (transaction == null)
            return Task.FromResult(false);

        var expectedHash = ComputeTransactionHash(
            transaction.TransactionId,
            transaction.System,
            transaction.Operation,
            transaction.EntityId,
            transaction.Value);

        var isValid = transaction.Hash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        return Task.FromResult(isValid);
    }

    /// <summary>
    /// Get total ecosystem value across all systems
    /// </summary>
    public Task<EcosystemValue> GetTotalEcosystemValue()
    {
        var bleuValue = _transactions
            .Where(t => t.System == "BLEU_FLAME")
            .Sum(t => t.Value);

        var zionValue = _transactions
            .Where(t => t.System == "ZION_GOLD_BAR")
            .Sum(t => t.Value);

        var es0ilValue = _transactions
            .Where(t => t.System == "ES0IL")
            .Sum(t => t.Value);

        var metavaultValue = _transactions
            .Where(t => t.System == "METAVAULT")
            .Sum(t => t.Value);

        var ecosystemValue = new EcosystemValue
        {
            TotalValue = bleuValue + zionValue + es0ilValue + metavaultValue,
            BLEUFlameValue = bleuValue,
            ZionGoldBarValue = zionValue,
            ES0ILValue = es0ilValue,
            MetaVaultValue = metavaultValue,
            Timestamp = DateTime.UtcNow
        };

        return Task.FromResult(ecosystemValue);
    }

    /// <summary>
    /// Export ledger to CSV for transparency
    /// </summary>
    public Task<string> ExportToCSV()
    {
        var csv = new StringBuilder();
        csv.AppendLine("TransactionId,Timestamp,System,Operation,EntityId,Value,Hash,Verified");

        foreach (var txn in _transactions.OrderBy(t => t.Timestamp))
        {
            csv.AppendLine($"{txn.TransactionId},{txn.Timestamp:O},{txn.System},{txn.Operation},{txn.EntityId},{txn.Value},{txn.Hash},{txn.Verified}");
        }

        return Task.FromResult(csv.ToString());
    }

    /// <summary>
    /// Compute SHA256 hash for transaction
    /// </summary>
    private string ComputeTransactionHash(string txnId, string system, string operation, string entityId, decimal value)
    {
        var data = $"{txnId}|{system}|{operation}|{entityId}|{value}";
        var bytes = Encoding.UTF8.GetBytes(data);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}

/// <summary>
/// Ledger transaction record
/// </summary>
public record LedgerTransaction
{
    public string TransactionId { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string System { get; init; } = string.Empty;
    public string Operation { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public Dictionary<string, object> Metadata { get; init; } = new();
    public string Hash { get; init; } = string.Empty;
    public bool Verified { get; init; }
}

/// <summary>
/// Ledger statistics
/// </summary>
public record LedgerStatistics
{
    public int TotalTransactions { get; init; }
    public decimal TotalValue { get; init; }
    public Dictionary<string, SystemStats> SystemBreakdown { get; init; } = new();
    public DateTime FirstTransaction { get; init; }
    public DateTime LastTransaction { get; init; }
}

/// <summary>
/// System-specific statistics
/// </summary>
public record SystemStats
{
    public int TransactionCount { get; init; }
    public decimal TotalValue { get; init; }
    public DateTime LastTransaction { get; init; }
}

/// <summary>
/// Total ecosystem value across all systems
/// </summary>
public record EcosystemValue
{
    public decimal TotalValue { get; init; }
    public decimal BLEUFlameValue { get; init; }
    public decimal ZionGoldBarValue { get; init; }
    public decimal ES0ILValue { get; init; }
    public decimal MetaVaultValue { get; init; }
    public DateTime Timestamp { get; init; }
}
