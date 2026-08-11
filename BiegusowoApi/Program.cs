using BiegusowoApi.Data;
using BiegusowoApi.Domain.Account;
using BiegusowoApi.Domain.Blobs;
using BiegusowoApi.Domain.Blobs.Service;
using BiegusowoApi.Domain.Conversations;
using BiegusowoApi.Domain.FileStorage;
using BiegusowoApi.Domain.Listings;
using BiegusowoApi.Domain.Profile;
using BiegusowoApi.Domain.Register;
using BiegusowoApi.ExceptionHandling;
using BiegusowoApi.Helpers.Composition;
using BiegusowoApi.Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddKeycloakAuthentication(builder.Configuration);
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
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();


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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


//for testing purposes
public partial class Program { }