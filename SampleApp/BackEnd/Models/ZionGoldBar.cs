namespace BackEnd.Models;

/// <summary>
/// Represents a resource in the Zion Gold Bar Classification - Saturn Strata
/// </summary>
public record SaturnResource(
    string Layer,
    string Symbol,
    string Resource,
    string Function,
    string SectoralUse
);

/// <summary>
/// Represents a Zion Gold Bar certificate
/// </summary>
public record ZionGoldBarCertificate(
    string CertificateId,
    string Layer,
    string Symbol,
    string Resource,
    DateTime IssuedDate,
    string IssuedTo,
    decimal Value
);

/// <summary>
/// Represents an ENFT (Enhanced NFT) Codex entry for Zion Gold Bars
/// </summary>
public record EnftCodexEntry(
    string TokenId,
    string Layer,
    string Symbol,
    string Resource,
    string Function,
    string SectoralUse,
    string MemorialSite,
    string AncestralLineage,
    DateTime MintedDate,
    string HashSignature
);
