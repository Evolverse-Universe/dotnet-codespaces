using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using BackEnd.Services;
using BackEnd.Services.BLEU;
using BackEnd.Models.BLEU;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    // current workaround for port forwarding in codespaces
    // https://github.com/dotnet/aspnetcore/issues/57332
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Servers = [];
        return Task.CompletedTask;
    });
});

// Add Zion Gold Bar services
builder.Services.AddSingleton<SaturnResourceService>();
builder.Services.AddSingleton<PdfCertificateService>();
// Register BLEU Flame services
builder.Services.AddSingleton<BLEUFlameService>();
builder.Services.AddSingleton<MetaVaultService>();
builder.Services.AddSingleton<ZionGoldBarService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Zion Gold Bar API Endpoints
app.MapGet("/saturn-resources", (SaturnResourceService service) =>
{
    return Results.Ok(service.GetAllSaturnResources());
})
.WithName("GetSaturnResources")
.WithDescription("Get all Saturn Strata resources in the Zion Gold Bar classification system");

app.MapPost("/certificate/generate", (SaturnResourceService service, PdfCertificateService pdfService, string issuedTo, decimal value) =>
{
    var certificate = service.GenerateCertificate(issuedTo, value);
    var pdfBytes = pdfService.GenerateCertificatePdf(certificate);
    return Results.File(pdfBytes, "application/pdf", $"ZionGoldBar-{certificate.CertificateId}.pdf");
})
.WithName("GenerateCertificate")
.WithDescription("Generate a BLEU Vault Gold Bar Certificate PDF");

app.MapPost("/enft/mint", (SaturnResourceService service, string layer, string memorialSite, string ancestralLineage) =>
{
    var enftToken = service.MintEnftToken(layer, memorialSite, ancestralLineage);
    return Results.Ok(enftToken);
})
.WithName("MintEnftToken")
.WithDescription("Mint a new ENFT (Enhanced NFT) Codex entry for Zion Gold Bars");
// BLEU Flame™ API Endpoints

// Get collection metadata
app.MapGet("/bleu/metadata", async (BLEUFlameService service) =>
{
    var metadata = await service.GetCollectionMetadata();
    return metadata != null ? Results.Ok(metadata) : Results.NotFound();
})
.WithName("GetBLEUMetadata")
.WithTags("BLEU Flame");

// Mint a new BLEU Flame™ ENFT
app.MapPost("/bleu/mint", async (BLEUFlameService service, ENFTTier tier, string owner) =>
{
    var attributes = new ENFTAttributes
    {
        SmartCeramicType = "Adaptive Cookware",
        Temperature = Random.Shared.Next(300, 500),
        MemoryIndex = Random.Shared.NextDouble() * 2.0,
        ThermalCalibration = Random.Shared.NextDouble() * 10.0,
        Features = tier switch
        {
            ENFTTier.PublicDrop => new[] { "Thermal calibration tracking", "Memory log recording", "Basic yield generation" },
            ENFTTier.EliteFounders => new[] { "Advanced thermal calibration", "Enhanced memory indexing", "2x yield multiplier" },
            ENFTTier.GodTier => new[] { "Master thermal calibration", "Maximum memory indexing", "5x yield multiplier", "Exclusive MetaVault priority" },
            _ => Array.Empty<string>()
        }
    };
    
    var nft = await service.MintENFT(tier, owner, attributes);
    return Results.Ok(nft);
})
.WithName("MintBLEUFlame")
.WithTags("BLEU Flame");

// Get ENFT by token ID
app.MapGet("/bleu/nft/{tokenId}", async (BLEUFlameService service, string tokenId) =>
{
    var nft = await service.GetENFTMetadata(tokenId);
    return nft != null ? Results.Ok(nft) : Results.NotFound();
})
.WithName("GetBLEUNFT")
.WithTags("BLEU Flame");

// Get ENFTs by tier
app.MapGet("/bleu/tier/{tier}", async (BLEUFlameService service, ENFTTier tier) =>
{
    var nfts = await service.GetENFTsByTier(tier);
    return Results.Ok(nfts);
})
.WithName("GetBLEUNFTsByTier")
.WithTags("BLEU Flame");

// Get ENFTs by owner
app.MapGet("/bleu/owner/{owner}", async (BLEUFlameService service, string owner) =>
{
    var nfts = await service.GetENFTsByOwner(owner);
    return Results.Ok(nfts);
})
.WithName("GetBLEUNFTsByOwner")
.WithTags("BLEU Flame");

// Get registry statistics
app.MapGet("/bleu/stats", async (BLEUFlameService service) =>
{
    var stats = await service.GetRegistryStats();
    return Results.Ok(new
    {
        stats.total,
        stats.publicDrop,
        stats.eliteFounders,
        stats.godTier
    });
})
.WithName("GetBLEUStats")
.WithTags("BLEU Flame");

// Calculate revenue split
app.MapGet("/bleu/revenue-split/{amount}", (BLEUFlameService service, decimal amount) =>
{
    var split = service.CalculateRevenueSplit(amount);
    return Results.Ok(new
    {
        totalAmount = amount,
        publicDrop = split.publicDrop,
        eliteFounders = split.eliteFounders,
        godTier = split.godTier
    });
})
.WithName("GetRevenueSplit")
.WithTags("BLEU Flame");

// MetaVault API Endpoints

// Trigger vault event
app.MapPost("/metavault/trigger", async (MetaVaultService service, string tokenId, double temperature, double memoryIndex, ENFTTier tier) =>
{
    var vaultEvent = await service.TriggerVaultEvent(tokenId, temperature, memoryIndex, tier);
    return Results.Ok(vaultEvent);
})
.WithName("TriggerVaultEvent")
.WithTags("MetaVault");

// Get vault events by token
app.MapGet("/metavault/events/{tokenId}", async (MetaVaultService service, string tokenId) =>
{
    var events = await service.GetVaultEventsByToken(tokenId);
    return Results.Ok(events);
})
.WithName("GetVaultEvents")
.WithTags("MetaVault");

// Get total yield for token
app.MapGet("/metavault/yield/{tokenId}", async (MetaVaultService service, string tokenId) =>
{
    var totalYield = await service.GetTotalYieldForToken(tokenId);
    return Results.Ok(new { tokenId, totalYield });
})
.WithName("GetTokenYield")
.WithTags("MetaVault");

// Stake EGoin
app.MapPost("/metavault/stake", async (MetaVaultService service, string address, decimal amount) =>
{
    var success = await service.StakeEGoin(address, amount);
    return success ? Results.Ok(new { message = "Staked successfully", address, amount }) : Results.BadRequest("Invalid stake amount");
})
.WithName("StakeEGoin")
.WithTags("MetaVault");

// Get Energy Treasury status
app.MapGet("/metavault/treasury", async (MetaVaultService service) =>
{
    var treasury = await service.GetEnergyTreasuryStatus();
    return Results.Ok(treasury);
})
.WithName("GetEnergyTreasury")
.WithTags("MetaVault");

// Get vault statistics
app.MapGet("/metavault/stats", async (MetaVaultService service) =>
{
    var stats = await service.GetVaultStatistics();
    return Results.Ok(new
    {
        stats.totalEvents,
        stats.totalYield,
        stats.averageYield
    });
})
.WithName("GetVaultStats")
.WithTags("MetaVault");

// Execute Harvest-Mint-Heal loop
app.MapPost("/metavault/harvest-mint-heal", async (MetaVaultService service, string tokenId, double temperature, double memoryIndex, ENFTTier tier) =>
{
    var result = await service.ExecuteHarvestMintHealLoop(tokenId, temperature, memoryIndex, tier);
    return Results.Ok(new { message = result });
})
.WithName("HarvestMintHeal")
.WithTags("MetaVault");

// Zion Gold Bar API Endpoints

// Get Zion Gold Bar collection metadata
app.MapGet("/zion/metadata", async (ZionGoldBarService service) =>
{
    var metadata = await service.GetCollectionMetadata();
    return metadata != null ? Results.Ok(metadata) : Results.NotFound();
})
.WithName("GetZionMetadata")
.WithTags("Zion Gold Bar");

// Mint a new Zion Gold Bar ENFT
app.MapPost("/zion/mint", async (ZionGoldBarService service, string creatorAddress, string imageCID, string certificateCID) =>
{
    var zionBar = await service.MintZionGoldBar(creatorAddress, imageCID, certificateCID);
    return Results.Ok(zionBar);
})
.WithName("MintZionGoldBar")
.WithTags("Zion Gold Bar");

// Get Zion Gold Bar by token ID
app.MapGet("/zion/bar/{tokenId}", async (ZionGoldBarService service, string tokenId) =>
{
    var bar = await service.GetZionGoldBar(tokenId);
    return bar != null ? Results.Ok(bar) : Results.NotFound();
})
.WithName("GetZionGoldBar")
.WithTags("Zion Gold Bar");

// Get Zion Gold Bars by owner
app.MapGet("/zion/owner/{address}", async (ZionGoldBarService service, string address) =>
{
    var bars = await service.GetZionGoldBarsByOwner(address);
    return Results.Ok(bars);
})
.WithName("GetZionGoldBarsByOwner")
.WithTags("Zion Gold Bar");

// Generate mint-ready metadata for IPFS
app.MapPost("/zion/generate-metadata", async (ZionGoldBarService service, string tokenId, string imageCID, string certificateCID, string creatorAddress) =>
{
    var metadata = await service.GenerateMetadata(tokenId, imageCID, certificateCID, creatorAddress);
    return Results.Ok(metadata);
})
.WithName("GenerateZionMetadata")
.WithTags("Zion Gold Bar");

// Get Saturn-Strata layers information
app.MapGet("/zion/layers", async (ZionGoldBarService service) =>
{
    var layers = await service.GetSaturnStrataLayers();
    return Results.Ok(layers);
})
.WithName("GetSaturnStrataLayers")
.WithTags("Zion Gold Bar");

// Calculate yield for Zion Gold Bar
app.MapGet("/zion/yield/{tokenId}", async (ZionGoldBarService service, string tokenId) =>
{
    var yield = await service.CalculateZionYield(tokenId);
    return Results.Ok(new { tokenId, yield, formula = "Yield = (Weight × Purity × LayerStack) / π⁴" });
})
.WithName("CalculateZionYield")
.WithTags("Zion Gold Bar");

// Verify certificate authenticity
app.MapPost("/zion/verify-certificate", async (ZionGoldBarService service, string tokenId, string certificateCID) =>
{
    var isValid = await service.VerifyCertificate(tokenId, certificateCID);
    return Results.Ok(new { tokenId, certificateCID, isValid });
})
.WithName("VerifyZionCertificate")
.WithTags("Zion Gold Bar");

// Get Zion Gold Bar registry statistics
app.MapGet("/zion/stats", async (ZionGoldBarService service) =>
{
    var stats = await service.GetRegistryStats();
    return Results.Ok(new
    {
        stats.totalMinted,
        stats.totalWeight,
        stats.totalValue
    });
})
.WithName("GetZionStats")
.WithTags("Zion Gold Bar");

// Update IPFS CIDs after deployment
app.MapPut("/zion/update-cids", async (ZionGoldBarService service, string tokenId, string imageCID, string certificateCID) =>
{
    var success = await service.UpdateIPFSCIDs(tokenId, imageCID, certificateCID);
    return success ? Results.Ok(new { message = "IPFS CIDs updated successfully", tokenId }) : Results.NotFound();
})
.WithName("UpdateZionCIDs")
.WithTags("Zion Gold Bar");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
