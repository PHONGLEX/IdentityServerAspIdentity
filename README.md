# IdentityServerAspIdentity


At first, we need to remove Data/Migrations folder along with create your own database for IdentityServer.

Second, we need to fix the connection string in appsettings.json file.

Finally, to create the migrations, open a command prompt in the IdentityServer project directory. In the command prompt run these commands:

=============================== For PersistantGrant ===============================
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb --project TokenService



=============================== For Configuration ===============================

dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb --project TokenService



=============================== For AspNet Identity ===============================

dotnet ef migrations add InitialAspNetIdentityDbMigration -c ApplicationDbContext -o Data/Migrations --project TokenService



=============================== Update database for AspNet Identity ===============================


Update-Database -context ApplicationDbContext

You should now see a ~/Data/Migrations/IdentityServer folder in the project. This contains the code for the newly created migrations.
