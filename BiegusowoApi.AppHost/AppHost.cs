var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env");

var databaseContextPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../Database")); //bruh 

var pgUserName = builder.AddParameter("postgres-username", "postgres");
var pgPassword = builder.AddParameter("postgres-password", "postgres", secret: true);
var postgres = builder.AddPostgres("postgres",
    userName: pgUserName,
    password: pgPassword,
    port: 5432)

    .WithDockerfile(databaseContextPath)
    .WithDataVolume()
    .WithPgAdmin(pgadmin => pgadmin.WithHostPort(2137));

var appDb = postgres.AddDatabase("biegusowo");

var s3SecretKey = builder.AddParameter("s3-secret-key", secret: true);

builder.AddProject<Projects.BiegusowoApi>("biegusowoapi")
    .WithReference(appDb)
    .WaitFor(appDb)
    .WithEnvironment("S3__SecretKey", s3SecretKey);

builder.Build().Run();
