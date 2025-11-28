using System.Security.Cryptography;
using System.Text;
using BackEnd.Models.Forensic;

namespace BackEnd.Services.Forensic;

/// <summary>
/// Chrono Governance Service for time-based signature management
/// Implements time-expired signatures aligned with π⁴ cycles
/// </summary>
public class ChronoGovernanceService
{
    private readonly ILogger<ChronoGovernanceService> _logger;
    private readonly List<ChronoSignature> _signatures = new();
    private readonly Dictionary<string, List<ChronoSignature>> _assetSignatures = new();
    
    private const double PiFourth = 97.409091034; // π⁴
    private const int SignatureIdLength = 20;
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromHours(24);

    public ChronoGovernanceService(ILogger<ChronoGovernanceService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Issue a new chrono signature for an asset
    /// </summary>
    public Task<ChronoSignature> IssueChronoSignature(
        string signerAddress,
        string assetId,
        TimeSpan? customExpiration = null)
    {
        var signatureId = $"CHRONO-{Guid.NewGuid():N}"[..SignatureIdLength];
        var now = DateTime.UtcNow;
        var expiration = customExpiration ?? DefaultExpiration;
        
        // Calculate π⁴ cycle alignment
        var piQuarterCycle = CalculatePiQuarterCycle(now);
        
        // Generate cryptographic signature
        var cryptoSignature = GenerateCryptoSignature(signatureId, signerAddress, assetId, now);
        
        var chronoSignature = new ChronoSignature
        {
            SignatureId = signatureId,
            SignerAddress = signerAddress,
            IssuedAt = now,
            ExpiresAt = now.Add(expiration),
            AssetId = assetId,
            PiQuarterCycle = piQuarterCycle,
            CryptoSignature = cryptoSignature
        };
        
        _signatures.Add(chronoSignature);
        
        // Track by asset
        if (!_assetSignatures.TryGetValue(assetId, out var assetSigs))
        {
            assetSigs = new List<ChronoSignature>();
            _assetSignatures[assetId] = assetSigs;
        }
        assetSigs.Add(chronoSignature);
        
        // Sanitize assetId before logging to avoid log forging
        var safeAssetId = assetId?.Replace("\r", "").Replace("\n", "");
        _logger.LogInformation(
            "Chrono signature {SignatureId} issued for asset {AssetId} by {Signer}, expires at {Expiry}",
            signatureId, safeAssetId, signerAddress, chronoSignature.ExpiresAt);
        
        return Task.FromResult(chronoSignature);
    }

    /// <summary>
    /// Validate a chrono signature
    /// </summary>
    public Task<(bool isValid, string reason)> ValidateChronoSignature(string signatureId)
    {
        var signature = _signatures.FirstOrDefault(s => s.SignatureId == signatureId);
        
        if (signature == null)
        {
            return Task.FromResult((false, "Signature not found"));
        }
        
        if (!signature.IsValid)
        {
            return Task.FromResult((false, $"Signature expired at {signature.ExpiresAt:O}"));
        }
        
        // Verify cryptographic signature
        var expectedCrypto = GenerateCryptoSignature(
            signature.SignatureId,
            signature.SignerAddress,
            signature.AssetId,
            signature.IssuedAt);
        
        if (!string.Equals(signature.CryptoSignature, expectedCrypto, StringComparison.Ordinal))
        {
            return Task.FromResult((false, "Cryptographic signature verification failed"));
        }
        
        _logger.LogInformation("Chrono signature {SignatureId} validated successfully", signatureId);
        return Task.FromResult((true, "Signature valid"));
    }

    /// <summary>
    /// Revoke a chrono signature
    /// </summary>
    public Task<bool> RevokeChronoSignature(string signatureId, string revokerAddress)
    {
        var signature = _signatures.FirstOrDefault(s => s.SignatureId == signatureId);
        
        if (signature == null)
        {
            _logger.LogWarning("Attempted to revoke non-existent signature {SignatureId}", signatureId);
            return Task.FromResult(false);
        }
        
        // Only the original signer can revoke
        if (!string.Equals(signature.SignerAddress, revokerAddress, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(
                "Unauthorized revocation attempt for signature {SignatureId} by {Revoker}",
                signatureId, revokerAddress);
            return Task.FromResult(false);
        }
        
        // Mark as expired by creating a new signature with immediate expiry
        var revokedSignature = signature with
        {
            ExpiresAt = DateTime.UtcNow.AddSeconds(-1)
        };
        
        var index = _signatures.FindIndex(s => s.SignatureId == signatureId);
        if (index >= 0)
        {
            _signatures[index] = revokedSignature;
        }
        
        _logger.LogInformation("Chrono signature {SignatureId} revoked by {Revoker}", signatureId, revokerAddress);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Get all signatures for an asset
    /// </summary>
    public Task<List<ChronoSignature>> GetSignaturesForAsset(string assetId)
    {
        if (_assetSignatures.TryGetValue(assetId, out var signatures))
        {
            return Task.FromResult(signatures.OrderByDescending(s => s.IssuedAt).ToList());
        }
        return Task.FromResult(new List<ChronoSignature>());
    }

    /// <summary>
    /// Get all valid signatures
    /// </summary>
    public Task<List<ChronoSignature>> GetValidSignatures()
    {
        var validSignatures = _signatures
            .Where(s => s.IsValid)
            .OrderByDescending(s => s.IssuedAt)
            .ToList();
        
        return Task.FromResult(validSignatures);
    }

    /// <summary>
    /// Get all expired signatures
    /// </summary>
    public Task<List<ChronoSignature>> GetExpiredSignatures()
    {
        var expiredSignatures = _signatures
            .Where(s => !s.IsValid)
            .OrderByDescending(s => s.ExpiresAt)
            .ToList();
        
        return Task.FromResult(expiredSignatures);
    }

    /// <summary>
    /// Cleanup expired signatures older than retention period
    /// </summary>
    public Task<int> CleanupExpiredSignatures(TimeSpan retentionPeriod)
    {
        var cutoff = DateTime.UtcNow.Subtract(retentionPeriod);
        var expiredToRemove = _signatures
            .Where(s => !s.IsValid && s.ExpiresAt < cutoff)
            .ToList();
        
        foreach (var sig in expiredToRemove)
        {
            _signatures.Remove(sig);
            if (_assetSignatures.TryGetValue(sig.AssetId, out var assetSigs))
            {
                assetSigs.Remove(sig);
            }
        }
        
        _logger.LogInformation("Cleaned up {Count} expired chrono signatures", expiredToRemove.Count);
        return Task.FromResult(expiredToRemove.Count);
    }

    /// <summary>
    /// Issue dual signatures for high-value operations (requires two signers)
    /// </summary>
    public async Task<(ChronoSignature primary, ChronoSignature secondary)> IssueDualSignature(
        string primarySigner,
        string secondarySigner,
        string assetId,
        TimeSpan? customExpiration = null)
    {
        if (string.Equals(primarySigner, secondarySigner, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Dual signature requires two different signers");
        }
        
        var expiration = customExpiration ?? DefaultExpiration;
        
        var primarySignature = await IssueChronoSignature(primarySigner, assetId, expiration);
        var secondarySignature = await IssueChronoSignature(secondarySigner, assetId, expiration);
        
        _logger.LogInformation(
            "Dual chrono signatures issued for asset {AssetId}: {Primary} and {Secondary}",
            assetId, primarySignature.SignatureId, secondarySignature.SignatureId);
        
        return (primarySignature, secondarySignature);
    }

    /// <summary>
    /// Verify dual signatures are both valid
    /// </summary>
    public Task<bool> VerifyDualSignature(string primarySignatureId, string secondarySignatureId, string assetId)
    {
        var primary = _signatures.FirstOrDefault(s => s.SignatureId == primarySignatureId);
        var secondary = _signatures.FirstOrDefault(s => s.SignatureId == secondarySignatureId);
        
        if (primary == null || secondary == null)
        {
            _logger.LogWarning("Dual signature verification failed: one or both signatures not found");
            return Task.FromResult(false);
        }
        
        // Both must be valid and for the same asset
        if (!primary.IsValid || !secondary.IsValid)
        {
            _logger.LogWarning("Dual signature verification failed: one or both signatures expired");
            return Task.FromResult(false);
        }
        
        if (!string.Equals(primary.AssetId, assetId, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(secondary.AssetId, assetId, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Dual signature verification failed: asset mismatch");
            return Task.FromResult(false);
        }
        
        // Signers must be different
        if (string.Equals(primary.SignerAddress, secondary.SignerAddress, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Dual signature verification failed: same signer for both");
            return Task.FromResult(false);
        }
        
        _logger.LogInformation("Dual signature verification successful for asset {AssetId}", assetId);
        return Task.FromResult(true);
    }

    /// <summary>
    /// Get chrono governance statistics
    /// </summary>
    public Task<ChronoGovernanceStats> GetGovernanceStats()
    {
        var now = DateTime.UtcNow;
        var stats = new ChronoGovernanceStats
        {
            TotalSignatures = _signatures.Count,
            ValidSignatures = _signatures.Count(s => s.IsValid),
            ExpiredSignatures = _signatures.Count(s => !s.IsValid),
            AssetsWithSignatures = _assetSignatures.Count,
            AveragePiQuarterCycle = _signatures.Any() ? _signatures.Average(s => s.PiQuarterCycle) : 0,
            ExpiringWithin24Hours = _signatures.Count(s => s.IsValid && s.ExpiresAt <= now.AddHours(24)),
            LastSignatureIssuedAt = _signatures.Any() ? _signatures.Max(s => s.IssuedAt) : DateTime.MinValue
        };
        
        return Task.FromResult(stats);
    }

    /// <summary>
    /// Calculate π⁴ cycle alignment for current time
    /// </summary>
    private double CalculatePiQuarterCycle(DateTime timestamp)
    {
        var ticks = timestamp.Ticks;
        return (ticks % (long)(PiFourth * 10000000)) / (PiFourth * 10000000);
    }

    /// <summary>
    /// Generate cryptographic signature
    /// </summary>
    private string GenerateCryptoSignature(string signatureId, string signerAddress, string assetId, DateTime issuedAt)
    {
        var data = $"{signatureId}|{signerAddress}|{assetId}|{issuedAt:O}|{PiFourth}";
        var bytes = Encoding.UTF8.GetBytes(data);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}

/// <summary>
/// Chrono governance statistics
/// </summary>
public record ChronoGovernanceStats
{
    public int TotalSignatures { get; init; }
    public int ValidSignatures { get; init; }
    public int ExpiredSignatures { get; init; }
    public int AssetsWithSignatures { get; init; }
    public double AveragePiQuarterCycle { get; init; }
    public int ExpiringWithin24Hours { get; init; }
    public DateTime LastSignatureIssuedAt { get; init; }
}
