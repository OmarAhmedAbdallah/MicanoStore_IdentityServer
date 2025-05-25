param ([string] $name = "SampleMigration")

dotnet ef migrations add $name `
  --project "../" `
  --startup-project "../" `
  --output-dir Data\Migrations `
  --context ConfigurationDbContext

dotnet ef migrations add $name `
  --project "../" `
  --startup-project "../" `
  --output-dir Data\Migrations `
  --context ApplicationPersistedGrantDbContext

dotnet ef migrations add $name `
  --project "../" `
  --startup-project "../" `
  --output-dir Data\Migrations `
  --context ApplicationDbContext

