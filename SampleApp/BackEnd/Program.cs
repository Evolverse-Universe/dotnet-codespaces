using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using BackEnd.Services;
using BackEnd.Services.BLEU;
using BackEnd.Services.ES0IL;
using BackEnd.Services.MetaSchools;
using BackEnd.Services.Forensics;
using BackEnd.Services.VisualAnalysis;
using BackEnd.Models.BLEU;
using BackEnd.Models.ES0IL;
using BackEnd.Models.MetaSchools;
using BackEnd.Models.Forensics;
using BackEnd.Models.VisualAnalysis;

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
// Register ES0IL and MetaSchools services
builder.Services.AddSingleton<ES0ILService>();
builder.Services.AddSingleton<MetaSchoolsService>();
// Register Unified Economic Ledger
builder.Services.AddSingleton<UnifiedEconomicLedgerService>();
// Register Blockchain Forensic Analysis Service
builder.Services.AddSingleton<ForensicAnalysisService>();
// Register Visual Analysis Service for security sweep checks
builder.Services.AddSingleton<VisualAnalysisService>();

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

// ========== ES0IL Smart Forestry & Agriculture API Endpoints ==========

// Get all ES0IL modules
app.MapGet("/es0il/modules", async (ES0ILService service) =>
{
    var modules = await service.GetAllModules();
    return Results.Ok(modules);
})
.WithName("GetES0ILModules")
.WithTags("ES0IL");

// Get ES0IL module by ID
app.MapGet("/es0il/module/{moduleId}", async (ES0ILService service, string moduleId) =>
{
    var module = await service.GetModule(moduleId);
    return module != null ? Results.Ok(module) : Results.NotFound();
})
.WithName("GetES0ILModule")
.WithTags("ES0IL");

// Deploy a new ES0IL module
app.MapPost("/es0il/deploy", async (ES0ILService service, ES0ILLocation location, string primaryCrop) =>
{
    var module = await service.DeployModule(location, primaryCrop);
    return Results.Ok(module);
})
.WithName("DeployES0ILModule")
.WithTags("ES0IL");

// Activate an ES0IL module
app.MapPost("/es0il/activate/{moduleId}", async (ES0ILService service, string moduleId) =>
{
    try
    {
        var module = await service.ActivateModule(moduleId);
        return Results.Ok(module);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
})
.WithName("ActivateES0ILModule")
.WithTags("ES0IL");

// Register forestry resource
app.MapPost("/es0il/forestry/register", async (ES0ILService service, string species, int age, double carbonCapture, bool es0ilIntegration) =>
{
    var resource = await service.RegisterForestryResource(species, age, carbonCapture, es0ilIntegration);
    return Results.Ok(resource);
})
.WithName("RegisterForestryResource")
.WithTags("ES0IL");

// Get all forestry resources
app.MapGet("/es0il/forestry", async (ES0ILService service) =>
{
    var resources = await service.GetForestryResources();
    return Results.Ok(resources);
})
.WithName("GetForestryResources")
.WithTags("ES0IL");

// Get ES0IL statistics
app.MapGet("/es0il/stats", async (ES0ILService service) =>
{
    var stats = await service.GetStatistics();
    return Results.Ok(stats);
})
.WithName("GetES0ILStatistics")
.WithTags("ES0IL");

// Get ES0IL ledger
app.MapGet("/es0il/ledger", async (ES0ILService service, string? moduleId) =>
{
    var ledger = await service.GetLedger(moduleId);
    return Results.Ok(ledger);
})
.WithName("GetES0ILLedger")
.WithTags("ES0IL");

// Calculate ecosystem value
app.MapGet("/es0il/ecosystem-value", async (ES0ILService service) =>
{
    var value = await service.CalculateEcosystemValue();
    return Results.Ok(new { totalEcosystemValue = value });
})
.WithName("CalculateEcosystemValue")
.WithTags("ES0IL");

// ========== MetaSchools API Endpoints ==========

// Enroll a student
app.MapPost("/metaschools/enroll", async (MetaSchoolsService service, string name, AgeGroup ageGroup) =>
{
    var student = await service.EnrollStudent(name, ageGroup);
    return Results.Ok(student);
})
.WithName("EnrollStudent")
.WithTags("MetaSchools");

// Get all students
app.MapGet("/metaschools/students", async (MetaSchoolsService service) =>
{
    var students = await service.GetAllStudents();
    return Results.Ok(students);
})
.WithName("GetAllStudents")
.WithTags("MetaSchools");

// Get student by ID
app.MapGet("/metaschools/student/{studentId}", async (MetaSchoolsService service, string studentId) =>
{
    var student = await service.GetStudent(studentId);
    return student != null ? Results.Ok(student) : Results.NotFound();
})
.WithName("GetStudent")
.WithTags("MetaSchools");

// Get students by age group
app.MapGet("/metaschools/students/age/{ageGroup}", async (MetaSchoolsService service, AgeGroup ageGroup) =>
{
    var students = await service.GetStudentsByAgeGroup(ageGroup);
    return Results.Ok(students);
})
.WithName("GetStudentsByAgeGroup")
.WithTags("MetaSchools");

// Progress a student
app.MapPost("/metaschools/progress/{studentId}", async (MetaSchoolsService service, string studentId, double masteryLevel) =>
{
    try
    {
        var student = await service.ProgressStudent(studentId, masteryLevel);
        return Results.Ok(student);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
})
.WithName("ProgressStudent")
.WithTags("MetaSchools");

// Record teaching session
app.MapPost("/metaschools/teach", async (MetaSchoolsService service, string teacherId, List<string> studentIds, string moduleId, double effectiveness) =>
{
    try
    {
        var session = await service.RecordTeachingSession(teacherId, studentIds, moduleId, effectiveness);
        return Results.Ok(session);
    }
    catch (KeyNotFoundException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("RecordTeachingSession")
.WithTags("MetaSchools");

// Get curriculum
app.MapGet("/metaschools/curriculum", async (MetaSchoolsService service) =>
{
    var curriculum = await service.GetCurriculum();
    return Results.Ok(curriculum);
})
.WithName("GetCurriculum")
.WithTags("MetaSchools");

// Get curriculum for age group
app.MapGet("/metaschools/curriculum/age/{ageGroup}", async (MetaSchoolsService service, AgeGroup ageGroup) =>
{
    var curriculum = await service.GetCurriculumForAgeGroup(ageGroup);
    return Results.Ok(curriculum);
})
.WithName("GetCurriculumForAgeGroup")
.WithTags("MetaSchools");

// Get MetaSchools statistics
app.MapGet("/metaschools/stats", async (MetaSchoolsService service) =>
{
    var stats = await service.GetStatistics();
    return Results.Ok(stats);
})
.WithName("GetMetaSchoolsStatistics")
.WithTags("MetaSchools");

// Get student progression report
app.MapGet("/metaschools/progress-report/{studentId}", async (MetaSchoolsService service, string studentId) =>
{
    try
    {
        var report = await service.GetProgressionReport(studentId);
        return Results.Ok(report);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
})
.WithName("GetProgressionReport")
.WithTags("MetaSchools");

// ========== Unified Economic Ledger API Endpoints ==========

// Record a transaction
app.MapPost("/ledger/record", async (UnifiedEconomicLedgerService service, string system, string operation, string entityId, decimal value, Dictionary<string, object>? metadata) =>
{
    var transaction = await service.RecordTransaction(system, operation, entityId, value, metadata);
    return Results.Ok(transaction);
})
.WithName("RecordTransaction")
.WithTags("Unified Ledger");

// Get all transactions
app.MapGet("/ledger/transactions", async (UnifiedEconomicLedgerService service) =>
{
    var transactions = await service.GetAllTransactions();
    return Results.Ok(transactions);
})
.WithName("GetAllTransactions")
.WithTags("Unified Ledger");

// Get transactions by system
app.MapGet("/ledger/transactions/system/{system}", async (UnifiedEconomicLedgerService service, string system) =>
{
    var transactions = await service.GetTransactionsBySystem(system);
    return Results.Ok(transactions);
})
.WithName("GetTransactionsBySystem")
.WithTags("Unified Ledger");

// Get transactions by entity
app.MapGet("/ledger/transactions/entity/{entityId}", async (UnifiedEconomicLedgerService service, string entityId) =>
{
    var transactions = await service.GetTransactionsByEntity(entityId);
    return Results.Ok(transactions);
})
.WithName("GetTransactionsByEntity")
.WithTags("Unified Ledger");

// Get ledger statistics
app.MapGet("/ledger/stats", async (UnifiedEconomicLedgerService service) =>
{
    var stats = await service.GetStatistics();
    return Results.Ok(stats);
})
.WithName("GetLedgerStatistics")
.WithTags("Unified Ledger");

// Verify transaction
app.MapGet("/ledger/verify/{transactionId}", async (UnifiedEconomicLedgerService service, string transactionId) =>
{
    var isValid = await service.VerifyTransaction(transactionId);
    return Results.Ok(new { transactionId, isValid });
})
.WithName("VerifyTransaction")
.WithTags("Unified Ledger");

// Get total ecosystem value
app.MapGet("/ledger/ecosystem-value", async (UnifiedEconomicLedgerService service) =>
{
    var value = await service.GetTotalEcosystemValue();
    return Results.Ok(value);
})
.WithName("GetTotalEcosystemValue")
.WithTags("Unified Ledger");

// Export ledger to CSV
app.MapGet("/ledger/export/csv", async (UnifiedEconomicLedgerService service) =>
{
    var csv = await service.ExportToCSV();
    return Results.Text(csv, "text/csv");
})
.WithName("ExportLedgerCSV")
.WithTags("Unified Ledger");

// ========== Blockchain Forensic Analysis API Endpoints ==========

// Create a new forensic investigation case
app.MapPost("/forensics/case/create", async (ForensicAnalysisService service, CreateForensicCaseRequest request) =>
{
    var forensicCase = await service.CreateCase(request.Title, request.Description, request.Addresses, request.Networks);
    return Results.Ok(forensicCase);
})
.WithName("CreateForensicCase")
.WithTags("Forensic Analysis")
.WithDescription("Create a new forensic investigation case for tracking blockchain asset movements");

// Get all forensic cases
app.MapGet("/forensics/cases", async (ForensicAnalysisService service) =>
{
    var cases = await service.GetAllCases();
    return Results.Ok(cases);
})
.WithName("GetAllForensicCases")
.WithTags("Forensic Analysis");

// Get forensic case by ID
app.MapGet("/forensics/case/{caseId}", async (ForensicAnalysisService service, string caseId) =>
{
    var forensicCase = await service.GetCase(caseId);
    return forensicCase != null ? Results.Ok(forensicCase) : Results.NotFound();
})
.WithName("GetForensicCase")
.WithTags("Forensic Analysis");

// Record a transaction for forensic analysis
app.MapPost("/forensics/transaction/record", async (ForensicAnalysisService service, RecordTransactionRequest request) =>
{
    var transaction = await service.RecordTransaction(request.TxHash, request.BlockNumber, request.Timestamp, request.Network, request.From, request.To, request.Value, request.MethodName);
    return Results.Ok(transaction);
})
.WithName("RecordForensicTransaction")
.WithTags("Forensic Analysis")
.WithDescription("Record a blockchain transaction for forensic tracking and analysis");

// Trace asset flow
app.MapPost("/forensics/flow/trace", async (ForensicAnalysisService service, TraceAssetFlowRequest request) =>
{
    var flow = await service.TraceAssetFlow(request.AssetSymbol, request.AssetContract, request.SourceAddress, request.DestinationAddress, request.Amount, request.SourceNetwork, request.DestinationNetwork, request.TxHash);
    return Results.Ok(flow);
})
.WithName("TraceAssetFlow")
.WithTags("Forensic Analysis")
.WithDescription("Trace asset movement from source to destination address across blockchains");

// Get asset flows for a case
app.MapGet("/forensics/case/{caseId}/flows", async (ForensicAnalysisService service, string caseId) =>
{
    var flows = await service.GetAssetFlowsForCase(caseId);
    return Results.Ok(flows);
})
.WithName("GetCaseAssetFlows")
.WithTags("Forensic Analysis");

// Generate chain of custody
app.MapGet("/forensics/case/{caseId}/chain-of-custody", async (ForensicAnalysisService service, string caseId) =>
{
    var custody = await service.GenerateChainOfCustody(caseId);
    return Results.Ok(custody);
})
.WithName("GenerateChainOfCustody")
.WithTags("Forensic Analysis")
.WithDescription("Generate a legal chain of custody provenance trail for a forensic case");

// Generate comprehensive forensic report
app.MapPost("/forensics/report/generate", async (ForensicAnalysisService service, GenerateReportRequest request) =>
{
    try
    {
        var report = await service.GenerateForensicReport(request.CaseId, request.PreparedBy);
        return Results.Ok(report);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { error = $"Case {request.CaseId} not found" });
    }
})
.WithName("GenerateForensicReport")
.WithTags("Forensic Analysis")
.WithDescription("Generate a comprehensive forensic report for legal/insurance proceedings");

// Analyze a swap transaction
app.MapPost("/forensics/swap/analyze", async (ForensicAnalysisService service, AnalyzeSwapRequest request) =>
{
    var analysis = await service.AnalyzeSwap(request.TxHash, request.DexName, request.RouterAddress, request.Network, request.TokenIn, request.TokenOut, request.AmountIn, request.AmountOut, request.ExpectedAmountOut, request.SwapMethod);
    return Results.Ok(analysis);
})
.WithName("AnalyzeSwap")
.WithTags("Forensic Analysis")
.WithDescription("Analyze a DEX swap transaction for slippage and potential issues");

// Analyze a bridge transaction
app.MapPost("/forensics/bridge/analyze", async (ForensicAnalysisService service, AnalyzeBridgeRequest request) =>
{
    var analysis = await service.AnalyzeBridge(request.BridgeName, request.SourceChain, request.DestinationChain, request.SourceTxHash, request.DestinationTxHash, request.AmountSent, request.AmountReceived);
    return Results.Ok(analysis);
})
.WithName("AnalyzeBridge")
.WithTags("Forensic Analysis")
.WithDescription("Analyze a cross-chain bridge transaction for completion status and potential issues");

// Get address analytics
app.MapGet("/forensics/address/{address}/analytics", async (ForensicAnalysisService service, string address, BlockchainNetwork network) =>
{
    var analytics = await service.GetAddressAnalytics(address, network);
    return Results.Ok(analytics);
})
.WithName("GetAddressAnalytics")
.WithTags("Forensic Analysis")
.WithDescription("Get forensic analytics profile for a blockchain address");

// Get transactions by address
app.MapGet("/forensics/address/{address}/transactions", async (ForensicAnalysisService service, string address) =>
{
    var transactions = await service.GetTransactionsByAddress(address);
    return Results.Ok(transactions);
})
.WithName("GetTransactionsByAddress")
.WithTags("Forensic Analysis");

// Export case to CSV
app.MapGet("/forensics/case/{caseId}/export/csv", async (ForensicAnalysisService service, string caseId) =>
{
    var csv = await service.ExportCaseToCSV(caseId);
    if (string.IsNullOrEmpty(csv))
        return Results.NotFound();
    return Results.Text(csv, "text/csv");
})
.WithName("ExportCaseToCSV")
.WithTags("Forensic Analysis")
.WithDescription("Export case asset flows to CSV for external analysis");

// Get forensic statistics
app.MapGet("/forensics/stats", async (ForensicAnalysisService service) =>
{
    var stats = await service.GetStatistics();
    return Results.Ok(stats);
})
.WithName("GetForensicStatistics")
.WithTags("Forensic Analysis");

// ========== Visual Analysis Security Sweep API Endpoints ==========
// Based on visual-analysis rules for disciplined, professional pattern recognition

// Get available analysis types
app.MapGet("/visual-analysis/types", async (VisualAnalysisService service) =>
{
    var types = await service.GetAnalysisTypes();
    return Results.Ok(types);
})
.WithName("GetVisualAnalysisTypes")
.WithTags("Visual Analysis")
.WithDescription("Get available security sweep check types: 1-SystemIntegrity, 2-NetworkBehavior, 3-GitHubAnalysis, 4-DeviceCompromise, 5-ImpersonationDetection, 6-GovSignatureDetection, 7-AISyntheticDetection");

// Perform a specific visual analysis
app.MapPost("/visual-analysis/analyze", async (VisualAnalysisService service, VisualAnalysisRequest request) =>
{
    var result = await service.PerformAnalysis(request);
    return Results.Ok(result);
})
.WithName("PerformVisualAnalysis")
.WithTags("Visual Analysis")
.WithDescription("Perform a security sweep check based on visual-analysis rules");

// Run comprehensive security sweep (all 7 analysis types)
app.MapPost("/visual-analysis/comprehensive-sweep", async (VisualAnalysisService service) =>
{
    var session = await service.RunComprehensiveSweep();
    return Results.Ok(session);
})
.WithName("RunComprehensiveSweep")
.WithTags("Visual Analysis")
.WithDescription("Run all 7 security sweep checks: system integrity, network behavior, GitHub analysis, device compromise, impersonation detection, government signatures, and AI synthetic detection");

// Create a new analysis session
app.MapPost("/visual-analysis/session/create", async (VisualAnalysisService service) =>
{
    var session = await service.CreateSession();
    return Results.Ok(session);
})
.WithName("CreateVisualAnalysisSession")
.WithTags("Visual Analysis")
.WithDescription("Create a new analysis session for multiple sequential checks");

// Add analysis to session
app.MapPost("/visual-analysis/session/{sessionId}/analyze", async (VisualAnalysisService service, string sessionId, VisualAnalysisType analysisType) =>
{
    var session = await service.AddAnalysisToSession(sessionId, analysisType);
    return session != null ? Results.Ok(session) : Results.NotFound(new { error = $"Session {sessionId} not found" });
})
.WithName("AddAnalysisToSession")
.WithTags("Visual Analysis")
.WithDescription("Add a specific analysis type to an existing session");

// Get session details
app.MapGet("/visual-analysis/session/{sessionId}", async (VisualAnalysisService service, string sessionId) =>
{
    var session = await service.GetSession(sessionId);
    return session != null ? Results.Ok(session) : Results.NotFound();
})
.WithName("GetVisualAnalysisSession")
.WithTags("Visual Analysis")
.WithDescription("Get details of a visual analysis session including all performed analyses");

// Get visual analysis statistics
app.MapGet("/visual-analysis/stats", async (VisualAnalysisService service) =>
{
    var stats = await service.GetStatistics();
    return Results.Ok(stats);
})
.WithName("GetVisualAnalysisStatistics")
.WithTags("Visual Analysis")
.WithDescription("Get statistics on visual analyses performed");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
