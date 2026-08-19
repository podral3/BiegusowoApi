using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env")
        .WithDashboard(false); //for staging envs etc it should be on, needs modifications of appsettings

var api = builder.AddProject<Projects.BiegusowoApi>("biegusowoapi");

var useLocalPostgres =
    builder.Configuration.GetValue("UseLocalPostgres", true);

if (useLocalPostgres)
{
    var databaseContextPath = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "../../../../Database"));

    var pgUserName = builder.AddParameter(
        "postgres-username",
        "postgres");

    var pgPassword = builder.AddParameter(
        "postgres-password",
        "postgres",
        secret: true);

    var postgres = builder.AddPostgres(
            "postgres",
            userName: pgUserName,
            password: pgPassword,
            port: 5432)
        .WithDockerfile(databaseContextPath)
        .WithDataVolume()
        .WithPgAdmin(pgadmin =>
            pgadmin.WithHostPort(2137));

    var appDb = postgres.AddDatabase("biegusowo");

    api
        .WithReference(appDb)
        .WaitFor(appDb);
}
else
{
    var appDb = builder.AddConnectionString("DatabaseConnectionString",
        environmentVariableName: "Database__ConnectionString");

    api.WithReference(appDb);
}

var s3SecretKey =
    builder.AddParameter("s3-secret-key", secret: true);
api.WithEnvironment("S3__SecretKey", s3SecretKey);

var supabaseWebhookSecret =
    builder.AddParameter("supabase-webhook-secret", secret: true);
api.WithEnvironment("SupabaseJwt__WebhookSecret", supabaseWebhookSecret);

builder.Build().Run();
