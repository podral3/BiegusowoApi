using BiegusowoApi.Data;
using BiegusowoApi.Data.Seeding;
using BiegusowoApi.Helpers.Composition;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddDatabaseContext(builder.Configuration);
builder.Services.AddKeycloakAuthentication(builder.Configuration, builder.Environment);
builder.Services.AddOpenApiServices();

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