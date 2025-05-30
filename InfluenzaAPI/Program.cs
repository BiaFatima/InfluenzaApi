using CsvHelper.Configuration;
using CsvHelper;
using InfluenzaAPI.Models;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Listen on HTTP only
builder.WebHost.UseUrls("http://0.0.0.0:5000");

// Register your cached CSV service
builder.Services.AddSingleton(provider =>
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "influenza_weekly.csv");
    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        MissingFieldFound = null,
        HeaderValidated = null,
    };

    using var reader = new StreamReader(filePath);
    using var csv = new CsvReader(reader, config);
    return csv.GetRecords<InfluenzaRecord>().ToList();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection disabled
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
