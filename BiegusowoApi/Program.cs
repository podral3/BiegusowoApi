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
    app.MapOpenApi();
    app.UseScalar();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
