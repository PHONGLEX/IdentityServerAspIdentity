# IdentityServerAspIdentity

To create the migrations, open a command prompt in the IdentityServer project directory. In the command prompt run these two commands:

dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

dotnet ef migrations add InitialAspNetIdentityDbMigration -c ApplicationDbContext -o Data/Migrations --project TokenService
Update-Database -context ApplicationDbContext

You should now see a ~/Data/Migrations/IdentityServer folder in the project. This contains the code for the newly created migrations.
