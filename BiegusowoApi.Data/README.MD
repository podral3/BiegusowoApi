# Adding EF Core Migrations

## Prerequisites

- Start the **Aspire AppHost** first — this spins up the Postgres container the migration needs to connect to.
- Make sure `dotnet-ef` tools are up to date:
  ```bash
  dotnet tool update --global dotnet-ef
  ```

## Steps

1. **Run the Aspire AppHost** (from Visual Studio, Rider, or CLI) and leave it running.
   Check the Aspire dashboard to confirm the Postgres resource is up and note the connection details (host/port/db) if they aren't pinned.

2. **Navigate to the Data project:**
   ```bash
   cd BiegusowoApi.Data
   ```

3. **Add a migration:**
   ```bash
   dotnet ef migrations add "DescriptiveMigrationName"
   ```

4. **Apply it to the database:**
   ```bash
   dotnet ef database update
   ```

## Notes

- Design-time connection info comes from `ApplicationDbContextFactory` (`IDesignTimeDbContextFactory<ApplicationDbContext>`), **not** from the running app's DI/Aspire service discovery. It first checks the `ConnectionStrings__mydb` env var, then falls back to a hardcoded local connection string — make sure that fallback matches the Postgres port/credentials pinned in the AppHost.
- The AppHost must be running (or another reachable Postgres matching the fallback connection string) whenever you run `migrations add` or `database update` — EF needs a live database to inspect/apply against.
- To preview SQL without applying it:
  ```bash
  dotnet ef migrations script
  ```
- To list applied/pending migrations:
  ```bash
  dotnet ef migrations list
  ```