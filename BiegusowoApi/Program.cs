using BiegusowoApi.Data;
using BiegusowoApi.Features.Blobs;
using BiegusowoApi.Features.Conversations;
using BiegusowoApi.Features.Listings;
using BiegusowoApi.Features.Users;
using BiegusowoApi.Shared.ExceptionHandling;
using BiegusowoApi.Shared.Helpers.Composition;
using BiegusowoApi.Shared.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services));

builder.Services.ConfigureCorsOrigins(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddSupabaseAuthentication(builder.Configuration);
builder.Services.AddOpenApiServices();

builder.Services.AddScoped<IFileStorageProvider, S3StorageProvider>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection("FileStorage"));
builder.Services.Configure<S3Options>(
    builder.Configuration.GetSection("S3"));

builder.Services.AddScoped<IConversationService, ConversationService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IListingService, ListingsService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

//app.MapDefaultEndpoints();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("UserId", httpContext.User?.FindFirst("sub")?.Value ?? "anonymous");
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
    };
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //if (!dbContext.Species.Any())
    //{
    //    await new DataSeeder(dbContext).Seed();
    //}

    app.MapOpenApi();
    app.UseScalar();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

//for testing purposes
public partial class Program { }