using Api.Endpoints;
using Application;
using Infrastructure;
using Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure("Server=(localdb)\\mssqllocaldb;Database=MedCoreDb;Trusted_Connection=True;TrustServerCertificate=True;");

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapUserEndpoints();

app.Run();