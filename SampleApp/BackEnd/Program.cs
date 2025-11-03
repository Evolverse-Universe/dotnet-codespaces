using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using BackEnd.Services;

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

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
