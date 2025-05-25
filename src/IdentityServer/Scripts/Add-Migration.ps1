param ([string] $name = "SampleMigration")

dotnet ef migrations add ($name + "_Configuration") `
  --project "../" `
  --startup-project "../" `
  --output-dir Data\Migrations\Configuration `
  --context ConfigurationDbContext


dotnet ef migrations add ($name + "_PersistedGrant") `
  --project "../" `
  --startup-project "../" `
  --output-dir Data\Migrations\PersistedGrant `
  --context PersistedGrantDbContext

dotnet ef migrations add ($name + "_Application") `
  --project "../" `
  --startup-project "../" `
  --output-dir Data\Migrations\Application `
  --context ApplicationDbContext

