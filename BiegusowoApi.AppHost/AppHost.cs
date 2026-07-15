var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgAdmin(pgadmin => pgadmin.WithHostPort(2137));

var appDb = postgres.AddDatabase("biegusowo");

var keycloak = builder.AddKeycloak("keycloak", 2136)
    .WithDataVolume();

builder.AddProject<Projects.BiegusowoApi>("biegusowoapi")
    .WithReference(appDb)
    .WithReference(keycloak)
    .WaitFor(appDb)
    .WaitFor(keycloak);

builder.Build().Run();
