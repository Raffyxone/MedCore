using Api.Endpoints;
using Application;
using Infrastructure;
using Infrastructure.Settings;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;

var builder = WebApplication.CreateBuilder(args);

var vaultUrl = builder.Configuration["Vault:Url"] ?? "http://localhost:8201";
var vaultToken = builder.Configuration["Vault:Token"] ?? "dev-only-token";

IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken);
var vaultClientSettings = new VaultClientSettings(vaultUrl, authMethod);
IVaultClient vaultClient = new VaultClient(vaultClientSettings);

try
{
    var smtpSecrets = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "smtp", mountPoint: "secret");
    var smtpData = smtpSecrets.Data.Data;

    builder.Configuration["SmtpSettings:Host"] = smtpData["Host"]?.ToString();
    builder.Configuration["SmtpSettings:Port"] = smtpData["Port"]?.ToString();
    builder.Configuration["SmtpSettings:User"] = smtpData["User"]?.ToString();
    builder.Configuration["SmtpSettings:Password"] = smtpData["Password"]?.ToString();
    builder.Configuration["SmtpSettings:SenderName"] = smtpData["SenderName"]?.ToString();
}
catch (Exception ex)
{
    Console.WriteLine($"[Advertencia] No se pudo conectar a Vault o leer los secretos SMTP: {ex.Message}");
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure("Server=localhost,1433;Database=MedCoreDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"); builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapUserEndpoints();

app.Run();
